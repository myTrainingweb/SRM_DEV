using PlusCP.Extensions;
using IP.Classess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace PlusCP.Models
{
    public class ChangePassword
    {
        public string Message { get; set; }
        [Required]
        [Display(Name = "Old Password")]
        public string oldPwd { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Your password must be at least 1 number, special character, upper case, lower case ")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string newPwd { get; set; }
        [Required]
        [Display(Name = "Confirm Password")]
        public string confirmPwd { get; set; }

        public bool GetChangeRequest(string signId)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.INIT);
            string sql = @"SELECT TOP 1  DATEDIFF(MINUTE,GETDATE(), ExpiresOn) AS ExpiresMin 
                           FROM EP.exturls 
                           WHERE  SigninId ='" + signId + "' " +
                           "ORDER BY Recnum DESC ";

            int expirexMin = Convert.ToInt32(oDAL.GetObject(sql).ToString());
            //int expirexMin = 30;
            if (expirexMin > 0)
            {
                Message = "Success";
                return true;
            }
            else
            {
                Message = "Expired";
                return false;
            }

        }

        public bool ChangeExternalPassword(string newPwd, string confirmPwd, string signId)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.PLUS_EXT);
            string sql;
           // string id;
           // string sql = "SELECT ID FROM dbo.[User] WHERE USERNAME = '" + signId + "' ";
           // id = oDAL.GetObject(sql).ToString();

           //  sql = @"SELECT passwordHash FROM dbo.[UserPassword] WHERE UserId ='" + id + "' ";
           // DataTable dt = oDAL.GetData(sql);
           //string pwdHash = dt.Rows[0]["passwordHash"].ToString();
           // byte[] binaryData = System.Text.Encoding.ASCII.GetBytes(pwdHash);

           // string oldPWD = Encoding.ASCII.GetString(binaryData);

         
            if (newPwd != confirmPwd)
            {
                Message = "New and Confirm Must be same.";
                return false;
            }


            sql = @"EXEC utl.spUserResetPassword '" + signId + "','" + newPwd + "','PRS','' ";
             oDAL.Execute(sql);
            if (oDAL.HasErrors)
            {
                Message = "Password not changed.";
                return false;
            }
            else
            {
                Message = "Password has been reset.";
                return true;
            }

            

        }
    }
}
