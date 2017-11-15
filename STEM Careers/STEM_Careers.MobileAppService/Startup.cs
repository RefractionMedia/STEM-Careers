using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(STEM_Careers.MobileAppService.Startup))]

namespace STEM_Careers.MobileAppService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}