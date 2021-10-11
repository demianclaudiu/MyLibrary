using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyLibrary.Startup))]
namespace MyLibrary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
