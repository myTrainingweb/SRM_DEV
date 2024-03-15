using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;

namespace PlusCP.Models
{
    public class RO
    {
        cDAL oDAL;
        #region Fields
        [Display(Name = "From:")]
        public string _fromDt = DateTime.Now.AddDays(-1).ToString(Format.DateOnly);
        public string fromDt { get { return _fromDt; } set { _fromDt = value; } }

        [Display(Name = "To:")]
        public string _toDt = DateTime.Now.ToString(Format.DateOnly);
        public string toDt { get { return _toDt; } set { _toDt = value; } }
        [Display(Name = "Serial No.:")]
        public string serialNo { get; set; }
        [Display(Name = "Customer Ref.:")]
        public string custRef { get; set; }
        [Display(Name = "Status:")]
        public string statusId { get; set; }
        [Display(Name = "Program:")]
        public string program_Id { get; set; }
        [Display(Name = "Third Party Ref.:")]
        public string thirdPartyReference { get; set; }
        [Display(Name = "Order Type:")]
        public string orderType { get; set; }
        [Display(Name = "Return Reason:")]
        public string returnReason { get; set; }
        [Display(Name = "Created On:")]
        public string createDate { get; set; }
        [Display(Name = "Last Activity On:")]
        public string lastActivityDate { get; set; }
        [Display(Name = "Program:")]
        public string program { get; set; }
        [Display(Name = "Status:")]
        public string description { get; set; }
        [Display(Name = "Way Bill:")]
        public string wayBill { get; set; }
        [Display(Name = "Address:")]
        public string address { get; set; }
        [Display(Name = "Created By:")]
        public string username { get; set; }
        public string filterString { get; set; }
        public string ReportTitle { get; set; }
        public List<ArrayList> lstStatus { get; set; }
        public List<Hashtable> lstROH { get; set; }
        public List<Hashtable> lstRODetail { get; set; }
        public List<Hashtable> lstPreRegistered { get; set; }

        public List<Hashtable> lstDetail { get; set; }
        public List<Hashtable> lstROUnit { get; set; }
        public List<Hashtable> lstROWithNoWO { get; set; }
        public List<Hashtable> lstROUnitAccessory { get; set; }
        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }
        public string ErrorMessage { get; set; }
        #endregion

        #region Method


        public bool Status()
        {
            oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;
            query = @"SELECT DISTINCT 
		                CS.ID,
		                CS.Description		
                    FROM pls.CodeStatus CS
                    Inner Join pls.ROHeader On CS.ID = StatusID
                    ORDER BY CS.Description ASC";
            DataTable dt = oDAL.GetData(query);
            lstStatus = cCommon.ConvertDtToArrayList(dt);
            if (!oDAL.HasErrors)
                return true;
            else
                return false;

            //return dt;
        }

        public bool GetROH(string Id, string frmDt, string toDt, bool isAllDate, string custRef, string programId, string status, string statusId, string type)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            // oDAL = new cDAL("ACTIVE", "ST");
            string query = string.Empty;
            query = @"
  SELECT ROH.ID, 
       ROH.CustomerReference, 
       P.Name AS Program,
	   CS.Description AS Status,
	   ROH.ThirdPartyReference, 
	   COT.Description AS OrderType, 
       CONCAT(CAD.Address1, ' ' ,CAD.Address2) AS Address,
       CAD.City,
       CAD.State,
       CAD.Country,
       CAD.Zip,
	   SUM(ROL.QtyToReceive) AS QtyToRcv,
       SUM(ROL.QtyReceived) AS RcvQty,
       SUM(ROL.QtyToReceive) - SUM(ROL.QtyReceived) AS Pending,
	   U.Username, 
       FORMAT(ROH.CreateDate, 'yyyy.MM.dd HH:mm') AS CreatedOn, 
	   FORMAT(ROH.LastActivityDate, 'yyyy.MM.dd HH:mm') AS LastActivityOn
FROM   pls.ROHeader ROH
INNER JOIN PLS.ROLine ROL ON ROL.ROHeaderID = ROH.ID
INNER JOIN pls.Program P ON P.ID = ROH.ProgramID
INNER JOIN pls.[User] U ON U.ID = ROH.UserID 
LEFT JOIN pls.CodeStatus CS ON CS.ID = ROH.StatusID
LEFT JOIN pls.CodeOrderType COT ON COT.ID = ROH.OrderTypeID
LEFT OUTER JOIN pls.CodeAddressDetails CAD ON CAD.AddressId = ROH.AddressID AND CAD.AddressType = 'ShipFrom'
WHERE 1=1";


