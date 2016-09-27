using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TreeApp.Helpers;
using static System.String;

namespace TreeApp.Models
{
    public class Activities : IComparable
    {
        [NotMapped]
        private string _activityDescription;

        [Key]
        public int ActivityID { get; set; }

        [NotMapped]
        public int Level { get; set; }

        [NotMapped]
        public string DisplayAsCategory => Concat("\xA0".Multiply(Level*5), ActivityDescription);

        [Required]
        [Display(Name = "Parent activity Id")]
        public int ParentActivityID { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 5)]
        [Display(Name = "Activity description")]
        public string ActivityDescription {
            get
            {
                return _activityDescription;
            }
            set
            {
                _activityDescription = value.Trim();
            }
        }

        [Required]
        [Display(Name = "Activity start time")]
        public DateTime StartDateTime { get; set; }

        [Required]
        [Display(Name = "Activity end time")]
        public DateTime EndDateTime { get; set; }

        [NotMapped]
        public List<Activities> ChildActivities { get; set; }

        public Activities()
        {
            ChildActivities = new List<Activities>();
        }

        public static Activities Find(Activities activity, int activityId)
        {
            if (activity == null) return null;
            return activity.ActivityID == activityId ? activity : activity.ChildActivities.Select(child => Find(child, activityId)).FirstOrDefault(found => found != null);
        }

        public int CompareTo(object compare)
        {
            var next = compare as Activities;
            return Compare(ActivityDescription, next.ActivityDescription, StringComparison.Ordinal);
        }
    }
}