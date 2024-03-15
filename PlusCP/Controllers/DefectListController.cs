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
    public class DefectListController : Controller
    {
        // GET: DefectList
        DefectList oDefectList = new DefectList();
        public ActionResult Option()
        {
            oDefectList = new DefectList();
            return View(oDefectList);
        }

        public ActionResult Index(string RptCode, string menuTitle)
        {
            oDefectList = new DefectList();
            TempData["ReportTitle"] = menuTitle;
            TempData["RptCode"] = RptCode;
            ViewBag.ReportTitle = menuTitle;
            return View(oDefectList);
        }
        public JsonResult GetList(string frmDt, string toDate)
        {
            string menuTitle = string.Empty;
            string RptCode;

            oDefectList = new DefectList();
            oDefectList.GetList(frmDt, toDate, Session["ProgramId"].ToString());
            var jsonResult = Json(oDefectList, JsonRequestBehavior.AllowGet);
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
    }
}