            if (!string.IsNullOrEmpty(statusId) && type.Equals("NEW RO") || type.Equals("RESERVED RO") || type.Equals("Received Units RO") || type.Equals("PARTIALLYRECEIVED RO") || type.Equals("RECEIVED RO"))
            {
                query += " AND CS.ID  = '<ID>'";
                query += " AND P.ID ='" + programId + "' ";
                query = query.Replace("<ID>", statusId);
                query += @"
GROUP BY 
ROH.ID, 
ROH.CustomerReference, 
P.Name,
CS.Description,
ROH.ThirdPartyReference, 
COT.Description, 
CONCAT(CAD.Address1, ' ' ,CAD.Address2),
CAD.City,
CAD.State,
CAD.Country,
CAD.Zip,
U.Username,
FORMAT(ROH.CreateDate, 'yyyy.MM.dd HH:mm'), 
FORMAT(ROH.LastActivityDate, 'yyyy.MM.dd HH:mm')";

                query += "ORDER BY CreatedOn DESC";
                filterString = "Status = '" + status + "'";
            }

            else
            {
                if (isAllDate != true)
                {
                    query += "AND CONVERT(Date, ROH.LastActivityDate) >= '<frmDt>' AND CONVERT(Date, ROH.LastActivityDate) <= '<toDt>'  ";
                    query = query.Replace("<frmDt>", frmDt);
                    query = query.Replace("<toDt>", toDt);
                }
                //query += "WHERE CONVERT(Date, ROH.LastActivityDate) >= '<frmDt>' AND CONVERT(Date, ROH.LastActivityDate) <= '<toDt>'  ";
                query = query.Replace("<frmDt>", frmDt);
                query = query.Replace("<toDt>", toDt);
                if (!string.IsNullOrEmpty(custRef))
                    query += "AND ROH.CustomerReference LIKE '%" + custRef + "%'";

                if (!status.Equals("All"))
                    query += "AND CS.ID  IN (" + statusId + ") ";

                //if (!program.Equals("All"))
                query += "AND P.ID ='" + programId + "' ";
                query += @"
    GROUP BY 
    ROH.ID, 
    ROH.CustomerReference, 
    P.Name,
	CS.Description,
	ROH.ThirdPartyReference, 
	COT.Description, 
    CONCAT(CAD.Address1, ' ' ,CAD.Address2),
    CAD.City,
    CAD.State,
    CAD.Country,
    CAD.Zip,
    U.Username,
    FORMAT(ROH.CreateDate, 'yyyy.MM.dd HH:mm'), 
	FORMAT(ROH.LastActivityDate, 'yyyy.MM.dd HH:mm')";

                query += "ORDER BY CreatedOn DESC";
                if (isAllDate != true)
                {
                    filterString += " | From = '" + frmDt + "' To = '" + toDt + "'";
                }
                if (!string.IsNullOrEmpty(custRef))
                    filterString += " Customer Ref. Like '" + custRef + "' ";
                if (!string.IsNullOrEmpty(status))
                    filterString += "| Status = '" + status + "'";
            }
            DataTable dt = oDAL.GetData(query);

            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("001", query, string.Empty, false);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstROH = cCommon.ConvertDtToHashTable(dt);
                return true;

            }
        }
        public bool GetRODetail(string Id, string frmDt, string toDt, bool isAllDate, string custRef, string status, string statusId, string type, string programId)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            // oDAL = new cDAL("ACTIVE", "ST");
            string query = string.Empty;
            query = @"
  
SELECT  ROH.ID,
        P.ID AS ProgramId,
        P.Name AS Program,
        CustomerReference,
        ROL.PartNo,
        ROU.ParentSerialNo,
        ROU.SerialNo,
        (SELECT CASE WHEN COUNT(PS.SerialNo) > 0 THEN 'Y' ELSE 'N' END
        FROM pls.partserial PS
        WHERE PS.SerialNo = ROU.SerialNo AND PS.ProgramID = Roh.ProgramID ) HAS_SN,
        PL.LocationNo,
        CLG.Description,
        CC.Description AS Configuration,
        (Case 
			WHEN CC.Description  = 'Good'
				THEN 'YES'
			ELSE 'NO'
			END
		
		) AS SealedBox,
        ThirdPartyReference, 
        COT.Description AS OrderType,
        CS.Description AS Status,
        SUM(ROL.QtyToReceive) AS QtyToRcv,
        SUM(ROU.Quantity) AS RcvQty,
        CONCAT(CAD.Address1, ' ' ,CAD.Address2) AS Address,
        CAD.City,
        CAD.State,
        CAD.Country,
        CAD.Zip,
        U.Username,
        ROH.CreateDate AS CreatedOn,
        ROH.LastActivityDate AS LastActivityOn
