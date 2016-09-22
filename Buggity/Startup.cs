using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Buggity.Startup))]
namespace Buggity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
