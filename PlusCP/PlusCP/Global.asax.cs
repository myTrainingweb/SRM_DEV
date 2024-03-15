using PlusCP.Models;
using IP.Classess;
using System;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Models;
using CaptchaMvc.Interface;
using CaptchaMvc.Infrastructure;
using System.Collections.Generic;
using System.Web;

namespace PlusCP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var catpch = (DefaultCaptchaManager)CaptchaUtils.CaptchaManager;
            catpch.CharactersFactory = () => "mycharacters";
            catpch.PlainCaptchaPairFactory=lenght=>
              {
                String randomtext=  RandomText.Generate(catpch.CharactersFactory(),lenght);
                  bool ignore = false;
                  return new KeyValuePair<string, ICaptchaValue>(Guid.NewGuid().ToString("N"),
                      new StringCaptchaValue(randomtext, randomtext,ignore)
                      );
              };
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        protected void Session_Start()
        {
            // Read Connection string from file
            string fileName = ConfigurationManager.AppSettings["Key"];
            Session["CONN_INIT"] = BasicEncrypt.Instance.Encrypt(System.IO.File.ReadAllLines(fileName)[0].ToString());
            Session["CONN_PLUS_EXT"] = BasicEncrypt.Instance.Encrypt(System.IO.File.ReadAllLines(fileName)[1].ToString());
            if (Request.QueryString["IsExternal"] != null)
            {
                cCommon.is_external_request = true;
                cAuth.SetConnectionString();
            }
        }
     
        protected void Session_End(object sender, EventArgs e)
        {

        }

        void Application_Error(object sender, EventArgs e)
        {
            cLog oLog = new cLog();
            object empName = HttpContext.Current.Session["FirstName"];
            string path = Request.Url.PathAndQuery;
            Exception ex = Server.GetLastError();
            string customErrorMessage = string.Empty;
            if (ex is HttpException && ((HttpException)ex).GetHttpCode() == 404)
            {
                Response.Redirect("/Home/PageNotFound");
                customErrorMessage = "Path Not Found 404 : " + path + " | ";
            }
            else
            {
                Response.Redirect("/Home/Error");
                //Response.Redirect("/Home/Error?message=" + ex.Message);
                //Response.Redirect("/Shared/Error?page=detailed");
            }
            
            if(empName != null) { 
            oLog.RecordError(customErrorMessage  + ex.Message, ex.StackTrace, string.Empty);
            }
            
            
            Server.ClearError();
        }
    }
}
