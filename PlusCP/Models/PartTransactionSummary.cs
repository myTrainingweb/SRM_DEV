using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace PlusCP.Models
{
    public class PartTransactionSummary
    {
        cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
        #region fields

        [Display(Name = "Contract:")]
        public string contract { get; set; }
        [Display(Name = "Part No.:")]
        public string partNo { get; set; }

        [Display(Name = "From:")]
        public string _fromDt = DateTime.Now.AddDays(-1).ToString(Format.DateOnly);
        public string fromDt { get { return _fromDt; } set { _fromDt = value; } }

        [Display(Name = "To:")]
        public string _toDt = DateTime.Now.ToString(Format.DateOnly);
        public string toDt { get { return _toDt; } set { _toDt = value; } }
        public string filterString { get; set; }
        public string ReportTitle { get; set; }
        public List<ArrayList> lstPartInventory { get; set; }
        public List<ArrayList> lstPartTransaction { get; set; }

        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }

        public string ErrorMessage { get; set; }

        #endregion
        #region Methods
        //public DataTable Contract(string program)
        //{
        //    cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
        //    //string query = string.Empty;
        //    //query = @"select 
        //    //            distinct CONTRACT
        //    //            from ifsapp.shop_ord  
        //    //            WHERE CONTRACT = '21456'";
        //    //DataTable dt = oDAL.GetData(query);

        //   // string sites = HttpContext.Current.Session["ProgramForSite"].ToString();

        //    string query = string.Empty;
        //    query = @"select ID AS programId
        //                     ,NAME AS programName
        //                     FROM pls.PROGRAM  
        //              WHERE SITE = '<site>'
        //              ORDER BY NAME ";
        //    query = query.Replace("<site>", program);
        //    DataTable dt = oDAL.GetData(query);
        //    return dt;
        //}

        public bool GetList(string programId,string partNo, string frmDate, string toDate)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;
            query = @"
-----Party Inventory By Location-----

          SELECT  PL.locationno,
        SUM(PQ.availableqty) AS QTY
FROM   pls.partqty PQ 
INNER JOIN pls.partno PN ON PQ.partno = PN.partno 
INNER JOIN pls.partlocation PL ON PL.id = PQ.locationid 
INNER JOIN pls.program P ON P.id = PQ.programid 
INNER JOIN [pls].[CodeConfiguration] CC ON CC.ID = PQ.ConfigurationID
INNER JOIN [pls].[User] U ON U.ID = PQ.UserID  
WHERE  PQ.availableqty > 0
       AND P.ID = '<program>'
       AND CONVERT(Date,PQ.CreateDate) >= '<frmDt>'
       AND CONVERT(Date,PQ.CreateDate) <= '<toDt>'
Group by PL.locationno
ORDER BY PL.locationno DESC

 ";
            query = query.Replace("<frmDt>", frmDate);
            query = query.Replace("<toDt>", toDate);
            query = query.Replace("<program>", programId);

            string sql = string.Empty;
            sql = @"
-----Part Transaction-----
SELECT P.ID, PT.PartNo AS partno,
       CPT.Description AS PartTransactionType,
        PT.Location AS FROM_LOCATION,
        PT.ToLocation AS TO_LOCATION,
        PT.Qty,
        U.Username,
         FORMAT( PT.createdate, 'yyyy.MM.dd') AS createdate
FROM   pls.PartTransaction PT
INNER JOIN pls.Program P ON P.ID = PT.ProgramID
INNER JOIN pls.[User] U ON U.ID = PT.UserID
INNER JOIN [pls].[CodePartTransaction] CPT ON CPT.ID = PT.PartTransactionID
LEFT OUTER JOIN pls.partno PN ON PN.partno = PT.partno
LEFT OUTER JOIN pls.CodePartType CT ON CT.Id = PN.PartTypeId 
      WHERE CONVERT(Date, PT.createdate) >= '<frmDt>' AND CONVERT(Date, PT.createdate) <= '<toDt>'
      AND PN.partno = '<partNo>'
ORDER BY PT.createdate DESC";

            sql = sql.Replace("<frmDt>", frmDate);
            sql = sql.Replace("<toDt>", toDate);
            sql = sql.Replace("<partNo>", partNo);


            DataTable dt = oDAL.GetData(query);
            DataTable dt1 = oDAL.GetData(sql);

            if (!string.IsNullOrEmpty(partNo))
                filterString = " From = '" + frmDate + "' To = '" + toDate + "' | Part No. Like '" + partNo + "' ";


            //For SQL Documentation
            //cLog oLog = new cLog();
            //oLog.AddSqlQuery("015", query + sql, string.Empty, false);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstPartInventory = cCommon.ConvertDtToArrayList(dt);
                    lstPartTransaction = cCommon.ConvertDtToArrayList(dt1);
                return true;

            }
        }
        #endregion
    }
}