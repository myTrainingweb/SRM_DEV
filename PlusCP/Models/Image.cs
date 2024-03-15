using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;


namespace PlusCP.Models
{
    public class Image
    {

        #region fields
        
        public string ImageBase64String { get; set; }
        [Display(Name = "From:")]
        //public string _fromDt = DateTime.Now.AddDays(-1).ToString(Format.DateOnly);
        public string _fromDt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");

        public string fromDt { get { return _fromDt; } set { _fromDt = value; } }

        [Display(Name = "To:")]
        public string _toDt = DateTime.Now.ToString(Format.DateOnly);
        public string toDt { get { return _toDt; } set { _toDt = value; } }
        [Display(Name = "Serial No.:")]
        public string serialNo { get; set; }
        public List<Hashtable> lstImage { get; set; }
        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }
        public string ErrorMessage { get; set; }
        public string filterString { get; set; }
        public string ReportTitle { get; set; }
        cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
        cLog oLog = new cLog();
        #endregion
        #region methods
        public bool GetList(string serialNo, string fromDt, string toDt, bool checkAllDate)
        {
            string programId = HttpContext.Current.Session["ProgramId"].ToString();
            string programName = HttpContext.Current.Session["ProgramName"].ToString();
            string _serialNo = GetInValue(serialNo);

            string query = string.Empty;
            query = @"
SELECT  A.ID, 
		A.ProgramId,
        P.Name as programName,
		A.Name, 
		A.Picture, 
		A.OrderNo, 
		A.SerialNo, 
		A.PartNo, 
		A.CustomerReference, 
		A.TableName, 
		A.TableID, 
		A.ImageDataID, 
		U.Username, 
		A.CreateDate, 
		A.LastActivityDate 
FROM Plus.pls.Image A
INNER JOIN Plus.pls.Program P ON P.ID = A.ProgramId
INNER JOIN Plus.Pls.[User] U ON U.ID = A.UserID
WHERE A.ProgramId = <programId>
";

            if (!checkAllDate)
            {
                query += @" AND CONVERT(Date, A.CreateDate) >= '<frmDt>' AND CONVERT(Date, A.CreateDate) <= '<toDt>'";
            }
            if (!string.IsNullOrEmpty(serialNo))
                query += "AND A.[SerialNo] IN (" + _serialNo + ") ";

            query += @"
            Order By 
            A.CreateDate DESC
            ";
            query = query.Replace("<programId>", programId);
            query = query.Replace("<frmDt>", fromDt);
            query = query.Replace("<toDt>", toDt);

            DataTable dt = oDAL.GetData(query);

            if (!string.IsNullOrEmpty(programName))
                filterString += "> Program = '" + programName + "' ";
            if (!checkAllDate)
            {
                filterString += " | From = '" + fromDt + "' To = '" + toDt + "' ";
            }

            if (!string.IsNullOrEmpty(serialNo))
                filterString += " | Serial No. =  " + _serialNo + "";


            //For SQL Documentation
            
            oLog.AddSqlQuery("053", query, string.Empty, false);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstImage = cCommon.ConvertDtToHashTable(dt);
                return true;
            }
        }

        public bool GetImage(string imageDataID)
        {
            string query = string.Empty;
            query = @"SELECT 
                Picture, 
                FilePath 
                    FROM 
                [plusimage].plusimage.pls.imagedata 
                    WHERE 
                id = '<imageDataID>'
";
            query = query.Replace("<imageDataID>", imageDataID);

            DataTable dt = oDAL.GetData(query);
            if (dt.Rows.Count == 1)
            {
                byte[] imageBytes = dt.Rows[0]["Picture"] as byte[];
                if (imageBytes != null)
                {
                    ImageBase64String = Convert.ToBase64String(imageBytes);
                }
                else
                {
                    try
                    {
                        string filePath = dt.Rows[0]["FilePath"] as string;
                        using (new cImpersonate())
                        {
                            byte[] imageDataFromFilePath = File.ReadAllBytes(filePath);
                            ImageBase64String = Convert.ToBase64String(imageDataFromFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        oLog.RecordError(ex.Message, ex.StackTrace, "EP - Report: Image - Method: GetImage(string imageDataID)");
                    }

                }

            }

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                //if (dt.Rows.Count > 0)
                //    lstImage = cCommon.ConvertDtToHashTable(dt);
                return true;
            }
        }
        private string GetInValue(string Value)
        {
            string[] arr = Value.Split(',');
            string _arr = null;
            foreach (var item in arr)
            {
                if (_arr == null)
                {
                    _arr = "\'" + item.Trim() + "\'";
                }
                else
                {
                    _arr += "," + "\'" + item.Trim() + "\'";
                }

            }
            return _arr;
        }
        #endregion
    }
}