using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TreeApp.Models
{
    public class ActivitiesDbCtxt : DbContext
    {
        public DbSet<Activities> Activities { get; set; }
    }
}