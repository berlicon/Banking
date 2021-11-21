using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Banking.Site.Startup))]
namespace Banking.Site
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
