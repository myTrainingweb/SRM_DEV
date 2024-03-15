using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;


public class cCommon
{
    public static int minDate = 90;

    public static bool is_external_request { get; set; }

    public static string GeteDocsFolder()
    {
        string localDrive = string.Empty;
        if (System.IO.Directory.Exists("C:\\" + "eDocs"))
        {
            localDrive = "C:\\";
        }
        else if (System.IO.Directory.Exists("D:\\" + "eDocs"))
        {
            localDrive = "D:\\";
        }
        else
        {
            localDrive = string.Empty;
        }

        return localDrive;
    }
    public static string GetLink(string linkName)
    {
        cDAL oDataLayer = new cDAL(cDAL.ConnectionType.INIT);
        string sql = "SELECT LnkUrl, LnkName FROM SRM.zlkupLnk WHERE LnkName = '" + linkName + "'";
        return oDataLayer.GetObject(sql).ToString();
    }

    public static string GetSysValue(string sysCode)
    {
        cDAL oDataLayer = new cDAL(cDAL.ConnectionType.INIT);
        string sql = "SELECT sysValue FROM zSysini WHERE sysCode = '" + sysCode + "'";
        return oDataLayer.GetObject(sql).ToString();
    }
    //public static ArrayList SetValues(DataTable data, string className)
    //{
    //    ArrayList objectCollection = new ArrayList();

    //    foreach (DataRow row in data.Rows)
    //    {
    //        Type type = Type.GetType("IP.Models." + className);
    //        object oClass = Activator.CreateInstance(type);

    //        foreach (DataColumn dc in data.Columns)
    //        {
    //            object value = row[dc.ColumnName];
    //            PropertyInfo propInfo = oClass.GetType().GetProperty(dc.ColumnName);
    //            if (propInfo != null)
    //            {
    //                Type t = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
    //                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
    //                if (safeValue.GetType().Equals(typeof(DateTime)))
    //                    safeValue = Convert.ToDateTime(safeValue).ToString(Format.DateTime);

    //                propInfo.SetValue(oClass, safeValue, null);
    //            }
    //        }

    //        objectCollection.Add(oClass);
    //    }

    //    return objectCollection;
    //}

    public static List<Dictionary<string, object>> DataTableToDictionary(DataTable dt)
    {
        List<Dictionary<string, object>> dictionaries = new List<Dictionary<string, object>>();
        foreach (DataRow row in dt.Rows)
        {
            Dictionary<string, object> dictionary = Enumerable.Range(0, dt.Columns.Count)
                .ToDictionary(i => dt.Columns[i].ColumnName, i => row.ItemArray[i]);
            dictionaries.Add(dictionary);
        }
        return dictionaries;
    }

