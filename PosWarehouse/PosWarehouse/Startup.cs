using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PosWarehouse.Startup))]
namespace PosWarehouse
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
