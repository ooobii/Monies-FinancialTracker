using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinancialTracker_Web.Startup))]
namespace FinancialTracker_Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