    public static List<Dictionary<string, object>> ConvertDtToList(DataTable dt)
    {
        List<Dictionary<string, object>>
        lstRows = new List<Dictionary<string, object>>();
        Dictionary<string, object> dictRow = null;

        foreach (DataRow dr in dt.Rows)
        {
            dictRow = new Dictionary<string, object>();

            foreach (DataColumn col in dt.Columns)
            {
                object columnValue = dr[col];

                if (columnValue.GetType().Equals(typeof(decimal)))
                {
                    if (col.ColumnName.ToUpper().Contains("PRICE") || col.ColumnName.ToUpper().Contains("COST"))
                    {
                        columnValue = SetFormat(columnValue, Format.ForPrice);
                        if (columnValue.ToString() == "0.00") { columnValue = ""; }
                    }

                    else
                    {
                        columnValue = SetFormat(columnValue, Format.ForQty);

                    }

                }
                else if (columnValue.GetType().Equals(typeof(int)))
                {
                    columnValue = SetFormat(columnValue, Format.ForQty);
                }
                else if (columnValue.GetType().Equals(typeof(DateTime)))
                {
                    if (columnValue.ToString().Length == 10 || columnValue.ToString().Contains("12:00:00 AM"))
                        columnValue = SetFormat(columnValue, Format.DateOnly);
                    else
                        columnValue = SetFormat(columnValue, Format.DateTime);
                }

                if (columnValue.ToString() == "0") { columnValue = ""; }
                dictRow.Add(col.ColumnName, columnValue == DBNull.Value ? "" : columnValue);
            }

            lstRows.Add(dictRow);
        }
        return lstRows;
    }
    public static List<ArrayList> ConvertDtToArrayList(DataTable dt)
    {
        List<ArrayList> lstRows = new List<ArrayList>();

        ArrayList dictRow = null;

        foreach (DataRow dr in dt.Rows)
        {
            dictRow = new ArrayList();
            foreach (DataColumn col in dt.Columns)
            {
                object columnValue = dr[col];
                if (columnValue.GetType().Equals(typeof(decimal)))
                {
                    if (col.ColumnName.ToUpper().Contains("PRICE") || col.ColumnName.ToUpper().Contains("COST"))
                    {
                        columnValue = SetFormat(columnValue, Format.ForPrice);
                        if (columnValue.ToString() == "0.00") { columnValue = ""; }
                    }
                    else
                    {
                        columnValue = SetFormat(columnValue, Format.ForQty);
                        if (columnValue.ToString() == "0") { columnValue = ""; }

                    }

                }
                //Comment By Tahir , it will be resolve later
                //else if (columnValue.GetType().Equals(typeof(int)))
                //{
                //    columnValue = SetFormat(columnValue, Format.ForQty);
                //    if (columnValue.ToString() == "0") { columnValue = ""; }
                //}

                else if (columnValue.GetType().Equals(typeof(DateTime)))
                {
                    if (columnValue.ToString().Length == 10 || columnValue.ToString().Contains("12:00:00 AM"))
                        columnValue = SetFormat(columnValue, Format.DateOnly);
                    else
                        columnValue = SetFormat(columnValue, Format.DateTime);
                }

                if (columnValue.ToString() == "0") { columnValue = ""; }
                if (DBNull.Value == columnValue)
                {
                    columnValue = "";
                }
                dictRow.Add(columnValue);
            }
            lstRows.Add(dictRow);
        }
        return lstRows;
    }

    public static List<ArrayList> ConvertDtToArrayListMRP(DataTable dt)
    {
        List<ArrayList> lstRows = new List<ArrayList>();

        ArrayList dictRow = null;

        foreach (DataRow dr in dt.Rows)
        {
            dictRow = new ArrayList();
            foreach (DataColumn col in dt.Columns)
            {
                object columnValue = dr[col];

                if (columnValue.ToString().ToUpper() == "NULL")
                    columnValue = "N/A";

                if (DBNull.Value == columnValue)
                {
                    columnValue = "N/A";
                }
                dictRow.Add(columnValue);
            }
            lstRows.Add(dictRow);
        }
        return lstRows;
    }

