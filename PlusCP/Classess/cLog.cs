using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

public class cLog
{
    cDAL oDal = new cDAL(cDAL.ConnectionType.INIT);
    string sql = string.Empty;

    public void SaveLog(string rptName, string rptUrl, string rptCode)
    {
        SaveMru(rptCode);
        rptUrl = rptUrl.Replace("'", "''");

        sql = "INSERT INTO zLogQuery (SigninId, SigninName, RemoteHost, RptCode, RptName, RptUrl, InsertOn, Origin, QueryExecTime, RowsFetched) ";
        sql += "VALUES (";
        sql += "'" + HttpContext.Current.Session["SigninId"].ToString() + "', ";
        sql += "'" + HttpContext.Current.Session["FirstName"].ToString() + "', ";
        sql += "NULL, ";
        sql += "'" + rptCode + "', ";
        sql += "'" + rptName + "', ";
        sql += "'" + rptUrl + "', ";
        sql += "Getdate(), ";
        sql += "'EP',";
        sql += "'" + cDAL.QueryExecTime + "', "; //Added by Huzaifa
        sql += "'" + cDAL.RowsFetched + "') "; //Added by Huzaifa
        oDal.Execute(sql);
    }
    public void UpdateLog(string gridExecTime, string recNo, string rptUrl) //Added by Huzaifa
    {
        cDAL oDal = new cDAL(cDAL.ConnectionType.INIT);
        string sql = string.Empty;

        sql = "UPDATE zLogQuery SET GridExecTime = '" + gridExecTime + "' " +
            "WHERE RECNUM = '" + recNo + "' " +
            "AND SigninId = '" + HttpContext.Current.Session["SigninId"].ToString() + "'" +
            "AND SigninName ='" + HttpContext.Current.Session["FirstName"].ToString() + "'" +
            "AND RptUrl = '" + rptUrl + "' ";

        oDal.Execute(sql);
    }

    public void RecordError(string errorMsg, string errorStack, string errorQry)
    {
        errorMsg = errorMsg.Replace("'", "");
        errorQry = errorQry.Replace("'", "");

        object empName = HttpContext.Current.Session["FirstName"];

        sql = "INSERT INTO zLogError (ErrMsg, ErrStack, ErrBy, ErrOn, ErrFrom, ErrQry) ";
        sql += "VALUES (";
        sql += "'" + errorMsg + "', ";
        sql += "'" + errorStack + "', ";
        sql += "'" + ((empName == null) ? "unknown" : empName) + "', ";
        sql += "Getdate(), ";
        sql += "'EP', ";
        sql += "\'" + errorQry + "\') ";
        oDal.Execute(sql);
    }

    public static void SendEmail(string sendTo, string subject, string body)
    {
        string senderEmail = "noreply@reconext.com";

        cDAL oDAL = new cDAL(cDAL.ConnectionType.INIT);
        
        string query = "INSERT INTO zLogEmail(SenderName, SenderEmail, SendTo, Cc, Bcc, Subject, Body, SentOn) ";
        query += "VALUES('<SenderName>', '<SenderEmail>', '<SendTo>', NULL, NULL, '<Subject>', '<Body>', <SentOn>)";

        query = query.Replace("<SenderName>", "PlusEP");
        query = query.Replace("<SenderEmail>", senderEmail);
        query = query.Replace("<SendTo>", sendTo);
        query = query.Replace("<Subject>", subject);
        query = query.Replace("<Body>", body);
        query = query.Replace("<SentOn>", "NULL");
        oDAL.Execute(query);
    }

    public static string GetEmailBody(List<string> lst)
    {
        StringBuilder sb = new StringBuilder();
        foreach (string item in lst)
        {
            sb.AppendLine(item);
        }
        return sb.ToString();
    }

    //internal void AddSqlQuery(string v1, string query, string empty, bool v2)
    //{
    //    throw new NotImplementedException();
    //}
    public void AddSqlQuery(string rptCode, string query, string headerText = "", bool isDetail = false)
    {
        query = query.Replace("'", "`");
        int empId = Convert.ToInt32(HttpContext.Current.Session["SigninId"]);
        if (isDetail == true)
        {
            sql = "DELETE FROM EP.SQLQuery WHERE EmpId = " + empId + " AND RptCode LIKE '" + rptCode + "%'";
            oDal.Execute(sql);
        }

        sql = "SELECT COUNT(*) FROM EP.SQLQuery WHERE EmpId = " + empId + " AND RptCode = '" + rptCode + "'";
        object obj = oDal.GetObject(sql);
        int count = Convert.ToInt32(obj);
        if (count == 0)
        {
            sql = "INSERT INTO EP.SQLQuery (RptCode, EmpId, HeaderText, Query, QueryOn) ";
            sql += "VALUES ('" + rptCode + "', " + empId + ", '" + (headerText == "" ? "" : headerText)
            + "', '" + query + "', GETDATE())";
        }
        else
        {
            sql = "UPDATE EP.SQLQuery Set HeaderText = '" + headerText + "', Query = '" + query + "', QueryOn = GETDATE() ";
            sql += "WHERE EmpId = " + empId + " AND RptCode = '" + rptCode + "'";
        }
        oDal.Execute(sql);
    }

    private void SaveMru(string rptCode)
    {
        int accessCount = 1;
        int empId = Convert.ToInt32(HttpContext.Current.Session["SigninId"]);
        string empName = HttpContext.Current.Session["FirstName"].ToString();

        sql = "SELECT AccessCount FROM zLogMru WHERE EmpId = " + empId + " AND RptCode = '" + rptCode + "' AND Origin = 'EP' ";
        object obj = oDal.GetObject(sql);

        if (obj == null)
        {
            sql = "INSERT INTO zLogMru (EmpId, EmpName, RptCode, AccessCount,Origin) ";
            sql += "VALUES (" + empId + ", '" + empName + "', '" + rptCode + "', " + accessCount + ",'EP')";
        }
        else
        {
            accessCount = Convert.ToInt32(obj) + 1;
            sql = "UPDATE zLogMru Set LogDt = GETDATE(), AccessCount = " + accessCount + " ";
            if (empId != 0)
                sql += "WHERE Origin = 'EP' AND EmpId = " + empId + " AND RptCode = '" + rptCode + "'";
            else
                sql += "WHERE Origin = 'EP' AND EmpName = '" + empName + "' AND RptCode = '" + rptCode + "'";
        } 

        oDal.Execute(sql);
    }
}
