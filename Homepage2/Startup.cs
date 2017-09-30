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
        }
    }
}
