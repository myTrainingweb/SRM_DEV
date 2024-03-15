using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace PlusCP.Models
{
    public class Admin
    {
        public List<ArrayList> lstUsers { get; set; }
        public List<ArrayList> lstRights { get; set; }
        public string actionInfo { get; set; }
        public string ReportTitle { get; set; }
        public string filterString { get; set; }
        public string Rights { get; set; }
        public string Type { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string signIn_Id { get; set; }
        public string Password { get; set; }


        public bool GetUsers()
        {
            string sql = "";
            cDAL oDAL = new cDAL();
            sql = "SELECT FirstName, LastName, SignInName, Program FROM EP.SignIn ";
            DataTable dt = oDAL.GetData(sql);

            if (!oDAL.HasErrors)
            {
                if (dt.Rows.Count > 0)
                {
                    lstUsers = cCommon.ConvertDtToArrayList(dt);
                }
                return true;
            }
            else
                return false;
        }

        public bool GetrightsById(string RightId)
        {
            cDAL oDAL = new cDAL();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT id, '' AS CheckRights, Rights FROM EP.UserRights Where id =  '" + RightId + "'");


            oDAL = new cDAL(cDAL.ConnectionType.INIT);

            DataTable dtDatapart = oDAL.GetData(sb.ToString());

            if (!oDAL.HasErrors)
                return true;
            else
                return false;
        }

        public bool GetRights()
        {
            string sql = "";
            cDAL oDAL = new cDAL();
            sql = "SELECT '' AS CheckRights, Rights FROM EP.UserRights ";
            DataTable dt = oDAL.GetData(sql);

            if (!oDAL.HasErrors)
            {
                if (dt.Rows.Count > 0)
                {
                    lstRights = cCommon.ConvertDtToArrayList(dt);
                }
                return true;
            }
            else
                return false;
        }


        public bool InsertUser(string First_Name, string Last_Name, string Type, string signIn_Id, string Password)
        {
            string insertState = @"Insert Into Ep.User (First_Name, Last_Name, Type, signin_id, Password) Values ('"+First_Name+ "', '" + Last_Name + "','" + Type + "','" + signIn_Id + "','" + Password + "') ";

           cDAL oDAL = new cDAL(cDAL.ConnectionType.INIT);
            oDAL.AddQuery(insertState);
            if (oDAL.HasErrors)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}