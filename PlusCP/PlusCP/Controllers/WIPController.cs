using IP.ActionFilters;
using IP.Areas.ListingReports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IP.Areas.ListingReports.Controllers
{
    [OutputCache(Duration = 0)]
    [SessionTimeout]
    public class WIPController : Controller
    {
        // GET: ListingReports/WIP
        WIP oWIP = new WIP();

        public ActionResult Option()
        {
            oWIP = new WIP();
            return View(oWIP);
        }

        public ActionResult GetWIP(string RptCode)
        {
            string menuTitle = string.Empty;
            //string RptCode;
            try
            {
                WIP oWIP = new WIP();
                bool success = oWIP.GetList(Session["ProgramId"].ToString());
                ViewBag.ReportTitle = "WIP";
                ViewBag.ProgramID = Session["ProgramId"].ToString();
                //LOAD MRU & LOG QUERY
                if (ViewBag.ReportTitle != null && RptCode != null)
                {
                    menuTitle = ViewBag.ReportTitle;
                    cLog oLog = new cLog();
                    oLog.SaveLog(menuTitle, Request.Url.PathAndQuery, RptCode);
                }
                //if (!string.IsNullOrEmpty(custRef))
                //{
                //    ViewBag.ReportTitle += " > Customer Ref. Like '" + custRef + "'";
                //}
                //if (!string.IsNullOrEmpty(partNo))
                //{
                //    if (!string.IsNullOrEmpty(custRef))
                //        ViewBag.ReportTitle += " | Part No. Like '" + partNo + "'";
                //    else
                //        ViewBag.ReportTitle += " > Part No. Like '" + partNo + "'";
                //}
                if (success)
                    return View(oWIP);
                else
                    return View();
            }
            catch (Exception e)
            {
                ViewBag.ErrMessage = e.Message;
                return View();
            }

        }

        public ActionResult GetUnits(string partNo, string workStation)
        {
            try
            {
                ViewBag.cust = Session["isCustomer"].ToString();
                ViewBag.ProgramID = Session["ProgramId"].ToString();
                ViewBag.ReportTitle = "SN List @ " + workStation + "";
                WIP oWIP = new WIP();
                bool success = oWIP.GetWOUnit(partNo, workStation, Session["ProgramId"].ToString());
                oWIP.serializer = new System.Web.Script.Serialization.JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };

                if (success)
                    return View(oWIP);
                else
                    return View();
            }
            catch (Exception e)
            {
                ViewBag.ErrMessage = e.Message;
                return View();
            }
        }

    }
}