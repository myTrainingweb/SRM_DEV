using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace PlusCP.Models
{
    public class PartInquiry
    {
        cDAL oDAL;
        #region Fields
        [Display(Name = "Part No.:")]
        public string partNo { get; set; }
        [Display(Name = "Description:")]
        public string description { get; set; }
        [Display(Name = "Manufacture Part No.:")]
        public string manufacturePartNo { get; set; }
        [Display(Name = "Model No.:")]
        public string modelNo { get; set; }
        [Display(Name = "Serialized:")]
        public string serialFlag { get; set; }
        [Display(Name = "Primary Commodity:")]
        public string primaryCommodity { get; set; }

        [Display(Name = "Secondary Commodity:")]
        public string secondaryCommodity { get; set; }
        [Display(Name = "Part Type:")]

        public string partType { get; set; }
        [Display(Name = "Status:")]
        public string status { get; set; }
        [Display(Name = "Created By:")]
        public string userName { get; set; }
        [Display(Name = "Created On:")]
        public string createdOn { get; set; }
        [Display(Name = "Last Activity On:")]
        public string lastActivityOn { get; set; }
        public string ReportTitle { get; set; }
        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }
        public string filterString { get; set; }
        public string ErrorMessage { get; set; }
        public List<Hashtable> lstPartInquiry { get; set; }
        // public List<Hashtable> lstPartAttribute { get; set; }
        public List<Hashtable> lstPartQty { get; set; }
        public List<Hashtable> lstPartTransaction { get; set; }

        public List<Hashtable> lstOpenRO { get; set; }
        public List<Hashtable> lstOpenSO { get; set; }

        public List<object> lstMst = new List<object>();
        #endregion
        #region Methods 
        public bool GetList(string partNo,string programId)
        {
            // oDAL = new cDAL("ACTIVE", "ST");
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;
            query = @"
SELECT 
      Distinct
	  P.NAME,
	  PQ.ProgramID,
	  PN.PartNo, 
      PN.Description, 
      PN.ManufacturePartNo, 
      PN.ModelNo, 
      CASE WHEN PN.SerialFlag = 1 THEN 'Y' ELSE 'N' END SerialFlag,
      CC.Description AS PrimaryCommodity, 
      SC.Description AS SecondaryCommodity,    
      CP.Description AS PartType, 
      CS.Description AS Status, 
      u.Username, 
      PN.CreateDate, 
      PN.LastActivityDate
FROM pls.PartNo PN
INNER JOIN Pls.CodeCommodity CC ON CC.ID = PN.PrimaryCommodityID
INNER JOIN Pls.CodeCommodity SC ON SC.ID = PN.SecondaryCommodityID
INNER JOIN pls.PartQty PQ ON PQ.PartNo = PN.PartNo
INNER JOIN pls.CodePartType CP ON CP.ID = PN.PartTypeID
INNER JOIN pls.CodeStatus CS ON CS.ID = PN.StatusID
INNER JOIN pls.[User] U ON U.ID = PN.UserID
INNER JOIN pls.Program P ON P.ID = PQ.ProgramID

";


            if (!string.IsNullOrEmpty(partNo))
            query += "WHERE PN.PartNo LIKE '%" + partNo + "%' ";

            if (!string.IsNullOrEmpty(programId))
                query += "AND P.ID = '" + programId + "' ";

            query += "order by PN.LastActivityDate DESC";

            DataTable dt = oDAL.GetData(query);

            if (!string.IsNullOrEmpty(partNo))
                filterString = "> Part No. Like '" + partNo + "' ";

            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("002", query, string.Empty, false);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstPartInquiry = cCommon.ConvertDtToHashTable(dt);
                return true;

            }
        }
        #endregion

        public bool GetDetail(string partNo, string programId, string locNo)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;

            #region Header

            query = @"SELECT
                      Distinct
                      P.ID,
                      P.NAME,
	                  PN.PartNo, 
                      PN.Description, 
                      PN.ManufacturePartNo, 
                      PN.ModelNo, 
                      CASE WHEN PN.SerialFlag = 1 THEN 'Y' ELSE 'N' END SerialFlag,
                      CC.Description AS PrimaryCommodity, 
                      SC.Description AS SecondaryCommodity,    
                      PN.SecondaryCommodityID, 
                      CP.Description AS PartType, 
                      CS.Description AS Status, 
                      u.Username, 
                      PN.CreateDate, 
                      PN.LastActivityDate
