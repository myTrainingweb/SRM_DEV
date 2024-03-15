using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlusCP.Models;
using IP.Classess;
using CaptchaMvc.HtmlHelpers;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace PlusCP.Controllers
{
    [OutputCache(Duration = 0)]
    public class HomeController : Controller
    {
        Home oHome;
        cAuth oAuth;
        //cEmployee oEmp = null;
        public ActionResult Login()
        {
            oAuth = new cAuth();
            if (cCommon.IsSessionExpired())
                return View(oAuth);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Login(cAuth oAuth)
        {
            if (string.IsNullOrEmpty(oAuth.SigninId))
            {
                ViewBag.ErrorMessage = "Username is required ";
                return View("Login");
            }
            else if (string.IsNullOrEmpty(oAuth.Password))
            {
                ViewBag.ErrorMessage = "Password is required ";
                return View("Login");
            }

            else if (!this.IsCaptchaValid(""))
            {
                ViewBag.ErrorMessage = "Please, enter a valid captcha. ";
                return View("Login");
            }


             

            //ViewBag.ErrMessage = "Error: captcha is not valid.";



            bool isValidUser = oAuth.IsValidUser(oAuth.SigninId, oAuth.Password);

            if (oAuth.Permission == "0")
            {

                ViewBag.ErrorMessage = "User Does Not Have Right Permission";
                return View("Login");
            }
            else if (isValidUser)
            {
                Session["SigninId"] = oAuth.SigninId;
                Session["ProgramId"] = oAuth.DefaultProgram;
                Session["Program"] = oAuth.Program;
                Session["FirstName"] = oAuth.FirstName;
                Session["LastName"] = oAuth.LastName;
                Session["VendorList"] = oAuth.VendorList;
                Session["Username"] = oAuth.UserName;
                Session["FullName"] = oAuth.FirstName + " " + oAuth.LastName;
                Session["isAdmin"] = oAuth.isAdmin;
                Session["DefaultProgram"] = oAuth.DefaultProgram;
                Session["DefaultDB"] = oAuth.DefaultDB;
                Session["isCustomer"] = oAuth.isCustomer;
                //Session["CONN_TYPE"] =oAuth.DefaultDB;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = oAuth.Message;
                return View("Login");
            }
        }

        //Register Controller
        public ActionResult Register()
        {
            return View(new SignupViewModel());
        }

        [HttpPost]
        public ActionResult Register(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Generate and send verification email
                SendVerificationEmail(model.Email);

                // Redirect to a page indicating email verification is required
                return RedirectToAction("VerificationPending");
            }

            return View(model);
        }

        private void SendVerificationEmail(string email)
        {
            // This method sends a verification email to the provided email address
            // You should implement your email sending logic here
            string verificationCode = Guid.NewGuid().ToString();
            string verificationLink = Url.Action("VerifyEmail", "Signup", new { email = email, code = verificationCode }, Request.Url.Scheme);

            // Here, you would send an email containing the verificationLink to the user's email address using SMTP or any other email service.
            // For example:
            // MailMessage message = new MailMessage();
            // message.To.Add(email);
            // message.Subject = "Verify your email address";
            // message.Body = "Please click on the following link to verify your email address: " + verificationLink;
            // SmtpClient smtp = new SmtpClient();
            // smtp.Send(message);
        }

        public ActionResult VerificationPending()
        {
            // This view should indicate that the user's signup is successful but email verification is pending
            return View();
        }

        public ActionResult VerifyEmail(string email, string code)
        {
            // Here you would verify the code sent via email
            // If the code matches, you would update the user's status to verified in the database

            // For demonstration purposes, let's assume verification is successful and redirect to a success page
            return RedirectToAction("VerificationSuccess");
        }

        public ActionResult VerificationSuccess()
        {
            // This view should indicate that email verification is successful
            return View();
        }
        //


        public ActionResult Logout()
        {
            cCommon.SessionExpired();
            //this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //this.Response.Cache.SetNoStore();
            //return RedirectToAction("Login", "Home");
            return View();
        }

        public ActionResult Base()
        {
            if (cCommon.IsSessionExpired())
                return RedirectToAction("Login");
            else
                return View();
        }
        [Route("Main")]
        public ActionResult Index()
        {

            cAuth oAuth = new cAuth();
            if (cCommon.IsSessionExpired())
                return RedirectToAction("Login");
            else
            {
                 Home oHome = new Home();
                DataSet dsCon = oHome.GetConnections();

                DataTable dt = dsCon.Tables["CONN"];
                if (dt.Rows.Count > 0)
                {
                    dt.DefaultView.RowFilter = "IsDropDown = true";
                    ViewBag.Connections = cCommon.ToDropDownList(dt.DefaultView.ToTable(), "ConType", "ConText", Session["DefaultDB"].ToString(), "ConText");

                    Session["CONN_ACTIVE"] = BasicEncrypt.Instance.Encrypt(oHome.GetConnectionString(Session["DefaultDB"].ToString()));
                    Session["CONN_TYPE"] = Session["DefaultDB"];
                    foreach (DataRow row in dt.Rows)
                        Session["CONN_" + row["ConType"]] = row["ConValue"];
                }


                ViewBag.UserName = Session["FirstName"].ToString().Replace(Session["FirstName"].ToString().Substring(0, 1), Session["FirstName"].ToString().Substring(0, 1).ToUpper());

                ViewBag.isAdmin = Session["isAdmin"].ToString();
                if (Session["isAdmin"].ToString() == "True")
                {

                    ViewBag.ddlVendor = cCommon.ToDropDownList((DataTable)Session["VendorList"], "ID", "NAME", Session["DefaultProgram"].ToString(), "ID");
                }
                else
                {
                    ViewBag.ddlVendor = cCommon.ToDropDownList((DataTable)Session["VendorList"], "ProgramId", "Program", Session["DefaultProgram"].ToString(), "ProgramId");
                }
                return View();
            }
        }
        public ActionResult Error()
        {
            //string message
            //if (System.Web.HttpContext.Current.Session["ErrorMsgMain"] != null)
            //{
            //    message = System.Web.HttpContext.Current.Session["ErrorMsgMain"].ToString();
            //}

            //ViewBag.Message = message;
            return View();
        }
        public ActionResult PageNotFound()
        {
            //Response.StatusCode = 404;
            return View("PageNotFound");
        }
        public ActionResult ChangeDfltVendor(string ProgramId, string program)
        {
            if (cCommon.IsSessionExpired())
            {
                return RedirectToAction("Login");
            }
            else
            {
                cAuth oAuth = new cAuth();
                bool isUpdated = oAuth.UpdateDfltVendor(ProgramId);
                if (isUpdated)
                {
                    Session["ProgramId"] = ProgramId;
                    Session["DefaultProgram"] = ProgramId;
                }
                return RedirectToAction("Index");
            }
        }
        //public void ChangeDBConnection(string conType)
        //{
        //    //if (cCommon.IsSessionExpired())
        //    //{
        //    //    return RedirectToAction("Login");
        //    //}
        //    //else
        //    //{
        //        oHome = new Home();
        //       // cAuth oAuth = new cAuth();
        //        Session["CONN_ACTIVE"] = BasicEncrypt.Instance.Encrypt(oHome.GetConnectionString(conType));
        //        Session["CONN_TYPE"] = conType;
        //        //Session["ProgramId"] = ProgramId;
        //        //Session["Program"] = Program;

        //     //return RedirectToAction("Index");
        //    //}
        //}
        
        public ActionResult ChangeDBConnection(string conType)
        {
            if (cCommon.IsSessionExpired())
            {
                return RedirectToAction("Login");
            }
            else
            {
                oHome = new Home();
                cAuth oAuth = new cAuth();
                Session["CONN_ACTIVE"] = BasicEncrypt.Instance.Encrypt(oHome.GetConnectionString(conType));
                //  Session["CONN_TYPE"] = conType;
                bool isUpdated = oAuth.UpdateDfltCon(conType);

                if (isUpdated)
                {
                    Session["DefaultDB"] = conType;

                }
                return RedirectToAction("Index");
            }
        }
        public JsonResult GetMenus(string ProgramId, string program)
        {
            oHome = new Home();
            string[] parts = program.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            string programName = parts[0].Trim();
            Session["ProgramName"] = programName;
            string defaultSite = parts[1].Trim();
            Session["DefaultSite"] = defaultSite; //For Inbound Report Model Summary Method

            String isadmin = Session["isAdmin"].ToString();
            oHome.GetMenus(Session["SigninId"].ToString(), isadmin, programName);
            return Json(oHome.webMnu, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            if (cCommon.IsSessionExpired())
                return View("Login");
            else
                return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(Duration = 0)]

        public ActionResult ChangePassword(ChangePassword oChangePwd)
        {

            bool isCaptcha = this.IsCaptchaValid("");
            string oldPwd = oChangePwd.oldPwd;
            string newPwd = oChangePwd.newPwd;
            string confirmPwd = oChangePwd.confirmPwd;
            string msg = string.Empty;

            if (!isCaptcha)
            {
                ViewBag.ErrMessage = "Please, enter a valid captcha. ";
                return View("ChangePassword");
            }
            if (ModelState.IsValid)
            {               // @ViewBag.newPwd = newPwd;

                //bool isValidPwd = ValidatePassword(newPwd, out msg);
                //if (isValidPwd == false)
                //{
                //    ViewBag.ErrMessage = msg;
                //    return View("ChangePassword");
                //}

                //if (string.IsNullOrEmpty(oldPwd))
                //{
                //    ViewBag.ErrMessage = "Enter old password.";
                //    return View("ChangePassword");
                //}

                //else if (string.IsNullOrEmpty(newPwd))
                //{
                //    ViewBag.ErrMessage = "Enter new password.";
                //    return View("ChangePassword");
                //}

                //else if (string.IsNullOrEmpty(confirmPwd))
                //{
                //    ViewBag.ErrMessage = "Enter confirm password.";
                //    return View("ChangePassword");
                //}

                if (oldPwd == newPwd)
                {
                    ViewBag.ErrMessage = "Old and new password cannot be same.";
                    return View("ChangePassword");
                }

                else if (newPwd != confirmPwd)
                {
                    ViewBag.ErrMessage = "Password does not match.";
                    return View("ChangePassword");
                }

                
                else
                {
                    oHome = new Home();
                    oHome.ChangePassword(oldPwd, newPwd, confirmPwd);
                    ViewBag.ErrMessage = oHome.Message;
                    return View("ChangePassword");
                }

            }
            else
                return View();
        }

        public JsonResult GetExternalRequest(string signId)

        {
            ChangePassword oChangePwd = new ChangePassword();
            oChangePwd.GetChangeRequest(signId);
            return Json(oChangePwd.Message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeExternalPassword(string newPwd, string confirmPwd, string signId)
        {
            ChangePassword oChangePwd = new ChangePassword();
            oChangePwd.ChangeExternalPassword(newPwd, confirmPwd, signId);
            return Json(oChangePwd.Message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendMail(string SignInId, string requestedOn)
        {
            cAuth oAuth = new cAuth();
            oAuth.SendMail(SignInId, requestedOn);
            var jsonResult = Json(oAuth, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        private bool ValidatePassword(string password, out string ErrorMessage)
        {
            var input = password;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one lower case letter";
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one upper case letter";
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "Password should not be less than or greater than 12 characters";
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one numeric value";
                return false;
            }

            else if (!hasSymbols.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one special case characters";
                return false;
            }
            else
            {
                return true;
            }
        }
        [HttpPost]
        public ActionResult SaveLog(double gridExecutionTime) //Added by Huzaifa
        {
            cLog oLog = new cLog();
            Home oHome = new Home();
            string recNo = "";
            string rptUrl = "";
            DataTable dt = oHome.GetRecord();
            if(dt.Rows.Count > 0)
            {
                 recNo = dt.Rows[0][0].ToString();
                 rptUrl = dt.Rows[0][1].ToString();
            }
          
            TimeSpan duration = TimeSpan.FromSeconds(gridExecutionTime);
            string gridExecTime = $"{(int)duration.Minutes:00}:{duration.Seconds:00}";
            oLog.UpdateLog(gridExecTime, recNo, rptUrl);

            return new EmptyResult();
        }
    }
}