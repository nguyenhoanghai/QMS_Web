using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;  
using System.Web.Mvc;

namespace QMS_Website.Cors
{
    public class AllowCrossSiteJsonAttribute :  ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "http://localhost:3000");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Credentials", "true");

            if (filterContext.HttpContext.Request.HttpMethod == "OPTIONS")
            {
                filterContext.HttpContext.Response.Flush();
            }

            base.OnActionExecuting(filterContext);
        }
    }
}