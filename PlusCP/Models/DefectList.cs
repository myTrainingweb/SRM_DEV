using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace PlusCP.Models
{
    public class DefectList
    {
        cDAL oDAL;
        #region Fields
        //[Display(Name = "Part No.:")]
        //public string partNo { get; set; }

        [Display(Name = "From:")]
        public string _fromDt = DateTime.Now.AddDays(-1).ToString(Format.DateOnly);
        public string fromDt { get { return _fromDt; } set { _fromDt = value; } }

        [Display(Name = "To:")]
        public string _toDt = DateTime.Now.ToString(Format.DateOnly);
        public string toDt { get { return _toDt; } set { _toDt = value; } }
        public string ReportTitle { get; set; }
        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }
        public string filterString { get; set; }
        public string ErrorMessage { get; set; }
        public List<Hashtable> lstDefectList { get; set; }

        public List<object> lstMst = new List<object>();
        #endregion
        #region Methods 
        public bool GetList(string frmDt, string toDt, string programId)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;
            query = @"
SELECT    
               woh.ProgramID
              ,prg.Name AS ProgramName
              , CASE WHEN wsd.Code IS NULL THEN cws.Description ELSE wsd.Description END AS WorkStationDesc
              , wsh.Iteration
              , woh.SerialNo
              , (SELECT CASE WHEN COUNT(PS.SerialNo) > 0 THEN 'Y' ELSE 'N' END
                FROM pls.partserial PS
                WHERE PS.SerialNo = woh.SerialNo AND PS.ProgramID = woh.ProgramID ) HAS_SN
              , woh.PartNo
              , woh.ID
              , woh.CustomerReference
              , cft.Code Repaircode   
              , cft.Description AS Reason
              , cf.Description as Fault
              , usr.Username AS CreatedBy
              , whf.CreateDate AS CreatedOn
        FROM pls.WOStationHistory wsh
        INNER JOIN pls.WOHeader woh ON
                   woh.ID = wsh.WOHeaderID
        INNER JOIN pls.Program prg ON
                   prg.ID = woh.ProgramId
        INNER JOIN pls.CodeWorkStation cws ON
                   cws.ID = wsh.WorkStationID
        LEFT JOIN pls.CodeWorkStationCustomDescription wsd ON
                   wsd.ProgramID = woh.ProgramID
                   AND wsd.RepairTypeID = woh.RepairTypeID
                   AND wsd.CodeWorkStationID = wsh.WorkStationID
        INNER JOIN pls.WOStationHistoryFailReasons whf ON
                   whf.WOStationHistoryId = wsh.ID
        LEFT JOIN pls.CodeFault cft ON
                   cft.ID = whf.FaultID
                   INNER JOIN pls.WOLine WOL ON WOL.WOHeaderID = WOH.ID and wol.ComponentPartNo = woh.PartNo
                   INNER JOIN pls.WOUnit WOU ON WOU.WOLineID = WOL.ID
                   INNER JOIN pls.WOUnitCodes WUC ON WUC.WOUnitID = WOU.ID
                   INNER JOIN pls.CodeFault CF ON CF.ID = WUC.FaultID

        LEFT JOIN pls.[User] usr ON usr.ID = whf.UserId
        WHERE wsh.IsPass = 0
--AND whf.CreateDate >= '<frmDt>' AND whf.CreateDate <= '<toDt>'
AND CONVERT(Date, whf.CreateDate) >= '<frmDt>' AND CONVERT(Date, whf.CreateDate) <= '<toDt>'
";


            query = query.Replace("<frmDt>", frmDt);
            query = query.Replace("<toDt>", toDt);

            query += "AND prg.ID = '" + programId + "' ";
            query += "ORDER BY whf.CreateDate DESC ";

            filterString = " > From = '" + frmDt + "' To = '" + toDt + "' ";

            DataTable dt = oDAL.GetData(query);



            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("019", query, string.Empty, false);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstDefectList = cCommon.ConvertDtToHashTable(dt);
                return true;

            }
        }
        #endregion
    }
}
