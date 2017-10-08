using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Homepage2.Startup))]
namespace Homepage2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            GlobalConfiguration.Configuration.UseSqlServerStorage("Data Source=tcp:bobdylan.database.windows.net,1433;Initial Catalog=CalendarDB;Persist Security Info=False;User ID=daveyman123;Password=dJGTT901;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False");
        }

    }
}
