using System;
using System.Collections.Generic;

namespace FinancialTracker_Svc.Models
{
    public class Household
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Greeting { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatorId { get; set; }
    }

    public class HouseholdsContainer
    {
        public List<Household> Households { get; set; }

        public HouseholdsContainer(List<Household> homes) {
            Households = homes;
        }
    }
}