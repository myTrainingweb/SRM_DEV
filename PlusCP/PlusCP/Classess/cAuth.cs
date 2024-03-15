using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Linq;
using System.Web;
using IP.Classess;
using System.Text.RegularExpressions;

namespace PlusCP.Models
{
    public class cAuth
    {
        cDAL oDAL;
        public string SigninId { get; set; }
        public string Password { get; set; }
        public string ProgramId { get; set; }
        public string Program { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string LastSignInDt { get; set; }
        public string Message { get; set; }
        public string EmailMessage { get; set; }
        public string UsrMenu { get; set; }
        public string isAdmin { get; set; }
        public int isActiveUser { get; set; }
        public string Permission { get; set; }
        public string isCustomer { get; set; }
        public string DefaultProgram { get; set; }
        public string DefaultDB { get; set; }

        public DataTable VendorList { get; set; }
        public DataTable Connection { get; set; }

        string Active;
        int LoginAttempt;
        DateTime lockTime;
        string result = "";
        DataTable dtUser = new DataTable();


        public bool IsValidUser(string username, string password)
        {

            cAuth.SetConnectionString();
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            DataTable dtUser = new DataTable();
            string sql = @"select PrimaryUse from [SRM].[UserInfo]  Where PrimaryUse = '<username>' ";
            sql = sql.Replace("<username>", username);
            dtUser = oDAL.GetData(sql);
            if(dtUser.Rows.Count == 0)
            {
                Message = "Error: Username is not valid";
                return false;
            }
           
            string UsrId = "";

             sql = @"select UserId, PrimaryUse, Password from [SRM].[UserInfo]  Where PrimaryUse = '<username>' AND Password = '<password>' ";
            sql = sql.Replace("<username>", username);
            sql = sql.Replace("<password>", password);
            dtUser = oDAL.GetData(sql);
            if(dtUser.Rows.Count == 0)
            {
                result = "Error: password is incorrect";
            }
            else
            {
                result = "OK";
            }
           

            sql = "SELECT UserId, isActive FROM [SRM].[UserInfo] WHERE PrimaryUse  ='" + username + "' ";
            DataTable dtUsr = oDAL.GetData(sql);
            if (dtUsr.Rows.Count > 0)
                UsrId = dtUsr.Rows[0]["UserId"].ToString();

            sql = "SELECT isActive, loginAttempt, LockTime FROM SRM.userInfo WHERE userId = '" + UsrId + "' ";
           
            DataTable dtUserInfo = oDAL.GetData(sql);
            if(dtUserInfo.Rows.Count > 0)
            {
                Active = dtUserInfo.Rows[0]["isActive"].ToString();
                LoginAttempt = Convert.ToInt32(dtUserInfo.Rows[0]["loginAttempt"]);
                if (dtUserInfo.Rows[0]["LockTime"].ToString() != "")
                {
                    lockTime = Convert.ToDateTime(dtUserInfo.Rows[0]["LockTime"]);
                }
               
            }
         
            int pendingTime = GetPendingTime(lockTime);
           
            if (result.Contains("password"))
            {
                if (Active == "N")
                {

                    if (pendingTime <= 0)
                    {
                        sql = "UPDATE SRM.UserInfo SET LoginAttempt = 0, LockTime = NULL, isActive = 'Y' Where UserId =" + UsrId;
                        oDAL.Execute(sql);
                        GetUerDtl(UsrId);
                        if (LoginAttempt <= 3)
                        {
                            if (dtUserInfo.Rows.Count > 0)
                            {
                                LoginAttempt = LoginAttempt + 1;
                                sql = "UPDATE SRM.UserInfo SET LoginAttempt = '" + LoginAttempt + "' Where UserId =" + UsrId;
                                oDAL.Execute(sql);
                                if (LoginAttempt >= 3)
                                {
                                    sql = "UPDATE SRM.UserInfo SET isActive = 'N', LockTime = '" + DateTime.Now.ToString() + "' WHERE UserId =" + UsrId;
                                    oDAL.Execute(sql);
                                }
                            }
                        }
                    }
                    
                    else
                    {
                        Message = "Your account has been locked for 15 minutes. " + " (" + pendingTime + " minutes left" + ") ";
                        return false;
                    }
                }
                else
                {
                    if (LoginAttempt <= 3)
                    {
                        if (dtUserInfo.Rows.Count > 0)
                        {

                            LoginAttempt = LoginAttempt + 1;
                            sql = "UPDATE SRM.UserInfo SET LoginAttempt = '" + LoginAttempt + "' Where UserId =" + UsrId;
                            oDAL.Execute(sql);
                            if (LoginAttempt >= 3)
                            {
                                sql = "UPDATE SRM.UserInfo SET isActive = 'N', LockTime = '" + DateTime.Now.ToString() + "' WHERE UserId =" + UsrId;
                                oDAL.Execute(sql);
                                Message = "Your account has been locked for 15 minutes.";
                                return false;
                            }
                        }
                    }
                }
                Message = result;
                return false;
            }
            // After 3 Attempt and time is over
            if (pendingTime <= 0)
            {
                if (result.Contains("OK"))
                {
                    var userId = Regex.Match(result, @"\d+").Value;
                    if (LoginAttempt <= 3)
                    {
                        sql = "UPDATE SRM.UserInfo SET LoginAttempt = 0, LockTime = NULL, isActive = 'Y' Where UserId =" + UsrId;
                        oDAL.Execute(sql);
                    }
                    sql = "SELECT FirstName, LastName FROM [SRM].[UserInfo]  WHERE UserId = '" + UsrId + "'";
                     dtUser = oDAL.GetData(sql);
                    if (dtUser.Rows.Count > 0)
                    {
                        //Updating User Password
                        sql = @"UPDATE SRM.UserInfo SET Password = '@Password' WHERE UserId = '" + UsrId + "' ";
                        sql = sql.Replace("@Password", password);
                        oDAL.Execute(sql);

                        SigninId = UsrId;
                        FirstName = dtUser.Rows[0]["FirstName"].ToString();
                        LastName = dtUser.Rows[0]["LastName"].ToString();
                        UserName = username;
                        string DefaultDtlquery = "SELECT IsAdmin, DefaultProgram, DefaultDB FROM SRM.UserInfo WHERE UserId = '" + UsrId + "'";
                        DataTable dtDefaultDtl = oDAL.GetData(DefaultDtlquery);
                        if (dtDefaultDtl.Rows.Count > 0)
                        {
                            isAdmin = dtDefaultDtl.Rows[0]["isAdmin"].ToString();
                            DefaultProgram = dtDefaultDtl.Rows[0]["DefaultProgram"].ToString();
                            DefaultDB = dtDefaultDtl.Rows[0]["DefaultDB"].ToString();
                        }

                        //sql = "EXEC [utl].[spUserLastLogOn] " + userId;
                        //oDAL.Execute(sql);

                        string query = @"SELECT isAdmin from [SRM].[UserInfo] where USERID = '" + SigninId + "'";
                        query += "order by isAdmin desc";
                        string query1 = @"SELECT IsCustomer from [SRM].[UserInfo] where USERID = '" + SigninId + "'";
                        //query1 += "order by IsCustomer desc";
                        DataTable dtIsAdmin = new DataTable();
                        DataTable dtIsCustomer = new DataTable();
                        dtIsAdmin = oDAL.GetData(query);
                        dtIsCustomer = oDAL.GetData(query1);
                        if (dtIsAdmin.Rows.Count > 0)
                            isAdmin = dtIsAdmin.Rows[0]["isAdmin"].ToString();
                        if (dtIsCustomer.Rows.Count > 0)
                            isCustomer = dtIsCustomer.Rows[0]["IsCustomer"].ToString();
                    }
                    if (isAdmin == "True")
                    {

                        oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
                        string vendorQuery = "SELECT ID, (Name + ' - ' + Site) AS NAME FROM SRM.Program ORDER BY Name";
                        DataTable dtVendor = oDAL.GetData(vendorQuery);
                        if (dtVendor.Rows.Count > 0)
                        {
                            ProgramId = dtVendor.Rows[0]["ID"].ToString();
                            Program = dtVendor.Rows[0]["Name"].ToString();

                            VendorList = new DataTable();
                            VendorList = dtVendor.DefaultView.ToTable(true, "ID", "Name").Copy();
                        }

                    }
                    else
                    {


                        string ProgramCount = "SELECT count(UserId) FROM [SRM].[UserProgramX] WHERE USERID =" + userId;
                        var dtVendor1 = oDAL.GetObject(ProgramCount);
                        string MenuCount = "SELECT count(UserId) FROM [SRM].[UserMnuX] WHERE USERID = cast('" + userId + "' as varchar)";
                        var dtVendor2 = oDAL.GetObject(MenuCount);
                        if (dtVendor1.Equals(0) || dtVendor2.Equals(0))
                        {
                            Permission = "0";
                        }
                        else
                        {
                            string vendorQuery = "SELECT ProgramId, Program FROM [SRM].[UserProgramX] WHERE USERID =" + userId;
                            DataTable dtVendor = oDAL.GetData(vendorQuery);
                            if (dtVendor.Rows.Count > 0)
                            {
                                ProgramId = dtVendor.Rows[0]["ProgramId"].ToString();
                                Program = dtVendor.Rows[0]["Program"].ToString();

                                VendorList = new DataTable();
                                VendorList = dtVendor.DefaultView.ToTable(true, "ProgramId", "Program").Copy();
                            }
                        }
                    }
                    return true;
                }
            }


            else
            {
                Message = "Your account has been locked for 15 minutes. " + " (" + pendingTime + " minutes left" + ") ";
                return false;
            }
          

            Message = result;
            return false;
        }

        public int GetPendingTime(DateTime lockTimes)
        {
            DateTime cdatetime = Convert.ToDateTime(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
            TimeSpan ts = cdatetime.Subtract(lockTimes);
            Int32 minuteslocked = Convert.ToInt32(ts.TotalMinutes);
            Int32 pendingminutes = 15 - minuteslocked;
            return pendingminutes;
        }
        public void GetUerDtl(string userId)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.INIT);
            string sql = "SELECT IsActive, loginAttempt, LockTime FROM SRM.userInfo WHERE userId = '" + userId + "' ";
            DataTable dtUserInfo = oDAL.GetData(sql);
            Active = dtUserInfo.Rows[0]["IsActive"].ToString();
            LoginAttempt = Convert.ToInt32(dtUserInfo.Rows[0]["loginAttempt"]);

        }
        //public bool ChkIsCust(string userId)
        //{
        //    cDAL oDAL = new cDAL(cDAL.ConnectionType.INIT);
        //    string sql = "SELECT IsCustomer FROM EP.userInfo WHERE userId = '" + userId + "' ";
        //    DataTable dtUserInfo = oDAL.GetData(sql);
        //    isCustomer = dtUserInfo.Rows[0]["IsCustomer"].ToString();

