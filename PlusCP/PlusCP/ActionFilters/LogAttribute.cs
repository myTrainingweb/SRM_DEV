using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlusCP.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller;
            var tempData = controller.TempData;

            string menuTitle = filterContext.HttpContext.Request.Headers["IP-menu-title"] as string;
            string rptCode = filterContext.HttpContext.Request.Headers["IP-menu-id"] as string;

            if (!string.IsNullOrEmpty(menuTitle) && !string.IsNullOrEmpty(rptCode))
            {
                cLog oLog = new cLog();
                oLog.SaveLog(menuTitle, filterContext.HttpContext.Request.Url.PathAndQuery, rptCode);
            }


            var ex = filterContext.Exception;
            if (ex != null)
            {
                controller = filterContext.Controller;
                string actionInfo = filterContext.RouteData.Values["action"].ToString() + " » " + controller.ControllerContext.RouteData.Values["controller"].ToString();
                cLog oLogs = new cLog();
                oLogs.RecordError(ex.Message, ex.StackTrace, actionInfo);
            }
        }
    }
}