using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;

namespace LeaveSample.UI.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            var jqueryDef = new ScriptResourceDefinition()
            {
                Path = "~/Scripts/jquery-2.0.1.min.js",
                DebugPath = "~/Scripts/jquery-2.0.1.js",
                CdnPath = "http://code.jquery.com/jquery-2.0.1.min.js",
                CdnDebugPath = "http://code.jquery.com/jquery-2.0.1.js"
            };

            var jqueryUiDef = new ScriptResourceDefinition()
            {
                Path = "~/Scripts/jquery-ui-1.10.2.min.js",
                DebugPath = "~/Scripts/jquery-ui-1.10.2.js",
                CdnPath = "http://code.jquery.com/jquery-ui-1.10.3.min.js",
                CdnDebugPath = "http://code.jquery.com/jquery-ui-1.10.3.js"
            };

            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", null, jqueryDef);
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", null, jqueryUiDef);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}