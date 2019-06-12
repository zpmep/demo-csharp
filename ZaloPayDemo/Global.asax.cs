using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using ZaloPayDemo.DAL;
using ZaloPayDemo.ZaloPay;

namespace ZaloPayDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // DbConfiguration.SetConfiguration(new MySql.Data.EntityFramework.MySqlEFConfiguration());
            Database.SetInitializer(new CreateDatabaseIfNotExists<ZaloPayDemoContext>());
            NgrokHelper.Init().Wait();
        }
    }
}
