﻿using System.Web;
using System.Web.Mvc;

namespace FinancialTracker_Svc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
