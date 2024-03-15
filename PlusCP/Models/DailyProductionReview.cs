using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace PlusCP.Models
{
    public class DailyProductionReview
    {
        cDAL oDAL;
        #region Fields
        [Display(Name = "From:")]
        public string _fromDt = DateTime.Now.AddDays(-1).ToString(Format.DateOnly);
        public string fromDt { get { return _fromDt; } set { _fromDt = value; } }
        [Display(Name = "To:")]
        public string _toDt = DateTime.Now.ToString(Format.DateOnly);
        public string toDt { get { return _toDt; } set { _toDt = value; } }
        [Display(Name = "Workstation:")]
        public string Workstation { get; set; }
        [Display(Name = "Program:")]
        public string program { get; set; }
        public string ReportTitle { get; set; }
        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }
        public string filterString { get; set; }
        public string ErrorMessage { get; set; }
       
        
        public List<Hashtable> lstDailyProductionReview { get; set; }

        public List<object> lstMst = new List<object>();
        #endregion

        #region Methods 
        public bool GetList(string programId, string frmDt, string toDt, string Workstation)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;
                      query = @"SELECT WOH.ProgramID,
       WOSH.woheaderid,
       WOH.serialno,
       (SELECT CASE WHEN COUNT(PS.SerialNo) > 0 THEN 'Y' ELSE 'N' END
	   FROM pls.partserial PS
	   WHERE PS.SerialNo = woh.SerialNo AND PS.ProgramID = woh.ProgramID AND ps.PartNo = woh.PartNo ) HAS_SN,
       CASE WHEN WOSH.IsPass = 1 THEN 'Y' ELSE 'N' END AS Ispass,
       WOSH.iteration,
       WOSH.createdate,
       WOSH.lastactivitydate,
       CASE
         WHEN wsd.code IS NULL THEN cws.description
         ELSE wsd.description
       END AS Workstation,
       CASE 
          WHEN CWSD.Code IS NULL THEN CWOS.Description 
	      ELSE CWSD.Description 
	   END AS ToWorkStation,
        (
	    select CASE WOH.RepairTypeID WHEN 42 THEN '1' WHEN 71 THEN '3' ELSE MAX(pna.Value) END
        from pls.woline wl 
        LEFT JOIN pls.PartNoAttribute pna ON WOH.ProgramID = pna.ProgramID and wl.ComponentPartNo = pna.PartNo and pna.AttributeID = 149
        where wl.WOHeaderID = WOH.ID and wl.StatusID = 14
	   ) as RepairLevel,
       usr.Username AS UserName,
       U.username AS Technician,
       CONVERT(DATE, WOSH.lastactivitydate) AS [Day],
       Format(WOSH.lastactivitydate, 'hh:mm:ss tt') AS [Time],
       CASE
         WHEN ( Cast(WOSH.lastactivitydate AS TIME) >= '06:00:00' )
              AND ( Cast(WOSH.lastactivitydate AS TIME) <= '15:30:00' ) THEN 1
         ELSE 2
       END AS [Shift],
       Upper(woh.partno) AS Model,
       (SELECT value
        FROM   pls.partnoattribute
        WHERE  programid = WOH.programid
               AND partno = woh.partno
               AND attributeid = 278) AS Family,
       (SELECT value
        FROM   pls.partnoattribute
        WHERE  programid = WOH.programid
               AND partno = woh.partno
               AND attributeid = 279) AS Technology,
       Datepart(hour, WOSH.lastactivitydate) AS [Hour],
       Concat(WOSH.woheaderid, WOH.serialno) AS [RMA/SN]
FROM   pls.woheader WOH
       INNER JOIN pls.wostationhistory WOSH
               ON WOH.id = WOSH.woheaderid 
       LEFT JOIN pls.codeworkstation CWS
              ON CWS.id = WOSH.workstationid
                 AND cws.passfail = 1
       LEFT JOIN pls.codeworkstationcustomdescription wsd
              ON wsd.programid = WOH.programid
                 AND wsd.repairtypeid = WOH.repairtypeid
                 AND wsd.codeworkstationid = WOSH.workstationid

        LEFT JOIN pls.CodeWorkStation CWOS ON CWOS.ID = WOSH.toWorkStationID
	   LEFT JOIN pls.CodeWorkStationCustomDescription CWSD ON CWSD.ProgramID = WOH.ProgramID
				AND CWSD.RepairTypeID = WOH.RepairTypeID
				AND CWSD.CodeWorkStationID = WOSH.ToWorkStationID         


       INNER JOIN pls.[User] U ON U.ID = WOSH.UserID

       LEFT JOIN pls.WOStationHistory WSH ON WSH.WOHeaderID = WOSH.WOHeaderID 
	             AND WSH.ToWorkStationID = WOSH.WorkStationID  
                 AND wsh.LastActivityDate = WOSH.CreateDate 
	   LEFT JOIN pls.[User] Usr ON Usr.ID = WSH.UserID

      

WHERE CONVERT(Date, WOSH.LastActivityDate) >= '<frmDt>' AND CONVERT(Date, WOSH.LastActivityDate) <= '<toDt>'
       AND WOSH.ispass IS NOT NULL ";

                if (!string.IsNullOrEmpty(Workstation))
                query += "AND wsd.Description LIKE '%" + Workstation + "%' ";

            query += "AND WOH.ProgramID = '" + programId + "' ";

            query += " ORDER BY WOSH.LastActivityDate DESC";

            query = query.Replace("<frmDt>", frmDt);
            query = query.Replace("<toDt>", toDt);
            query = query.Replace("<programId>", programId);

            DataTable dt = oDAL.GetData(query);

         
            filterString += " > From = '" + frmDt + "' To = '" + toDt + "' ";
            if (!string.IsNullOrEmpty(Workstation))
                filterString += " | WorkStation Like '" + Workstation + "' "; 


            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("037", query, string.Empty, false);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstDailyProductionReview = cCommon.ConvertDtToHashTable(dt);
                return true;

            }
        }
        #endregion
    }
}