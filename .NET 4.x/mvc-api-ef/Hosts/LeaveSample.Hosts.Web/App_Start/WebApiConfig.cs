using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace LeaveSample.Hosts.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            
            // Remove "Controller" suffix.
            // Taken from http://www.strathweb.com/2013/02/but-i-dont-want-to-call-web-api-controllers-controller/
            var suffix = typeof(DefaultHttpControllerSelector).GetField("ControllerSuffix", BindingFlags.Static | BindingFlags.Public);
            if (suffix != null) suffix.SetValue(null, string.Empty);

            config.Services.Replace(typeof(IHttpControllerTypeResolver),
               new HttpServiceTypeResolver());

            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}