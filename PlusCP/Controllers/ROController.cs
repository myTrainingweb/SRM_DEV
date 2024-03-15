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
    public class ROController : Controller
    {
        // GET: ROH
        RO oROH = new RO();
        // GET: SupplyChain/ROH
        public ActionResult Option()
        {
            oROH = new RO();
            bool statusSuccess  = oROH.Status();
            //ViewBag.ddlStatus = cCommon.ToDropDown(statusSuccess, "Id", "Description", "All");
            if (statusSuccess)
                return View(oROH);
            else
                return View();
        }

      
        public ActionResult Index(string rptCode, string menuTitle, String Option,string type)
        {
            oROH = new RO();
            TempData["ReportTitle"] = menuTitle;
            TempData["RptCode"] = rptCode;
            ViewBag.ReportTitle = menuTitle;
            ViewBag.ReportTitle1 = type;
            ViewBag.Option = Option;
            //cLog oLog = new cLog();
            //oLog.SaveLog(menuTitle, Request.Url.PathAndQuery, rptCode);
            TempData.Keep();
            return View(oROH);
        }

        public ActionResult GetRODetail(string rptCode, string menuTitle, String Option, string type)
        {
            oROH = new RO();
            TempData["ReportTitle"] = menuTitle;
            TempData["RptCode"] = rptCode;
            ViewBag.ReportTitle = menuTitle;
            ViewBag.ReportTitle1 = type;
            ViewBag.Option = Option;

            return View(oROH);
        }
        public ActionResult ROWithNoWO(string rptCode, string menuTitle, String Option, string type)
        {
            oROH = new RO();
            TempData["ReportTitle"] = menuTitle;
            TempData["RptCode"] = rptCode;
            ViewBag.ReportTitle = menuTitle;
            ViewBag.ReportTitle1 = type;
            ViewBag.Option = Option;

            return View(oROH);
        }
        public ActionResult PreRegisteredUnit(string rptCode, string menuTitle, String Option, string type)
        {
            oROH = new RO();
            TempData["ReportTitle"] = menuTitle;
            TempData["RptCode"] = rptCode;
            ViewBag.ReportTitle = menuTitle;
            ViewBag.ReportTitle1 = type;
            ViewBag.Option = Option;

            return View(oROH);
        }
        public JsonResult GetPreRegisteredUnit(string frmDt, string toDt, bool isAllDate, string serialNo)
        {
            string menuTitle = string.Empty;
            string RptCode;

            string programId = Session["ProgramId"].ToString();
            string ProgramName = Session["ProgramName"].ToString();
            oROH = new RO();
            oROH.GetPreRegisteredUnit(frmDt, toDt, isAllDate, serialNo , programId, ProgramName);
            var jsonResult = Json(oROH, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetROWithNoWO(string ROHeaderId, string custRef, string status, string statusId, string type, string programId, string ProgramName)
        {
            string menuTitle = string.Empty;
            string RptCode;

            oROH = new RO();
            oROH.GetROWithNoWo(ROHeaderId, custRef, status, statusId, type, Session["ProgramId"].ToString(), ProgramName);
            var jsonResult = Json(oROH, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetRODtl(string ROHeaderId, string frmDt, string toDate, bool isAllDate, string custRef, string status, string statusId, string type, string programId)
        {
            string menuTitle = string.Empty;
            string RptCode;

            oROH = new RO();
            oROH.GetRODetail(ROHeaderId, frmDt, toDate, isAllDate, custRef, status, statusId, type, Session["ProgramId"].ToString());
            var jsonResult = Json(oROH, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetROH(string Id, string frmDt, string toDate, bool isAllDate, string custRef, string status, string statusId,string type)
        {
            string menuTitle = string.Empty;
            string RptCode;

            oROH = new RO();
            oROH.GetROH(Id, frmDt, toDate, isAllDate, custRef, Session["ProgramId"].ToString(), status, statusId,type);
            var jsonResult = Json(oROH, JsonRequestBehavior.AllowGet);
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

        public ActionResult Detail(string Id, string custRef)
        {
            string id = null;
            string CustRef = null;
            //if (string.IsNullOrEmpty(custRef))
            //    return View();
            try
            {
                oROH = new RO();
                if (!string.IsNullOrEmpty(custRef))
                {

                    Session["roId"] = Id;
                    Session["custRef"] = custRef;
                    id = Session["roId"].ToString();
                    CustRef = Session["custRef"].ToString();
                }

                if (!string.IsNullOrEmpty(Session["custRef"].ToString()))
                {
                    id = Session["roId"].ToString();
                    CustRef = Session["custRef"].ToString();
                }
                bool success = oROH.GetDetail(id, CustRef);
                ViewBag.CustRef = CustRef;
                if (success)
                    return View(oROH);
                else
                    return View(oROH);
            }
            catch (Exception e)
            {
                ViewBag.ErrMessage = e.Message;
                return View();
            }
        }

        public ActionResult ROUnit(string ROLineID, string statusId)
        {
            try
            {
                oROH = new RO();
                ViewBag.ReportTitle = "RO Unit";
                ViewBag.data = null;
                if (statusId != null)
                {
                    ViewBag.data = " > Received";
                }
                else
                {
                    ViewBag.data = " > Ordered";
                }

                bool success = oROH.GetROUnit(ROLineID, statusId);
                if (success)
                    return View(oROH);
                else
                    return View();
            }
            catch (Exception e) {
                ViewBag.ErrMessage = e.Message;
                return View();
            }

        }
    }
}