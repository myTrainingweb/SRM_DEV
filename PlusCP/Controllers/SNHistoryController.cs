using IP.ActionFilters;
using PlusCP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlusCP.Controllers
{
    [OutputCache(Duration = 0)]
    [SessionTimeout]
    public class SNHistoryController : Controller
    {
        // GET: ListingReports/PartsSerial
        SNHistory oSNHistory = new SNHistory();

        public ActionResult Option()
        {
            oSNHistory = new SNHistory();
            return View(oSNHistory);
        }
        public ActionResult Index(string RptCode, string menuTitle)
        {
            oSNHistory = new SNHistory();
            if (menuTitle != null)
            {
                ViewBag.ReportTitle = menuTitle;
                TempData["ReportTitle"] = menuTitle;
                TempData["RptCode"] = RptCode;
                ViewBag.RptCode = RptCode;
            }
            else
            {
                ViewBag.ReportTitle = "Serial Number History";
                TempData["ReportTitle"] = "Serial Number History ";
                TempData.Keep("ReportTitle");
                ViewBag.RptCode = RptCode;
            }
            ViewBag.list = menuTitle;
            return View(oSNHistory);
        }
        public JsonResult GetList(string frmDt, string toDate, string serialNo, string partNo, string locNo, string palletNo, string config = "", string originFrom="")
        {
            string menuTitle = string.Empty;
            string RptCode;

            oSNHistory = new SNHistory();
            oSNHistory.GetList(frmDt, toDate, serialNo, Session["ProgramId"].ToString(), partNo, locNo, palletNo, config, originFrom);
            var jsonResult = Json(oSNHistory, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            //LOAD MRU & LOG QUERY

            if (TempData["ReportTitle"] != null && TempData["RptCode"] != null)
            {
                menuTitle = TempData["ReportTitle"] as string;
                RptCode = TempData["RptCode"].ToString();
                TempData.Keep();
                cLog oLog = new cLog();
                oLog.SaveLog(menuTitle, Request.Url.PathAndQuery, RptCode);
            }
            return jsonResult;
        }
        public ActionResult Detail(string serialNo, string partNo)
        {
            string sn = null;
            string pn = null;
            //if (serialNo == null)
            //{
            //    //  Session["serialNo"] = serialNo;
            //    // Session["partNo"] = partNo;
            //    sn = Session["serialNo"].ToString();
            //    pn = Session["partNo"].ToString();
            //}
            //if (serialNo != null)
            //{
            //   Session["serialNo"] = serialNo;
            //   Session["partNo"] = partNo;
            //}


            //if (string.IsNullOrEmpty(serialNo))
            //    return View();

            try
            {
                oSNHistory = new SNHistory();
                if (!string.IsNullOrEmpty(serialNo)) { 
               
                    Session["serialNo"] = serialNo;
                    Session["partNo"] = partNo;
                    sn = Session["serialNo"].ToString();
                    pn = Session["partNo"].ToString();
                }

                if (!string.IsNullOrEmpty(Session["serialNo"].ToString()))
                {
                     sn = Session["serialNo"].ToString();
                     pn = Session["partNo"].ToString();
                }
                bool success = oSNHistory.GetDetail(sn, pn, Session["ProgramId"].ToString());
                    ViewBag.SerialNo = sn;
                if (success)
                { 
                        return View(oSNHistory);
                }
                else
                { 
                        return View(oSNHistory);
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrMessage = e.Message;
                return View();
            }           
        }
    }
}