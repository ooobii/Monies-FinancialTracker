using FinancialTracker_Web.Models;
using FinancialTracker_Web.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace FinancialTracker_Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index() {
            var house = db.Users.Find(User.Identity.GetUserId()).Household;

            var viewModel = new HomeIndexViewModel() {
                Household = house
            };

            return View(viewModel);
        }
    }
}