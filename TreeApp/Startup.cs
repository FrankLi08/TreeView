using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TreeApp.Startup))]
namespace TreeApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
