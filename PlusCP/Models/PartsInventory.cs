using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace PlusCP.Models
{
    public class PartsInventory
    {
        cDAL oDAL;
        #region Fields
        [Display(Name = "Part No.:")]
        public string partNo { get; set; }

        public string filterString { get; set; }
        public string ReportTitle { get; set; }
        public List<Hashtable> lstPartsInventory { get; set; }
        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }

        public string ErrorMessage { get; set; }
        #endregion

        #region Method

        public bool GetList(string partNo, string programId)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;
            query = @"
SELECT P.ID, PQ.partno, 
       PN.description, 
       PQ.LocationId,
	   PL.locationno,
       PL.Bin,
       PL.Warehouse,
       CC.ID AS ConfigId,
	   CC.Description AS Configuration ,
       PQ.PalletBoxNo AS PalletNo,
	   PQ.LotNo AS CartonNo,
       CASE WHEN PN.SerialFlag = 0 THEN 'N' ELSE 'Y' END AS SerialFlag,
       PQ.availableqty,
       P.NAME,
	   U.Username,
	   PQ.CreateDate,
	   PQ.LastActivityDate
FROM   pls.partqty PQ 
INNER JOIN pls.partno PN ON PQ.partno = PN.partno 
INNER JOIN pls.partlocation PL ON PL.id = PQ.locationid 
INNER JOIN pls.program P ON P.id = PQ.programid 
LEFT JOIN [pls].[CodeConfiguration] CC ON CC.ID = PQ.ConfigurationID
INNER JOIN [pls].[User] U ON U.ID = PQ.UserID 
WHERE   PQ.availableqty > 0 
";

            if (!string.IsNullOrEmpty(partNo))
                query += "AND PQ.partno LIKE '%" + partNo + "%' ";
           
            
            if (!string.IsNullOrEmpty(programId))
                query += "AND P.ID = '" + programId + "' ";
            query += "ORDER BY PQ.LastActivityDate DESC";

            query = query.Replace("<partno>", partNo);
            if (!string.IsNullOrEmpty(partNo))
                filterString += " > Part No. Like '" + partNo + "' ";
            DataTable dt = oDAL.GetData(query);

            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("003", query, string.Empty, false);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstPartsInventory = cCommon.ConvertDtToHashTable(dt);
                return true;

            }

        }
        #endregion
    }
}