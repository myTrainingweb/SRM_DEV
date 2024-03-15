using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace IP.Areas.ListingReports.Models
{
    public class WIP
    {
       
        #region Fields
        [Display(Name = "Part No.:")]
        public string partNo { get; set; }
        [Display(Name = "Customer Ref.:")]
        public string custRef { get; set; }

        public int totalIndex { get; set; }
        public string ReportTitle { get; set; }
        public string cust { get; set; }
        public string ProgramID { get; set; }
        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }
        public string filterString { get; set; }
        public string ErrorMessage { get; set; }
        public List<ArrayList> lstWip { get; set; }
        public List<Hashtable> lstWOUnit { get; set; }

        public List<ArrayList> lstDataColumn { get; set; }
        public string WOHNo { get; set; }
        public string WOId { get; set; }

        #endregion
        #region "Methods"
        public bool GetList(string programId)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;

            query = @"
DECLARE @SQL_QUERY AS NVARCHAR(MAX)
DECLARE @COLUMNS AS NVARCHAR(MAX)
SELECT @COLUMNS = COALESCE(@COLUMNS + ',', '') + QUOTENAME(WorkstationDescription)
FROM
(
    SELECT WorkStationDescription
	FROM 
	(
		SELECT  WO.ProgramID
				, WO.PartNo 
	           , PN.Description
	           , CASE WHEN WO.StatusID = 19 THEN -- FOR WIP
		CASE WHEN WSD.Code IS NULL THEN cws.Description ELSE WSD.CODE END
  ELSE -- FOR HOLD
		CS.Description
END AS WorkstationDescription
	           , count(SerialNo) SN
	    FROM pls.WOHeader WO
	    INNER JOIN pls.PartNo PN ON PN.PartNo = WO.PartNo
        INNER JOIN pls.CodeStatus cs ON cs.ID = WO.StatusID
	    INNER JOIN pls.CodeWorkStation cws ON cws.ID = WO.WorkStationId  
		LEFT  JOIN pls.CodeWorkStationCustomDescription WSD ON
				   WSD.ProgramID = WO.ProgramID 
				   AND WSD.RepairTypeID = WO.RepairTypeID
				   AND WSD.CodeWorkStationID = WO.WorkStationID
	    WHERE  WO.ProgramID = @ProgramId AND WO.StatusID IN (19, 28)
		GROUP BY WO.ProgramID, WO.PartNo, WSD.Code, cws.Description, PN.Description,cs.Description, WO.StatusID
	) TMP
	GROUP BY WorkStationDescription
    ORDER BY WorkStationDescription OFFSET 0 ROWS

) AS PIVOT_COLUMNS

SET @SQL_QUERY =
N'SELECT * FROM 
(
    SELECT ProgramID ,PartNo, Description, WorkStationDescription, SUM(SN) AS SN
	FROM 
	(
		SELECT  Wo.ProgramID 
				,WO.PartNo 
	           , PN.Description
	           , CASE WHEN WO.StatusID = 19 THEN -- FOR WIP
		CASE WHEN WSD.Code IS NULL THEN cws.Description ELSE WSD.CODE END
  ELSE -- FOR HOLD
		CS.Description
END AS WorkstationDescription
	           , count(SerialNo) SN
	    FROM pls.WOHeader WO
	    INNER JOIN pls.PartNo PN ON PN.PartNo = WO.PartNo
        INNER JOIN pls.CodeStatus cs ON cs.ID = WO.StatusID
	    INNER JOIN pls.CodeWorkStation cws ON cws.ID = WO.WorkStationId  
		LEFT  JOIN pls.CodeWorkStationCustomDescription WSD ON
				   WSD.ProgramID = WO.ProgramID 
				   AND WSD.RepairTypeID = WO.RepairTypeID
				   AND WSD.CodeWorkStationID = WO.WorkStationID 
	    WHERE  WO.ProgramId = @ProgramId AND WO.StatusID IN (19, 28)
		GROUP BY WO.ProgramId,WO.PartNo, WSD.Code, cws.Description, PN.Description,cs.Description, WO.StatusID
	) TMP
	GROUP BY ProgramID,PartNo, Description, WorkStationDescription        
  ) A
PIVOT
(
    SUM(SN)
    FOR WorkstationDescription
    IN('+ @COLUMNS +')
)
AS PIVOT_TABLE
 ORDER BY PartNo
    '
    EXEC sp_executesql @SQL_QUERY
";

            query = query.Replace("@ProgramId", programId);

            DataTable dt = oDAL.GetData(query);

            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("011", query, string.Empty, false);

            dt.Columns.Add("Total");
            //foreach (DataRow row in dt.Rows)
            //{
            //    decimal total = 0;
            //    for (int i = 2; i < dt.Columns.Count - 1; i++)
            //    {
            //        decimal value = 0;
            //        if (row[i] != DBNull.Value)
            //            value = Convert.ToDecimal(row[i]);

            //        total += value;
            //    }


            //    row["TOTAL"] = total;
            //}
            foreach (DataRow row in dt.Rows)
            {
                decimal total = 0;
                for (int i = 3; i < dt.Columns.Count - 1; i++)
                {
                    decimal value = 0;
                    if (row[i] != DBNull.Value)
                        value = Convert.ToDecimal(row[i]);

                    total += value;
                }


                row["TOTAL"] = total;
            }

            DataTable dtList = dt.Clone();
            DataTable dtColHeader = cCommon.GenerateTransposedTable(dtList);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstWip = cCommon.ConvertDtToArrayList(dt);
                lstDataColumn = cCommon.ConvertDtToArrayList(dtColHeader);
                return true;
            }
        }

        public bool GetWOUnit( string partNo, string workStation, string programId)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;
            query = @"SELECT WOH.SerialNo ,CustomerReference as WONo ";
            if (workStation == "HOLD")
                query += ", CASE WHEN WSD.Code IS NULL THEN cws.Description ELSE WSD.Description END Station ";
            else { query += ", null as Station "; }
                query +=", WOH.PartNo , WOH.id AS WOId,WOH.customerreference WOHNo " +
                "FROM pls.WOHeader WOH INNER JOIN pls.CodeWorkStation CWS ON CWS.ID = WOH.WorkStationID LEFT  JOIN pls.CodeWorkStationCustomDescription WSD ON "+
			   "WSD.ProgramID = WOH.ProgramID   AND WSD.RepairTypeID = WOH.RepairTypeID   AND WSD.CodeWorkStationID = WOH.WorkStationID "+
                "left join pls.partserial PS  ON WOH.id = PS.woheaderid and ps.serialno = WOH.SerialNo  WHERE WOH.ProgramId = '<ProgramId>' and  PS.ProgramID ='<ProgramId>'  ";
          
             if (!string.IsNullOrEmpty(partNo)) 
            { query += "AND WOH.PartNo = '<partNo>'"; }
                

            if (workStation == "HOLD")
                query += " AND WOH.StatusID IN (28)";
            else if (workStation == "Total")
                query += " AND WOH.StatusID IN (28,19)";
            else
                query += " AND WOH.StatusID IN (19)";


            if (!string.IsNullOrEmpty(workStation) && workStation != "Undefined" && workStation != "Total" && workStation != "HOLD")
            {
                if (workStation == "Close" || workStation == "Kitting" || workStation == "Scrap")
                    query += " AND CWS.Description = '<workStation>' AND ( WSD.Code = '<workStation>' OR WSD.Code IS NULL) ";
                else
                    query += "AND (CWS.Description = '<workStation>' OR WSD.Code = '<workStation>' ) ";

            }

            query = query.Replace("<partNo>", partNo); 
            query = query.Replace("<workStation>", workStation);
            query = query.Replace("<ProgramId>", programId);

            DataTable dt = oDAL.GetData(query);

            if (!oDAL.HasErrors)
            {
                if (dt.Rows.Count > 0)
                {
                    lstWOUnit = cCommon.ConvertDtToHashTable(dt);
                }
                return true;
            }
            return false;
        }
        #endregion
    }
}