FROM    pls.ROHeader ROH
INNER JOIN PLS.ROLine ROL ON ROL.ROHeaderID = ROH.ID
INNER JOIN PLS.Program P ON P.ID = ROH.ProgramID 
INNER JOIN pls.ROUnit ROU ON ROU.ROLineID = ROL.ID
INNER JOIN PLS.CodeStatus CS ON CS.ID = ROH.StatusID
INNER JOIN PLS.[User] U ON U.ID = ROH.UserID
LEFT OUTER JOIN pls.PartSerial PS ON PS.ROHeaderID = ROH.ID AND PS.PartNo = ROL.PartNo AND PS.SerialNo = ROU.SerialNo
LEFT OUTER JOIN pls.PartLocation PL ON PL.ID = PS.LocationID
LEFT OUTER JOIN pls.CodeLocationGroup CLG ON CLG.ID = PL.LocationGroupID
LEFT OUTER JOIN PLS.CodeAddressDetails CAD ON CAD.AddressID = ROH.AddressID AND CAD.AddressType = 'ShipFrom'
LEFT OUTER JOIN pls.CodeOrderType COT ON COT.ID = ROH.OrderTypeID
LEFT OUTER JOIN PLS.CodeConfiguration CC ON CC.ID = ROL.ConfigurationID
WHERE 1=1
 ";
            if (!string.IsNullOrEmpty(statusId) && type.Equals("NEW RO") || type.Equals("RESERVED RO") || type.Equals("Received Units RO") || type.Equals("PARTIALLYRECEIVED RO") || type.Equals("RECEIVED RO"))
            {

                query += " AND ROH.StatusID = '<ID>'";

                query += "AND P.ID = '" + programId + "' ";

                query = query.Replace("<ID>", statusId);
                query += @" 
GROUP BY 
ROH.ID, 
P.ID,
Roh.ProgramID,
P.Name ,
CustomerReference,
ROL.PartNo,
ROU.ParentSerialNo,
ROU.SerialNo,
HAS_SN,
PL.LocationNo,
CLG.Description,
CC.Description,
ThirdPartyReference, 
COT.Description, 
CS.Description,
CAD.City,
CAD.State,
CAD.Country,
CAD.Zip,
U.Username,
ROH.CreateDate ,
ROH.LastActivityDate,
CONCAT(CAD.Address1, ' ' ,CAD.Address2) ";

                query += "ORDER BY CreatedOn DESC";
                filterString = "Status = '" + status + "'";
            }

            else
            {
                //query += "WHERE CONVERT(Date, ROH.LastActivityDate) >= '<frmDt>' AND CONVERT(Date, ROH.LastActivityDate) <= '<toDt>'  ";
                if (isAllDate != true)
                {
                    query += "AND CONVERT(Date, ROH.LastActivityDate) >= '<frmDt>' AND CONVERT(Date, ROH.LastActivityDate) <= '<toDt>' ";
                    query = query.Replace("<frmDt>", frmDt);
                    query = query.Replace("<toDt>", toDt);
                }

                query = query.Replace("<frmDt>", frmDt);
                query = query.Replace("<toDt>", toDt);
                if (!string.IsNullOrEmpty(custRef))
                    query += "AND ROH.CustomerReference LIKE '%" + custRef + "%'";

                if (!status.Equals("All"))
                    query += "AND ROH.StatusID  IN (" + statusId + ") ";

                //if (!program.Equals("All"))
                //query += "AND P.ID ='" + programId + "' ";

                query += "AND P.ID = '" + programId + "' ";

                query += @" 
GROUP BY 
ROH.ID, 
P.ID,
Roh.ProgramID,
P.Name ,
CustomerReference,
ROL.PartNo,
ROU.ParentSerialNo,
ROU.SerialNo,
PL.LocationNo,
CLG.Description,
CC.Description,
ThirdPartyReference, 
COT.Description, 
CS.Description,
CAD.City,
CAD.State,
CAD.Country,
CAD.Zip,
U.Username,
ROH.CreateDate ,
ROH.LastActivityDate,
CONCAT(CAD.Address1, ' ' ,CAD.Address2) ";
                query += "ORDER BY CreatedOn DESC";

                if (isAllDate != true)
                {
                    filterString += " | From = '" + frmDt + "' To = '" + toDt + "'";
                }
                if (!string.IsNullOrEmpty(custRef))
                    filterString += "| Customer Ref. Like '" + custRef + "' ";
                if (!string.IsNullOrEmpty(status))
                    filterString += "| Status = '" + status + "'";
            }
            DataTable dt = oDAL.GetData(query);
            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("001", query, string.Empty, false);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstRODetail = cCommon.ConvertDtToHashTable(dt);
                return true;

            }
        }

        public bool GetDetail(string Id, string custRef)
        {
            string query = string.Empty;
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            #region Header

            query = @"
SELECT ROH.ThirdPartyReference, 
		CASE 
			WHEN ROHA.Value IS NULL THEN COT.Description
			ELSE CONCAT( ROHA.Value,' / ',COT.Description) 
		END AS OrderType, 
	   ROH.CreateDate, 
	   ROH.LastActivityDate,
	   P.Name AS Program,
	   CS.Description,
	   CONCAT(CAD.Address1, ' ' ,CAD.Address2) AS Address, 
	   U.Username
FROM   pls.ROHeader ROH
INNER JOIN pls.Program P ON P.ID = ROH.ProgramID
LEFT JOIN pls.CodeStatus CS ON CS.ID = ROH.StatusID
LEFT JOIN pls.CodeOrderType COT ON COT.ID = ROH.OrderTypeID
LEFT JOIN pls.CodeAddressDetails CAD ON CAD.AddressId = ROH.AddressID AND CAD.AddressType = 'ShipFrom'
INNER JOIN pls.[User] U ON U.ID = ROH.UserID
LEFT  JOIN pls.roheaderattribute ROHA ON   ROHA.ROHeaderID =ROH.id AND ROHA.AttributeID = 6
WHERE ROH.ID = '<Id>'
";
            query = query.Replace("<Id>", Id);
            filterString = "Customer Ref. = '" + custRef + "' ";

            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("001-1", query, "Header", false);

            DataTable dtHeader = oDAL.GetData(query);

            if (dtHeader.Rows.Count > 0)
            {
                DataRow dr = dtHeader.Rows[0];
                thirdPartyReference = dr["ThirdPartyReference"].ToString();
                orderType = dr["OrderType"].ToString();
                createDate = dr["CreateDate"].ToString() == DBNull.Value.ToString() ? "" : Convert.ToDateTime(dr["CreateDate"]).ToString("yyyy.MM.dd HH:mm");
                lastActivityDate = dr["LastActivityDate"].ToString() == DBNull.Value.ToString() ? "" : Convert.ToDateTime(dr["LastActivityDate"]).ToString("yyyy.MM.dd HH:mm");
                program = dr["Program"].ToString();
                description = dr["Description"].ToString();
                address = dr["Address"].ToString();
                username = dr["Username"].ToString();
            }
            #endregion

            query = @"
SELECT ROL.ROHeaderID,
       ROL.ID,
	   ROL.PartNo, 
	   CC.Description AS Configuration,
	   ROL.QtyToReceive, 
	   ROL.QtyReceived, 
	   CS.Description AS Status,
	   U.Username,
       FORMAT(ROL.CreateDate, 'yyyy.MM.dd HH:mm') AS CreatedOn, 
	   FORMAT(ROL.LastActivityDate, 'yyyy.MM.dd HH:mm') AS LastActivityOn
FROM   pls.ROLine ROL
LEFT JOIN pls.CodeConfiguration CC ON CC.ID = ROL.ConfigurationID 
LEFT JOIN pls.CodeStatus CS ON CS.ID = ROL.StatusID
INNER JOIN pls.[User] U ON U.ID = ROL.UserID
WHERE ROHeaderID = '<Id>' 
ORDER BY CreatedOn DESC
";

            query = query.Replace("<Id>", Id);
            //For SQL Documentation
            oLog.AddSqlQuery("001-2", query, "---RO Line---", false);
            DataTable dt = oDAL.GetData(query);

            if (!oDAL.HasErrors)
            {
                if (dt.Rows.Count > 0)
                {
                    lstDetail = cCommon.ConvertDtToHashTable(dt);
                    return true;
                }
            }
            return false;
        }

        public bool GetROUnit(string ROLineID, string statusId)
        {
            string query = string.Empty;
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);

            query = @"