    public static List<Hashtable> ConvertDtToHashTable(DataTable dt)
    {
        List<Hashtable> lstRows = new List<Hashtable>();

        Hashtable ht = null;

        ArrayList dictRow = null;

        foreach (DataRow dr in dt.Rows)
        {
            ht = new Hashtable();
            dictRow = new ArrayList();
            foreach (DataColumn col in dt.Columns)
            {
                object columnValue = dr[col];
                if (columnValue.GetType().Equals(typeof(decimal)))
                {
                    if (col.ColumnName.ToUpper().Contains("PRICE") || col.ColumnName.ToUpper().Contains("COST"))
                    {
                        //columnValue = (columnValue, cCommon.Format.ForPrice);
                        if (columnValue.ToString() == "0.00") { columnValue = ""; }
                    }
                    else
                    {
                        //columnValue = (columnValue, cCommon.Format.ForQty);
                        if (columnValue.ToString() == "0") { columnValue = ""; }

                    }

                }
                //Comment By Tahir , it will be resolve later
                //else if (columnValue.GetType().Equals(typeof(int)))
                //{
                //    columnValue = SetFormat(columnValue, Format.ForQty);
                //    if (columnValue.ToString() == "0") { columnValue = ""; }
                //}

                else if (columnValue.GetType().Equals(typeof(DateTime)))
                {
                    if (columnValue.ToString().Length == 10 || columnValue.ToString().Contains("12:00:00 AM"))
                        columnValue = Convert.ToDateTime(columnValue).ToString("yyyy.MM.dd"); //(columnValue, cCommon.Format.DateOnly);// Changed by tahir
                    else
                        columnValue = Convert.ToDateTime(columnValue).ToString("yyyy.MM.dd HH:mm");// (columnValue, cCommon.Format.DateTime); changed by tahir
                }

                if (columnValue.ToString() == "0") { columnValue = ""; }
                if (DBNull.Value == columnValue)
                {
                    columnValue = "";
                }
                ht.Add(col.ColumnName, columnValue);
            }
            lstRows.Add(ht);
        }
        return lstRows;
    }
    public static List<Hashtable> ConvertDtToHashTableMeta(DataTable dt)
    {
        List<Hashtable> lstRows = new List<Hashtable>();

        Hashtable ht = null;

        ArrayList dictRow = null;

        foreach (DataRow dr in dt.Rows)
        {
            ht = new Hashtable();
            dictRow = new ArrayList();
            foreach (DataColumn col in dt.Columns)
            {
                object columnValue = dr[col];
                if (columnValue.GetType().Equals(typeof(decimal)))
                {
                    if (col.ColumnName.ToUpper().Contains("PRICE") || col.ColumnName.ToUpper().Contains("COST"))
                    {
                        //columnValue = (columnValue, cCommon.Format.ForPrice);
                        if (columnValue.ToString() == "0.00") { columnValue = ""; }
                    }
                    else
                    {
                        //columnValue = (columnValue, cCommon.Format.ForQty);
                        if (columnValue.ToString() == "0") { columnValue = ""; }

                    }

                }
                //Comment By Tahir , it will be resolve later
                //else if (columnValue.GetType().Equals(typeof(int)))
                //{
                //    columnValue = SetFormat(columnValue, Format.ForQty);
                //    if (columnValue.ToString() == "0") { columnValue = ""; }
                //}

                else if (columnValue.GetType().Equals(typeof(DateTime)))
                {
                    if (columnValue.ToString().Length == 10 || columnValue.ToString().Contains("12:00:00 AM"))
                        columnValue = Convert.ToDateTime(columnValue).ToString("yyyy.MM.dd"); //(columnValue, cCommon.Format.DateOnly);// Changed by tahir
                    else
                        columnValue = Convert.ToDateTime(columnValue).ToString("yyyy.MM.dd HH:mm:ss");// (columnValue, cCommon.Format.DateTime); changed by tahir
                }

                if (columnValue.ToString() == "0") { columnValue = ""; }
                if (DBNull.Value == columnValue)
                {
                    columnValue = "";
                }
                ht.Add(col.ColumnName, columnValue);
            }
            lstRows.Add(ht);
        }
        return lstRows;
    }

    public static IList<SelectListItem> BindDropDownList(string query, string text, string value, cDAL.ConnectionType conStr, string firstItem = "")
    {
        IList<SelectListItem> lst;
        cDAL oDAL = new cDAL(conStr);
        DataTable dt = oDAL.GetData(query);

        if (dt.Rows.Count == 0)
        {
            dt.Rows.Add();
            dt.Rows[0][value] = 0;
            dt.Rows[0][text] = "No Data Available";
        }
        else if (!string.IsNullOrEmpty(firstItem))
        {
            DataRow dr;
            dr = dt.NewRow();
            string coltype = dr.Table.Columns[value].DataType.Name;
            if (coltype == typeof(System.Int32).ToString().Split('.')[1])
                dr[value] = 0;
            else
                dr[value] = firstItem;

            dr[text] = firstItem;
            dt.Rows.InsertAt(dr, 0);
        }
        lst = dt.AsEnumerable().Select(dataRow => new SelectListItem
        {
            Text = dataRow[text].ToString(),
            Value = dataRow[value].ToString(),
        }).ToList<SelectListItem>();
        if (lst.Count > 0)
            return lst;
        else
            return lst = null;

    }

