using IP.Classess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IP.ActionFilters
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (cCommon.IsSessionExpired())
            {
                filterContext.Result = new RedirectResult("~/Home/Logout");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}