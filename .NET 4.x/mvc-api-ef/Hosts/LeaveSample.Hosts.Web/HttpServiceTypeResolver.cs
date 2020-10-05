using System;
using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace LeaveSample.Hosts.Web
{
    /// <summary>
    /// Custom Controller Type Resolver class to load only controllers from Services.Http projects
    /// Note: Solution taken and modified from 
    /// http://www.strathweb.com/2013/02/but-i-dont-want-to-call-web-api-controllers-controller/
    /// </summary>
    public class HttpServiceTypeResolver : DefaultHttpControllerTypeResolver
    {
        public HttpServiceTypeResolver(): base(IsControllerType)
        {}

        internal static bool IsControllerType(Type t)
        {
           if (t == null) throw new ArgumentNullException("t");
           
            bool result = t.IsClass && t.IsVisible && !t.IsAbstract && 
                            typeof(IHttpController).IsAssignableFrom(t) &&
                            t.Namespace.EndsWith(".Services.Http");

            Debug.WriteLineIf(result == true, t.Name + "=" + result.ToString());

            return result;
            
        }
    }
}
