using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TreeApp.Models
{
    public class EditViewModel
    {
        public Activities Activity { get; set; }
        public SelectList Categories { get; set; }
        public int SelectedParentActivityId { get; set; }
    }
}