FROM pls.PartNo PN
INNER JOIN Pls.CodeCommodity CC ON CC.ID = PN.PrimaryCommodityID
INNER JOIN Pls.CodeCommodity SC ON SC.ID = PN.SecondaryCommodityID
INNER JOIN pls.PartQty PQ ON PQ.PartNo = PN.PartNo
INNER JOIN pls.CodePartType CP ON CP.ID = PN.PartTypeID
INNER JOIN pls.CodeStatus CS ON CS.ID = PN.StatusID
INNER JOIN pls.[User] U ON U.ID = PN.UserID
INNER JOIN pls.Program P ON P.ID = PQ.ProgramID
WHERE PN.PartNo = '<partNo>'
";
            if (!string.IsNullOrEmpty(programId))
                query += "AND P.ID = @PROGRAM";


            query = query.Replace("@PROGRAM", programId);

            query = query.Replace("<partNo>", partNo);

            filterString = "Part No. = '" + partNo + "' ";

            // oDAL = new cDAL(HttpContext.Current.Request["DB"]);

            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("002-1", query, "Header", false);
            DataTable dtHeader = oDAL.GetData(query);

            if (dtHeader.Rows.Count > 0)
            {
                DataRow dr = dtHeader.Rows[0];
                description = dr["Description"].ToString();
                manufacturePartNo = dr["ManufacturePartNo"].ToString();
                modelNo = dr["ModelNo"].ToString();
                serialFlag = dr["SerialFlag"].ToString();
                primaryCommodity = dr["PrimaryCommodity"].ToString();
                secondaryCommodity = dr["SecondaryCommodity"].ToString();
                partType = dr["PartType"].ToString();
                status = dr["Status"].ToString();
                description = dr["Description"].ToString();
                userName = dr["Username"].ToString();
                createdOn = dr["CreateDate"].ToString() == DBNull.Value.ToString() ? "" : Convert.ToDateTime(dr["CreateDate"]).ToString("yyyy.MM.dd HH:mm");
                lastActivityOn = dr["LastActivityDate"].ToString() == DBNull.Value.ToString() ? "" : Convert.ToDateTime(dr["LastActivityDate"]).ToString("yyyy.MM.dd HH:mm");

            }
            #endregion


            query = @"
---Part Qty---

SELECT  P.Name AS Program,
        CASE WHEN PN.SerialFlag = 0 THEN 'N' ELSE 'Y' END AS SerialFlag,
        PQ.PartNo,
        PQ.LocationId,
        PL.LocationNo, 
		PQ.PalletBoxNo AS PalletNo,
	    PQ.LotNo AS CartonNo, 	
        CC.Id AS ConfigId,
		CC.Description AS Configuration,
	    SUM(PQ.AvailableQty) AS AvailQty
FROM   pls.PartQty PQ
INNER JOIN pls.partno PN ON PQ.partno = PN.partno
INNER JOIN pls.Program P ON P.ID = PQ.ProgramID
INNER JOIN pls.PartLocation PL ON PL.ID = PQ.LocationID
INNER JOIN pls.CodeConfiguration CC ON CC.ID = PQ.ConfigurationID
WHERE PQ.PartNo = '<partNo>' AND P.ID = '<programId>' AND PQ.AvailableQty > 0 ";

            if (!string.IsNullOrEmpty(locNo))
                query += "AND PL.locationNo LIKE '%" + locNo + "%' ";

            query += @" GROUP BY P.ID, P.Name,SerialFlag,PQ.PartNo, PQ.LocationId, PL.LocationNo, PQ.LotNo, CC.Id, CC.Description, PQ.PalletBoxNo ;


---Part Transaction---