SELECT ROU.ID, 
       ROL.PartNo,
       ROU.ParentSerialNo, 
	   (SELECT  CASE WHEN COUNT(SerialNo) > 0 THEN 'Y' ELSE 'N' END
        FROM pls.PartSerial
        WHERE SerialNo = ROU.SerialNo) AS HAS_SN,
       ROU.SerialNo, 
       ROU.Quantity, 
	   CS.Description As Status,
	   U.Username,
       FORMAT(ROU.CreateDate, 'yyyy.MM.dd HH:mm') AS CreatedOn, 
	   FORMAT(ROU.LastActivityDate, 'yyyy.MM.dd HH:mm') AS LastActivityOn
FROM pls.ROUnit ROU
INNER JOIN pls.CodeStatus CS ON CS.ID = ROU.StatusID
INNER JOIN pls.[User] U ON U.ID = ROU.UserID
INNER JOIN pls.ROLine ROL ON ROU.ROLineID = ROL.ID
INNER Join pls.ROHeader ROH ON ROH.ID = ROL.ROHeaderID
WHERE ROLineID = '<ROLineID>' AND CS.ID IN (6,9)
ORDER BY CreatedOn DESC
";

            query = query.Replace("<ROLineID>", ROLineID);


            //if (!string.IsNullOrEmpty(statusId))
            //{ 
            //    query += "AND CS.ID = '" + statusId + "' ";
            //    query += "ORDER BY LastActivityOn DESC";
            //}
            //else
            //{
            //    query += "ORDER BY LastActivityOn DESC";
            //}
            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("001-3", query, "---RO Unit---", false);
            DataTable dt = oDAL.GetData(query);

            if (!oDAL.HasErrors)
            {
                if (dt.Rows.Count > 0)
                {
                    lstROUnit = cCommon.ConvertDtToHashTable(dt);
                }
                return true;
            }
            return false;
        }
        public bool GetROWithNoWo(string Id, string custRef, string status, string statusId, string type, string programId, string ProgramName)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;
            query = @"