        //    //LoginAttempt = Convert.ToInt32(dtUserInfo.Rows[0]["loginAttempt"]);
        //    return true;
        //}

        public static void SetConnectionString()
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.INIT);
            DataTable dt = oDAL.GetData("SELECT * FROM SRM.zConStr");
            if (dt.Rows.Count > 0)
            {
                //dt.DefaultView.RowFilter = "ConType = 'TEST'";
                HttpContext.Current.Session["CONN_ACTIVE"] = BasicEncrypt.Instance.Encrypt(dt.DefaultView.ToTable().Rows[0]["ConValue"].ToString());
               
            }
        }

        public bool UpdateDfltVendor(string ProgramId)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.INIT);
            string sql = @"UPDATE EP.UserInfo
                           SET DefaultProgram = '" + ProgramId + "' " +
                           "WHERE UserId = '" + HttpContext.Current.Session["SigninId"] + "'";
            oDAL.Execute(sql);
            if (oDAL.HasErrors)
                return false;
            return true;
        }

        public bool UpdateDfltCon(string conType)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.INIT);
            string sql = @"UPDATE EP.UserInfo
                           SET DefaultDB = '" + conType + "' " +
                           "WHERE UserId = '" + HttpContext.Current.Session["SigninId"] + "'";
            oDAL.Execute(sql);
            if (oDAL.HasErrors)
                return false;
            return true;
        }

        public bool SendMail(string SignInId, string requestedOn) // string ProgramId, Commented
        {

            cDAL oDAL = new cDAL(cDAL.ConnectionType.PLUS_EXT);
            cAuth oAuth = new cAuth();
            string URL = "";
            string sql = @"SELECT * FROM dbo.[User] WHERE USERNAME = '" + SignInId + "' ";

            DataTable dt = oDAL.GetData(sql);
            if (dt.Rows.Count > 0)
            {
                cLookup oLookup = new cLookup();
                string emailAdddress = dt.Rows[0]["EMAIL"].ToString();
                string userNameFrst = dt.Rows[0]["FirstName"].ToString();
                string subject = oLookup.GetSysValue("EP_FORGOT_PWD_SUBJECT");
                string body = oLookup.GetSysValue("EP_FORGOT_PWD_BODY");
                string urlExpTime = oLookup.GetSysValue("EP_FORGOT_URL_EXP");
                string url = oLookup.GetSysValue("EP_CHANGE_PWD_URL");
                requestedOn = DateTime.Now.ToString();
                double urlexptime = Convert.ToDouble(urlExpTime);
                object expiresOn = Convert.ToDateTime(requestedOn).AddMinutes(urlexptime);
                URL = url + Convert.ToBase64String(Encoding.UTF8.GetBytes(SignInId));

                oDAL = new cDAL(cDAL.ConnectionType.INIT);
                sql = @"INSERT INTO EP.ExtUrls(RptUrl, SigninId, RequestOn, ExpiresOn) 
                        VALUES('" + URL + "', '" + SignInId + "', '" + requestedOn + "', '" + expiresOn + "')";
                oDAL.Execute(sql);

                body = body.Replace("@UserName", "" + userNameFrst + "");
                body = body.Replace("<link>", "" + URL + "");
                string toAddress = emailAdddress;
                cLog.SendEmail(toAddress, subject, body);
            }
            else
            {
                EmailMessage = "Username is not valid";
                return false;
            }
            EmailMessage = "Email sent.";
            return true;

        }

    }
}