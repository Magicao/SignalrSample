
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalrSample.Startup))]

namespace SignalrSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}