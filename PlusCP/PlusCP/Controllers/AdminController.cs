using PlusCP.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlusCP.Controllers
{
    public class AdminController : Controller
    {
        string Connection = @"Data Source=E2db1;Initial Catalog=CWDEV;Persist Security Info=True;User ID=CWdbusr;Password=mandy;";
        Admin oAdmin;
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["LogInUser"] != null)
            {
                //oAdmin = new Admin();
                //oAdmin.GetUsers();
                return View();
            }
            else
            {
                RedirectResult Rr = new RedirectResult("~/Home/Login");
                return Rr;
            }
        }

        public ActionResult GetList()
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            oAdmin = new Admin();
            oAdmin.GetUsers();
            var jsonResult = Json(oAdmin, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetRights()
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            oAdmin = new Admin();
            oAdmin.GetRights();
            var jsonResult = Json(oAdmin, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult Logout()
        {
            Session["LoginUser"] = null;
            //return View();
            return RedirectToAction("Login", "Home");
        }


        public JsonResult GetrightsById(string RightId)
        {

            oAdmin = new Admin();
            oAdmin.GetrightsById(RightId);
            var jsonResult = Json(oAdmin.lstRights, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpPost]
        public ActionResult EditPartAttr(string First_Name, string Last_Name, string Type, string signIn_Id, string Password)
        {
            oAdmin = new Admin();
            bool retVal = oAdmin.InsertUser(First_Name, Last_Name, Type, signIn_Id, Password);
            return Json(retVal);
        }

    }
}