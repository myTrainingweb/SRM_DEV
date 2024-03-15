using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace PlusCP.Models
{
    public class SqlDocs
    {
        #region Variables
        cDAL oDAL;
        public IList<ArrayList> lstSqlBlog { get; set; }

        public List<string> lstQuery { get; set; }
        //  public string DisplayQuery { get; set; }

        #endregion


        #region Methods

        public bool AddBlog(string cmnts, string rptCode)
        {
            cmnts = cCommon.RemoveSymbols(cmnts);
            string userName = HttpContext.Current.Session["UserName"].ToString();
            string sql = string.Empty;
            sql = "INSERT INTO EP.SQLDoc (DocCmnts, RptCode, AddedBy, AddedOn) ";
            sql += "VALUES('" + cmnts + "', '" + rptCode + "', '" + userName + "', GETDATE())";

            oDAL = new cDAL();
            oDAL.AddQuery(sql);
            oDAL.Commit();

            if (!oDAL.HasErrors)
                return true;
            else
                return false;
        }

        public bool GetQuery(string rptCode, string rptTitle, string isDetail)
        {
            oDAL = new cDAL();
            int empId = Convert.ToInt32(HttpContext.Current.Session["SigninId"]);
            string sql = string.Empty;

            if (rptCode == "007-SN")
            {
                rptCode = rptCode.Replace("-SN", "");
                sql = "SELECT HeaderText, Query ";
                sql += "FROM EP.SQLQuery ";
                sql += "WHERE RptCode = '" + rptCode + "' AND EmpId = " + empId;
            }

            else if (isDetail.Equals("Y"))
            {
                rptCode = rptCode + '_';
                sql = "SELECT HeaderText, Query ";
                sql += "FROM EP.SQLQuery ";
                sql += "WHERE RptCode LIKE '" + rptCode + "%' AND EmpId = " + empId;
            }

            else
            {
                sql = "SELECT HeaderText, Query ";
                sql += "FROM EP.SQLQuery ";
                sql += "WHERE RptCode = '" + rptCode + "' AND EmpId = " + empId;
            }


            DataTable dt = oDAL.GetData(sql);
            if (!oDAL.HasErrors)
            {
                lstQuery = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string headerText = row["HeaderText"].ToString();
                    string queryText = row["Query"].ToString().Replace("`", "'");

                    StringBuilder sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(headerText))
                    {
                        sb.AppendLine("<h4> -- " + headerText + " -- </h4>");
                    }
                    sb.AppendLine(queryText);
                    lstQuery.Add(sb.ToString());
                }

                return true;
            }
            else
                return false;
        }

        public bool GetBlog(string rptCode)
        {
            string sql = string.Empty;
            sql = "SELECT DocCmnts, AddedBy, FORMAT(AddedOn, 'yyyy.MM.dd HH:mm') AS AddedOn ";
            sql += "FROM EP.SQLDoc ";
            sql += "WHERE RptCode = '" + rptCode + "'";
            sql += " ORDER BY DocId DESC";
            oDAL = new cDAL();
            DataTable dt = oDAL.GetData(sql);
            lstSqlBlog = cCommon.ConvertDtToArrayList(dt);
            if (!oDAL.HasErrors)
                return true;
            else
                return false;
        }
        #endregion
    }
}