    public static IEnumerable<SelectListItem> ToDropDownList(DataTable table, string valueField, string textField, string selectedField = "", string isSelected = "")
    {
        List<SelectListItem> list = new List<SelectListItem>();
        foreach (DataRow row in table.Rows)
        {
            list.Add(new SelectListItem()
            {
                Text = row[textField].ToString(),
                Value = row[valueField].ToString(),
                Selected = selectedField == row[isSelected].ToString() ? true : false
            });
        }
        return list;
    }

    public static IEnumerable<SelectListItem> ToDropDown(DataTable table, string valueField, string textField, int selected)
    {
        List<SelectListItem> list = new List<SelectListItem>();
        foreach (DataRow row in table.Rows)
        {
            list.Add(new SelectListItem()
            {
                Text = row[textField].ToString(),
                Value = row[valueField].ToString(),
                Selected = selected == (int)row[valueField] ? true : false
            });
        }
        return list;
    }

    public static IEnumerable<SelectListItem> ToDropDown(DataTable table, string valueField, string textField, string firstItem = "All")
    {
        if (table.Rows.Count == 0)
        {
            table.Rows.Add();
            table.Rows[0][valueField] = 0;
            table.Rows[0][textField] = "No Data Available";
        }
        else if (!string.IsNullOrEmpty(firstItem))
        {
            DataRow dr;
            dr = table.NewRow();
            string coltype = dr.Table.Columns[valueField].DataType.Name;
            if (coltype == typeof(System.Int16).ToString().Split('.')[1] || coltype == typeof(System.Int32).ToString().Split('.')[1] || coltype == typeof(System.Byte).ToString().Split('.')[1])
                dr[valueField] = 0;
            else
                dr[valueField] = firstItem;

            dr[textField] = firstItem;
            table.Rows.InsertAt(dr, 0);
        }
        List<SelectListItem> list = new List<SelectListItem>();
        foreach (DataRow row in table.Rows)
        {
            list.Add(new SelectListItem()
            {
                Text = row[textField].ToString(),
                Value = row[valueField].ToString()
            });
        }
        return list;
    }

    public static IEnumerable<SelectListItem> ToDropDownList(DataTable table, string valueField, string textField)
    {
        if (table.Rows.Count == 0)
        {
            table.Rows.Add();
            table.Rows[0][valueField] = 0;
            table.Rows[0][textField] = "No Data Available";
        }

        List<SelectListItem> list = new List<SelectListItem>();
        foreach (DataRow row in table.Rows)
        {
            list.Add(new SelectListItem()
            {
                Text = row[textField].ToString(),
                Value = row[valueField].ToString()
            });
        }
        return list;
    }

    public static DataTable GenerateTransposedTable(DataTable inputTable)
    {
        DataTable outputTable = new DataTable();

        // Add columns by looping rows

        // Header row's first column is same as in inputTable
        outputTable.Columns.Add("HeaderName");

        // Header row's second column onwards, 'inputTable's first column taken
        foreach (DataRow inRow in inputTable.Rows)
        {
            string newColName = inRow[0].ToString();
            outputTable.Columns.Add(newColName);
        }

        // Add rows by looping columns        
        for (int rCount = 0; rCount <= inputTable.Columns.Count - 1; rCount++)
        {
            DataRow newRow = outputTable.NewRow();

            // First column is inputTable's Header row's second column
            newRow[0] = inputTable.Columns[rCount].ColumnName.ToString();
            for (int cCount = 0; cCount <= inputTable.Rows.Count - 1; cCount++)
            {
                string colValue = inputTable.Rows[cCount][rCount].ToString();
                newRow[cCount + 1] = colValue;
            }
            outputTable.Rows.Add(newRow);
        }

        return outputTable;
    }