SELECT PT.OrderHeaderID,
       PT.PartNo,
       P.Name AS Program, 
       PT.ParentSerialNo, 
       PT.SerialNo, 
       PT.Qty, 
       PT.Source, 
       PT.Condition,
       PT.Configuration, 
       PT.Location, 
       PT.ToLocation,
       PT.Reason, 
       CT.Description AS PartType,
       PT.CustomerReference, 
       CPT.Description AS PartTransactionType,
	   PT.OrderType,
	   U.Username, 
       PT.CreateDate
FROM   pls.PartTransaction PT
INNER JOIN pls.Program P ON P.ID = PT.ProgramID
INNER JOIN pls.[User] U ON U.ID = PT.UserID
INNER JOIN [pls].[CodePartTransaction] CPT ON CPT.ID = PT.PartTransactionID
INNER JOIN pls.PartNo PN ON PN.PartNo = PT.PartNo
INNER JOIN pls.CodePartType CT ON CT.Id = PN.PartTypeId 
WHERE PT.PartNo = '<partNo>' AND P.ID = '<programId>'
";

            
            query += "AND PT.CreateDate >= (GetDate() - 90)" ;
            
            query += @"ORDER BY PT.createdate DESC;

            ---Open RO---

SELECT ROH.ID,
ROH.CustomerReference,
ROL.QtyToReceive,
ROL.QtyReceived,
CS.Description AS [Status],
ROL.CreateDate,
ROL.LastActivityDate 
FROM [Plus].[pls].ROHeader AS ROH
INNER JOIN [Plus].[pls].CodeStatus AS CS
ON ROH.StatusID =CS.ID
INNER JOIN [Plus].[pls].ROLine AS ROL
ON ROH.ID =ROL.ROHeaderID ANd CS.ID = ROL.StatusID
WHERE ROH.StatusID =7 AND ROL.PartNo ='<partNo>'
AND ROH.ProgramId = '<programId>'
ORDER BY ROH.LastActivityDate DESC;

---Open SO---
SELECT SOH.ID,
       SOH.customerreference,
       SOL.qtytoship,
       SOL.qtyreserved,
       CS.description       AS [Status],
       SOL.createdate,
       SOL.lastactivitydate
FROM   [Plus].[pls].soheader AS SOH
       INNER JOIN [Plus].[pls].codestatus AS CS
               ON SOH.statusid = CS.id
       INNER JOIN [Plus].[pls].soline AS SOL
               ON SOH.id = SOL.soheaderid
                  AND CS.id = SOL.statusid
WHERE  SOH.statusid = 7 AND SOL.partno = '<partNo>'
AND SOH.ProgramId = '<programId>'
ORDER BY SOH.LastActivityDate DESC;";



            query = query.Replace("<partNo>", partNo);
            query = query.Replace("<programId>", programId);

           // oDAL = new cDAL("ACTIVE");
            DataSet DS = oDAL.GetDataSet(query);

            //For SQL Documentation
            oLog.AddSqlQuery("002-2", query, "Part Inquiry Detail", false);

            if (!oDAL.HasErrors)
            {
                List<ArrayList> lstDtl = new List<ArrayList>();

                //lstDtl = cCommon.ConvertDtToArrayList(DS.Tables[0]);
                //lstPartAttribute = cCommon.ConvertDtToHashTable(DS.Tables[0]);


                lstDtl = cCommon.ConvertDtToArrayList(DS.Tables[0]);
                lstPartQty = cCommon.ConvertDtToHashTable(DS.Tables[0]);

                lstDtl = cCommon.ConvertDtToArrayList(DS.Tables[1]);
                lstPartTransaction = cCommon.ConvertDtToHashTable(DS.Tables[1]);

                lstDtl = cCommon.ConvertDtToArrayList(DS.Tables[2]);
                lstOpenRO = cCommon.ConvertDtToHashTable(DS.Tables[2]);

                lstDtl = cCommon.ConvertDtToArrayList(DS.Tables[3]);
                lstOpenSO = cCommon.ConvertDtToHashTable(DS.Tables[3]);



                return true;

            }
            else
            {
                return false;
            }

        }

    }
}