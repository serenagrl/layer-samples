using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Converters;

[assembly: OwinStartup(typeof(LeaveSample.UI.Web.SignalR.Startup))]

namespace LeaveSample.UI.Web.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Create JsonSerializer with StringEnumConverter.
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
        
            // Register the serializer.
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);


            app.MapSignalR();
        }
    }
}
