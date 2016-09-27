using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TreeApp.Models;
using Activities = TreeApp.Models.Activities;

namespace TreeApp.Controllers
{
    public class ActivitiesController : Controller
    {
        private List<Activities> _activitiesList = new List<Activities>();

        [HttpGet]
        public ActionResult Index()
        {
            List<Activities> tempList;

            using (var activitiesDb = new ActivitiesDbCtxt())
            {
                tempList = activitiesDb.Activities.ToList();
            }
            var activitiesList = CreateStructure(0, tempList);
            return View(activitiesList);
        }

        public ActionResult Sort()
        {
            List<Activities> tempList;
            var activityId = int.Parse(RouteData.Values["id"].ToString());
            var sortType = RouteData.Values["sort"].ToString();
            using (var activitiesDb = new ActivitiesDbCtxt())
            {
                tempList = activitiesDb.Activities.ToList();
            }
            var activitiesList = CreateStructure(0, tempList);

            return View(sortType == "asc" ? GetSortedList(activityId, activitiesList, SortType.Ascending) : GetSortedList(activityId, activitiesList, SortType.Descending));
        }

        private List<Activities> GetSortedList(int startSortingAt, List<Activities> activitiesList, SortType sortType)
        {
            var baseList = activitiesList;
            if (startSortingAt == 0)
            {
                if (sortType == SortType.Ascending)
                {
                    baseList = baseList.OrderBy(x => x.ActivityDescription).ToList();
                }
                if (sortType == SortType.Descending)
                {
                    baseList = baseList.OrderByDescending(x => x.ActivityDescription).ToList();
                }
                foreach (var child in baseList)
                {
                    SortList(sortType, child);
                }
            }
            else
            {
                var lookingFor = baseList.Select(item => Activities.Find(item, startSortingAt)).FirstOrDefault(activity => activity != null);
                SortList(sortType, lookingFor);
            }

            return baseList;
        }

        private void SortList(SortType sortType, Activities firstActivity)
        {
            if (firstActivity.ChildActivities.Count == 0) return;
            if (sortType == SortType.Ascending)
            {
                firstActivity.ChildActivities = firstActivity.ChildActivities.OrderBy(x => x.ActivityDescription).ToList();
            }
            else
            {
                firstActivity.ChildActivities =
                    firstActivity.ChildActivities.OrderByDescending(x => x.ActivityDescription).ToList();
            }
            foreach (var activity in firstActivity.ChildActivities)
            {
                SortList(sortType, activity);
            }
        }

        private List<Activities> CreateStructure(int parentActivityId, List<Activities> source, int level = 0)
        {
            return (from activities in source
                    where activities.ParentActivityID == parentActivityId
                    select new Activities()
                    {
                        ActivityDescription = activities.ActivityDescription,
                        ActivityID = activities.ActivityID,
                        ChildActivities = CreateStructure(activities.ActivityID, source, level + 1).ToList(),
                        StartDateTime = activities.StartDateTime,
                        EndDateTime = activities.EndDateTime,
                        ParentActivityID = activities.ParentActivityID,
                        Level = level
                    }).ToList();
        }

