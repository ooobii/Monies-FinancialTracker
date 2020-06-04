using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialTracker_Web.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int ParentHouseholdId { get; set; }


        public virtual Household ParentHousehold { get; set; }
    }
}