    public static string SwitchToDetailPage(string isSingle)
    {
        string layout = "~/Views/Shared/_LayoutEmpty.cshtml";
        if (isSingle.Contains("page=detailed"))
        {
            layout = null;
        }
        else if (isSingle.Contains("page=list"))
        {
            layout = "~/Views/Shared/_LayoutEmpty.cshtml";
        }
        return layout;
    }

    public static Hashtable breakConStr(string conStr)
    {
        char splitAttribute = ';';
        char splitKeyValue = '=';
        Hashtable info = new Hashtable();

        string[] attributes = conStr.Split(splitAttribute);
        string[] keyAndValue = null;
        string key = string.Empty;
        string value = string.Empty;
        for (int i = 0; i < attributes.Length - 1; i++)
        {
            keyAndValue = attributes[i].Split(splitKeyValue);
            key = keyAndValue[0];
            value = keyAndValue[1];
            info.Add(key, value);
        }
        return info;
    }

    public static object SetFormat(object value, Format format)
    {
        switch (format)
        {
            case Format.DateOnly:
                if (!string.IsNullOrEmpty(value.ToString())) { value = Convert.ToDateTime(value).ToString("yyyy.MM.dd"); }
                break;
            case Format.DateTime:
                if (!string.IsNullOrEmpty(value.ToString())) { value = Convert.ToDateTime(value).ToString("yyyy.MM.dd HH:mm"); }
                break;
            case Format.DayOnly:
                if (!string.IsNullOrEmpty(value.ToString())) { value = Convert.ToDateTime(value).ToString("dd"); }
                break;
            case Format.ForQty:
                if (!string.IsNullOrEmpty(value.ToString())) { value = Convert.ToDecimal(value).ToString("#,#"); }
                break;
            case Format.ForPrice:
                if (!string.IsNullOrEmpty(value.ToString())) { value = Convert.ToDecimal(value).ToString("#,0.00##"); }
                break;
            default:
                break;
        }

        return value;
    }

    public static bool IsNumber(string validString)
    {
        return Regex.IsMatch(validString, "^[0-9]+$");
    }

    public static bool IsDecimal(string validString)
    {
        bool isValid = false;

        try
        {
            isValid = Regex.IsMatch(validString, @"^-?[0-9]\d*(\.\d+)?$");

            if (!isValid)
                isValid = Regex.IsMatch(Decimal.Parse(validString, System.Globalization.NumberStyles.Float).ToString(), @"^-?[0-9]\d*(\.\d+)?$");
        }
        catch (Exception)
        {
            isValid = false;
        }



        return isValid;
    }
    public static List<Hashtable> ConvertDtToHashTableWithZero(DataTable dt)
    {
        List<Hashtable> lstRows = new List<Hashtable>();

        Hashtable ht = null;

        ArrayList dictRow = null;

        foreach (DataRow dr in dt.Rows)
        {
            ht = new Hashtable();
            dictRow = new ArrayList();
            foreach (DataColumn col in dt.Columns)
            {
                object columnValue = dr[col];
                if (columnValue.GetType().Equals(typeof(decimal)))
                {
                    if (col.ColumnName.ToUpper().Contains("PRICE") || col.ColumnName.ToUpper().Contains("COST"))
                    {
                        columnValue = SetFormat(columnValue, Format.ForPrice);
                        if (columnValue.ToString() == "0.00") { columnValue = "0"; }
                    }
                    else
                    {
                        columnValue = SetFormat(columnValue, Format.ForQty);
                        if (columnValue.ToString() == "0") { columnValue = "0"; }
                        if (columnValue.ToString() == "0.0") { columnValue = "0.0"; }

                    }

                }
                //Comment By Tahir , it will be resolve later
                //else if (columnValue.GetType().Equals(typeof(int)))
                //{
                //    columnValue = SetFormat(columnValue, Format.ForQty);
                //    if (columnValue.ToString() == "0") { columnValue = ""; }
                //}

                else if (columnValue.GetType().Equals(typeof(DateTime)))
                {
                    if (columnValue.ToString().Length == 10 || columnValue.ToString().Contains("12:00:00 AM"))
                        columnValue = Convert.ToDateTime(columnValue).ToString("yyyy.MM.dd"); //(columnValue, cCommon.Format.DateOnly);// Changed by tahir
                    else
                        columnValue = Convert.ToDateTime(columnValue).ToString("yyyy.MM.dd HH:mm");// (columnValue, cCommon.Format.DateTime); changed by tahir
                }
                if (columnValue.ToString() == "0") { columnValue = "0"; }

                if (columnValue.ToString() == "NULL") { columnValue = ""; }
                //if (columnValue.ToString() == "0") { columnValue = ""; }
                //if (DBNull.Value == columnValue)
                //{
                //    columnValue = "";
                //}
                ht.Add(col.ColumnName, columnValue);
            }
            lstRows.Add(ht);
        }
        return lstRows;
    }