        public ActionResult DeleteSelected(IEnumerable<int> ids)
        {
            using (var activitiesDb = new ActivitiesDbCtxt())
            {
                foreach (var activity in ids.Select(id => activitiesDb.Activities.Find(id)).Where(activity => activity != null))
                {
                    activitiesDb.Activities.Remove(activity);
                    activitiesDb.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete()
        {
            var activityId = int.Parse(RouteData.Values["id"].ToString());
            using (var activitiesDb = new ActivitiesDbCtxt())
            {
                var activity = activitiesDb.Activities.Find(activityId);

                if (activity == null) return HttpNotFound();
                activitiesDb.Activities.Remove(activity);
                activitiesDb.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details()
        {
            Activities activity;
            var activityId = int.Parse(RouteData.Values["id"].ToString());

            using (var activitiesDb = new ActivitiesDbCtxt())
            {
                activity = activitiesDb.Activities.Find(activityId);
            }
            return View(activity);
        }

        public ActionResult Edit()
        {
            var activityId = int.Parse(RouteData.Values["id"].ToString());
            var editViewModel = new EditViewModel();
            using (var activitiesDb = new ActivitiesDbCtxt())
            {
                int parentActivityId;
                editViewModel.Categories = GetCategories(activityId, out parentActivityId);
                editViewModel.Activity = activitiesDb.Activities.Find(activityId);
                editViewModel.SelectedParentActivityId = parentActivityId;
                return View(editViewModel);
            }
        }

        private SelectList GetCategories(int currentId, out int parentActivityId)
        {
            var dbCtxt = new ActivitiesDbCtxt();
            var activitiesList = CreateStructure(0, dbCtxt.Activities.ToList());
            var lookingFor = activitiesList.Select(item => Activities.Find(item, currentId)).FirstOrDefault(activity => activity != null);
            var parent = activitiesList.Select(item => Activities.Find(item, lookingFor.ParentActivityID)).FirstOrDefault(activity => activity != null);
            if (parent != null)
            {
                parent.ChildActivities.Remove(lookingFor);
            }
            else
            {
                activitiesList.Remove(lookingFor);
            }
            var childList = GetAllChilds(activitiesList);
            var categories = childList.Select(x => new SelectListItem
            {
                Value = x.ActivityID.ToString(),
                Text = x.DisplayAsCategory
            });

            var list = categories.ToList();
            SelectListItem selected;
            if (parent != null)
            {
                selected = (from category in list
                            where category.Value == parent.ActivityID.ToString()
                            select category).First();
            }
            else
            {
                selected = null;
            }

            if (selected != null)
            {
                selected.Selected = true;
                list.Insert(0, new SelectListItem { Value = "0", Text = "No parent category" });
                parentActivityId = int.Parse(selected.Value);
            }
            else
            {
                list.Insert(0, new SelectListItem { Value = "0", Text = "No parent category", Selected = true });
                parentActivityId = 0;
            }

            return new SelectList(list, "Value", "Text");
        }

        public List<Activities> GetAllChilds(List<Activities> activitiesList)
        {
            foreach (var activity in activitiesList)
            {
                _activitiesList.Add(activity);
                if (activity.ChildActivities.Count > 0)
                {
                    GetAllChilds(activity.ChildActivities);
                }
            }
            return _activitiesList;
        }

        [HttpPost]
        public ActionResult Edit(EditViewModel editViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var activitiesDb = new ActivitiesDbCtxt())
                {
                    var activity = editViewModel.Activity;
                    activity.ParentActivityID = editViewModel.SelectedParentActivityId;
                    activitiesDb.Entry(activity).State = EntityState.Modified; ;
                    activitiesDb.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Add()
        {
            var activityId = int.Parse(RouteData.Values["id"].ToString());
            string parentActivityDescription;

            using (var activitiesDb = new ActivitiesDbCtxt())
            {
                var parentActivity = activitiesDb.Activities.Find(activityId);
                parentActivityDescription = parentActivity?.ActivityDescription ?? "No parent activity";
            }

            ViewBag.ParentActivity = activityId;
            ViewBag.ParentActivityDescription = parentActivityDescription;

            return View();
        }

        [HttpPost]
        public ActionResult Add(Activities activities)
        {
            try
            {
                var activity = new Activities()
                {
                    ParentActivityID = activities.ParentActivityID,
                    ActivityDescription = activities.ActivityDescription,
                    StartDateTime = activities.StartDateTime,
                    EndDateTime = activities.EndDateTime
                };

                using (var activitiesDb = new ActivitiesDbCtxt())
                {
                    var parentActivity = activitiesDb.Activities.Find(activity.ParentActivityID);
                    if (parentActivity == null && activities.ActivityID != 0)
                    {
                        return RedirectToAction("Index");
                    }
                    activitiesDb.Activities.Add(activity);
                    activitiesDb.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ParentActivityId", ex.Message);
                return View("Add", activities);
            }
        }
    }
}