SELECT 
         PS.ProgramID
		,P.Name
		,ROH.ID
		,ROH.CustomerReference
		,PS.PartNo
       ,(SELECT  CASE WHEN COUNT(SerialNo) > 0 THEN 'Y' ELSE 'N' END
        FROM pls.PartSerial
        WHERE SerialNo = PS.SerialNo) AS HAS_SN
		,PS.SerialNo
		,CS.Description AS Status
		,U.Username
		,ROH.CreateDate AS Received_On
		,PS.LastActivityDate
FROM pls.PartSerial PS
INNER JOIN pls.ROHeader ROH ON ROH.ID = PS.ROHeaderID 
INNER JOIN pls.Program P ON P.ID = PS.ProgramID
INNER JOIN pls.CodeStatus CS ON CS.ID = PS.StatusID
INNER JOIN pls.[User] U ON U.ID = ROH.UserID
WHERE CS.ID IN (6,9) AND WOHeaderID IS NULL
";

            if (!string.IsNullOrEmpty(custRef))
                query += " AND ROH.CustomerReference LIKE '%" + custRef + "%'";

            //if (!status.Equals("All"))
            //    query += "AND CS.ID  IN (" + statusId + ") ";

            if (programId != "0" && programId != null)
            {
                query += "AND P.ID = '" + programId + "' ";
            }

            query += @"ORDER BY Received_On DESC";

            if (!string.IsNullOrEmpty(ProgramName))
                filterString += " Program = '" + ProgramName + "' ";


            //if (!string.IsNullOrEmpty(statusId))

            //    filterString += " Status '" + status + "' ";

            if (!string.IsNullOrEmpty(custRef))

                filterString += " > Customer Ref. Like '" + custRef + "' ";


            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("001", query, string.Empty, false);

            DataTable dt = oDAL.GetData(query);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstROWithNoWO = cCommon.ConvertDtToHashTable(dt);
                return true;

            }
        }
        public bool GetPreRegisteredUnit(string frmDt, string toDt, bool isAllDate, string serialNo, string programId, string ProgramName)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = string.Empty;
            query = @"
    SELECT ROH.ID, 
       ROH.CustomerReference, 
       ROH.ProgramID,
       P.Name AS Program,
	   CS.Description AS Status,
	   ROH.ThirdPartyReference, 
	   COT.Description AS OrderType, 
	   SUM(ROL.QtyToReceive) AS QtyToRcv,
       SUM(ROL.QtyReceived) AS QtyRcvd,
        SUM(ROL.QtyToReceive) - SUM(ROL.QtyReceived) AS Pending,
	   U.Username, 
       FORMAT(ROH.CreateDate, 'yyyy.MM.dd HH:mm') AS CreatedOn, 
	   FORMAT(ROH.LastActivityDate, 'yyyy.MM.dd HH:mm') AS LastActivityOn,
	   ROU.SerialNo,
        ROL.PartNo,
	   CC.Description,
        CASE WHEN ROU.PreAlert = 1 THEN 'Y'
		ELSE 'N' END AS PreAlert,
        (SELECT CASE WHEN COUNT(PS.SerialNo) > 0 THEN 'Y' ELSE 'N' END
        FROM pls.partserial PS
        WHERE PS.SerialNo = ROU.SerialNo AND PS.ProgramID = Roh.ProgramID ) HAS_SN
