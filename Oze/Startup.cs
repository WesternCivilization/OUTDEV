using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Oze.Startup))]
namespace Oze
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
