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
    public class PartsInventoryController : Controller
    {
        // GET: PartsInventory
        PartsInventory oPartsInventory;
        public ActionResult Option()
        {
            oPartsInventory = new PartsInventory();
            return View(oPartsInventory);
        }

        public ActionResult Index(string RptCode, string menuTitle)
        {
            oPartsInventory = new PartsInventory();
            TempData["ReportTitle"] = menuTitle;
            TempData["RptCode"] = RptCode;
            ViewBag.ReportTitle = menuTitle;
            return View(oPartsInventory);
        }

        public JsonResult GetList(string partNo)
        {
            string menuTitle = string.Empty;
            string RptCode;

            oPartsInventory = new PartsInventory();
            oPartsInventory.GetList(partNo,  Session["ProgramId"].ToString());
            var jsonResult = Json(oPartsInventory, JsonRequestBehavior.AllowGet);
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