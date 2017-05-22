using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KaiutYoga.Startup))]
namespace KaiutYoga
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