    public enum Format
    {
        DateOnly,
        DateTime,
        DayOnly,
        ForQty,
        ForPrice
    }

    /// <summary>
    /// Remove invalid Symbols 
    /// </summary>
    /// <param name="text">String</param>
    /// <returns>String</returns>
    public static string RemoveSymbols(string text)
    {
        text = text.Trim();
        text = Regex.Replace(text, @"[^\w\s\.@\-#()_,=\*\^]", "");
        return text;
    }

    public static string EncodeImage(byte[] bytes, int? width = null, int? height = null, long jpegQuality = 100L)
    {
        using (var ms = new MemoryStream(bytes))
        {
            var source = Image.FromStream(ms);
            int newWidth = width.HasValue ? width.Value : source.Width;
            int newHeight = height.HasValue ? height.Value : source.Height;

            var destination = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(destination))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                g.DrawImage(source, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }
            using (var msRet = new MemoryStream())
            {
                ImageCodecInfo jpeg = ImageCodecInfo.GetImageEncoders().Single(c => c.FormatID == ImageFormat.Jpeg.Guid);
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters encoderParameters = new EncoderParameters(1);
                EncoderParameter encoderParameter = new EncoderParameter(encoder, jpegQuality);
                encoderParameters.Param[0] = encoderParameter;
                destination.Save(msRet, jpeg, encoderParameters);
                return "data:image/jpeg;base64," + Convert.ToBase64String(msRet.ToArray());
            }
        }
    }

    public static bool IsSessionExpired()
    {
        is_external_request = false;
        string is_external = HttpContext.Current.Request.QueryString["IsExternal"];
        if (!string.IsNullOrEmpty(is_external))
            is_external_request = true;

        if (HttpContext.Current.Session["SigninId"] == null && !is_external_request)
            return true;
        else
            return false;
    }

    public static void SessionExpired()
    {
        HttpContext.Current.Session["SigninId"] = null;
        HttpContext.Current.Session["ProgramId"] = null;
        HttpContext.Current.Session["Program"] = null;
        HttpContext.Current.Session["FirstName"] = null;
        HttpContext.Current.Session["LastName"] = null;
        HttpContext.Current.Session["CustList"] = null;
        
    }
}


public static class Format
{
    public static string DateOnly = "yyyy.MM.dd";
    public static string DateTime = "yyyy.MM.dd hh:mm";
    public static string DayOnly = "dd";
    public static string ForQty = "#,#";
    public static string ForPrice = "#,0.00##";
}


