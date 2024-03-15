using IP.ActionFilters;
using PlusCP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlusCP.Controllers
{
    [OutputCache(Duration = 0)]
    [SessionTimeout]
    public class PartInquiryController : Controller
    {
        // GET: ListingReports/PartInquiry
        PartInquiry oPartInquiry = new PartInquiry();
        public ActionResult Option()
        {
            oPartInquiry = new PartInquiry();
            return View(oPartInquiry);
        }

        public ActionResult Index(string RptCode, string menuTitle)
        {
            oPartInquiry = new PartInquiry();
            TempData["ReportTitle"] = menuTitle;
            TempData["RptCode"] = RptCode;
            ViewBag.ReportTitle = menuTitle;
            return View(oPartInquiry);
        }

        public JsonResult GetList(string partNo)
        {
            string menuTitle = string.Empty;
            string RptCode;

            oPartInquiry = new PartInquiry();
            oPartInquiry.GetList(partNo,Session["ProgramId"].ToString());
            var jsonResult = Json(oPartInquiry, JsonRequestBehavior.AllowGet);
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

        public ActionResult Detail(string partNo, string locNo)
        {
            string _locNo = null;
            string pn = null;
            //if (string.IsNullOrEmpty(partNo))
            //    return View();


            try
            {
                oPartInquiry = new PartInquiry();
                if (!string.IsNullOrEmpty(partNo))
                {

                    Session["locNo"] = locNo;
                    Session["partNo"] = partNo;
                    //_locNo = Session["locNo"].ToString();
                    pn = Session["partNo"].ToString();
                }

                if (!string.IsNullOrEmpty(Session["partNo"].ToString()))
                {
                    //_locNo = Session["locNo"].ToString();
                    pn = Session["partNo"].ToString();
                }
                bool success = oPartInquiry.GetDetail(pn, Session["ProgramId"].ToString(), _locNo);
                ViewBag.PartNo = pn;
                if (success)
                    return View(oPartInquiry);
                else
                    return View(oPartInquiry);
            }
            catch (Exception e)
            {
                ViewBag.ErrMessage = e.Message;
                return View();
            }
        }
    }
}