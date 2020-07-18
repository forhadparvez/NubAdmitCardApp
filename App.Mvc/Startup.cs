using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(App.Mvc.Startup))]
namespace App.Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