FROM   pls.ROHeader ROH
INNER JOIN PLS.ROLine ROL ON ROL.ROHeaderID = ROH.ID
INNER JOIN pls.ROUnit ROU ON ROU.ROLineID = ROL.ID
INNER JOIN pls.Program P ON P.ID = ROH.ProgramID
INNER JOIN pls.[User] U ON U.ID = ROH.UserID 
LEFT OUTER JOIN pls.CodeStatus CS ON CS.ID = ROU.StatusID
LEFT OUTER JOIN pls.CodeOrderType COT ON COT.ID = ROH.OrderTypeID
INNER JOIN pls.CodeConfiguration CC ON CC.Id = ROL.ConfigurationID
";
            if (programId != "0" && programId != null)
            {
                query += "WHERE ROH.ProgramID = '" + programId + "' ";
            }
            else
            {
                query += "WHERE ROH.ProgramID IN (" + HttpContext.Current.Session["ProgramForSite"].ToString() + ") ";
            }


            if (isAllDate != true)
            {
                query += "AND CONVERT(Date, ROH.LastActivityDate) >= '<frmDt>' AND CONVERT(Date, ROH.LastActivityDate) <= '<toDt>' ";
                query = query.Replace("<frmDt>", frmDt);
                query = query.Replace("<toDt>", toDt);
            }
            //query += "AND CONVERT(Date, ROH.LastActivityDate) >= '<frmDt>' AND CONVERT(Date, ROH.LastActivityDate) <= '<toDt>' ";
            query = query.Replace("<frmDt>", frmDt);
            query = query.Replace("<toDt>", toDt);

            if (!string.IsNullOrEmpty(serialNo))
                query += " AND ROU.SerialNo LIKE '%" + serialNo + "%'";

            query += @"
    GROUP BY 
    ROH.ID, 
    ROH.CustomerReference, 
    ROH.ProgramID,
    P.Name,
	CS.Description,
	ROH.ThirdPartyReference, 
	COT.Description, 
    U.Username,
    FORMAT(ROH.CreateDate, 'yyyy.MM.dd HH:mm'), 
	FORMAT(ROH.LastActivityDate, 'yyyy.MM.dd HH:mm'), ROU.SerialNo, ROL.PartNo, CC.Description,ROU.PreAlert

    ORDER BY CreatedOn DESC
";

            if (!string.IsNullOrEmpty(ProgramName))
                filterString += " > Program = '" + ProgramName + "' ";

            if (isAllDate != true)
            {
                filterString += " | From = '" + frmDt + "' To = '" + toDt + "'";
            }

            if (!string.IsNullOrEmpty(serialNo))

                filterString += " | Serial No. Like '" + serialNo + "' ";

            //For SQL Documentation
            cLog oLog = new cLog();
            oLog.AddSqlQuery("001", query, string.Empty, false);

            DataTable dt = oDAL.GetData(query);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstPreRegistered = cCommon.ConvertDtToHashTable(dt);
                return true;

            }
        }

        #endregion
    }
}