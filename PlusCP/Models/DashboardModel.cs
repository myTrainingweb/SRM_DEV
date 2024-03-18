using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PlusCP.Models
{
    public class DashboardModel
    {
        cDAL oDAL;
        #region Data Fields
        public string widget_id { get; set; }
        public string widget_desc { get; set; }
        public string widget_count_count { get; set; }
        public string widget_count_fraction { get; set; }
        public string widget_count_bg { get; set; }
        public string widget_count_report_URL { get; set; }
        public string report_URL { get; set; }
        public string report_name { get; set; }
        public string report_target { get; set; }
        public string report_title_short { get; set; }
        public string report_designed_by { get; set; }
        public bool report_is_internal { get; set; } = true;
        public string report_code { get; set; }
        public string chart_title { get; set; }
        public List<Dictionary<string, object>> lst_widgets { get; set; }
        public List<Employee_Widgets> lst_employee_widgets { get; set; }
        public List<ArrayList> lst_widget_hyperlist { get; set; }
        public List<ArrayList> lst_widget_list { get; set; }
        public List<ArrayList> lst_widget_table { get; set; }
        public List<ArrayList> lst_widget_group_counts { get; set; }
        public string widget_table_column_format { get; set; }
        public int widget_table_cols_count { get; set; }
        public Dictionary<string, object> chart_data { get; set; }

        public DataTable dtStations { get; set; }
        public DataTable dtCurrent { get; set; }
        public List<ArrayList> dataChart { get; set; }
        public DataTable dtChart { get; set; }
        #endregion

        #region Structures
        public struct Employee_Widgets
        {
            public string WidgetId { get; set; }
            public string WidgetTitle { get; set; }
            public string WidgetType { get; set; }
            public string WidgetMinSize { get; set; }
            public string WidgetMaxSize { get; set; }
            public string WidgetPositionY { get; set; }
            public string WidgetPositionX { get; set; }
            public string WidgetSizeX { get; set; }
            public string WidgetSizeY { get; set; }
        }

        #endregion

        #region Methods
        public void Get_Widget_List()
        {
            string query = string.Empty;
            oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            bool isAdmin = Convert.ToBoolean(HttpContext.Current.Session["isAdmin"]);
            if (isAdmin)
            {
                query = @"
SELECT  WidgetId
	  , WidgetIcon
	  , WidgetTitle
	  , WidgetType
	  , WidgetMinSize
	  , WidgetMaxSize 
FROM SRM.Widgets W
WHERE WidgetStatus = 1 ";
            }
            else
            {
                query = @"
-- It is getting common widgets
SELECT  WidgetId
	  , WidgetIcon
	  , WidgetTitle
	  , WidgetType
	  , WidgetMinSize
	  , WidgetMaxSize 
FROM SRM.Widgets W
WHERE W.ForMnuId = 0 AND
	  WidgetStatus = 1  

UNION

-- It is getting widget by mnu assigned to a user
SELECT WidgetId
	 , WidgetIcon
	 , WidgetTitle
	 , WidgetType
	 , WidgetMinSize
	 , WidgetMaxSize 
FROM SRM.Widgets W
INNER JOIN SRM.UserMnuX UMX ON UMX.MnuId = W.ForMnuId
WHERE WidgetStatus = 1 AND 
	  UMX.UserId = '@LOGON_USER'
ORDER BY WidgetTitle ";
            }


            query = query.Replace("@LOGON_USER", HttpContext.Current.Session["SigninId"].ToString());
            DataTable dtWidgetsEmployee = oDAL.GetData(query);

            lst_widgets = ConvertDtToList(dtWidgetsEmployee);
        }

        public void Get_Employee_Widgets()
        {
            string query = string.Empty;
            cDAL portal_db = new cDAL(cDAL.ConnectionType.ACTIVE);
            int employee_id = Convert.ToInt32(HttpContext.Current.Session["SigninId"]);
            bool isAdmin = Convert.ToBoolean(HttpContext.Current.Session["isAdmin"]);
            if (isAdmin)
            {
                query = @"
SELECT W.WidgetId
     , W.WidgetTitle
     , W.WidgetType
     , W.WidgetMinSize
     , W.WidgetMaxSize
     , WidgetPositionX
     , WidgetPositionY
     , WidgetSizeX
     , WidgetSizeY 
FROM SRM.Widgets W 
INNER JOIN SRM.WidgetsEmp E ON E.WidgetId = W.WidgetId 
WHERE W.WidgetStatus = 1 AND E.EmpId = @EMP_ID
";
            }
            else 
            {
                query = @"
SELECT W.WidgetId
     , W.WidgetTitle
     , W.WidgetType
     , W.WidgetMinSize
     , W.WidgetMaxSize
     , WidgetPositionX
     , WidgetPositionY
     , WidgetSizeX
     , WidgetSizeY 
FROM SRM.Widgets W 
INNER JOIN SRM.WidgetsEmp E ON E.WidgetId = W.WidgetId 
WHERE W.ForMnuId = 0 AND
	  WidgetStatus = 1 AND
	  E.EmpId = @EMP_ID

UNION

SELECT W.WidgetId
     , W.WidgetTitle
     , W.WidgetType
     , W.WidgetMinSize
     , W.WidgetMaxSize
     , WidgetPositionX
     , WidgetPositionY
     , WidgetSizeX
     , WidgetSizeY 
FROM SRM.Widgets W 
INNER JOIN SRM.WidgetsEmp E ON E.WidgetId = W.WidgetId 
INNER JOIN SRM.UserMnuX UMX ON 
           UMX.MnuId = W.ForMnuId AND 
           UMX.UserId  = CAST(E.EmpId AS VARCHAR)
WHERE W.WidgetStatus = 1 AND E.EmpId = @EMP_ID
";
            }
            

            query = query.Replace("@EMP_ID", employee_id.ToString());

            DataTable dtWidgetsEmployee = portal_db.GetData(query);

            lst_employee_widgets = new List<Employee_Widgets>();
            Employee_Widgets oEmpWidget;
            foreach (DataRow dr in dtWidgetsEmployee.Rows)
            {
                oEmpWidget = new Employee_Widgets();
                oEmpWidget.WidgetId = dr["WidgetId"].ToString();
                oEmpWidget.WidgetTitle = dr["WidgetTitle"].ToString();
                oEmpWidget.WidgetType = dr["WidgetType"].ToString();
                oEmpWidget.WidgetMinSize = dr["WidgetMinSize"].ToString();
                oEmpWidget.WidgetMaxSize = dr["WidgetMaxSize"].ToString();
                oEmpWidget.WidgetPositionY = dr["WidgetPositionY"].ToString();
                oEmpWidget.WidgetPositionX = dr["WidgetPositionX"].ToString();
                oEmpWidget.WidgetSizeX = dr["WidgetSizeX"].ToString();
                oEmpWidget.WidgetSizeY = dr["WidgetSizeY"].ToString();
                lst_employee_widgets.Add(oEmpWidget);
            }
        }

        public void Get_Count_Widget(string widget_id)
        {
            cDAL portal_db = new cDAL(cDAL.ConnectionType.ACTIVE);

            string query = string.Empty;

            query = "SELECT WidgetQuery, WidgetDesc, ReportURL, CountType, BackgroundColor, ConType FROM SRM.Widgets WHERE WidgetId = " + widget_id + " ";
            DataTable dtWidget = portal_db.GetData(query);
            widget_desc = dtWidget.Rows[0]["WidgetDesc"].ToString();
            widget_count_report_URL = dtWidget.Rows[0]["ReportURL"].ToString();
            widget_count_bg = dtWidget.Rows[0]["BackgroundColor"].ToString();
            query = dtWidget.Rows[0]["WidgetQuery"].ToString();
            query = change_query_params(query);
            query = query.Replace("<programId>", HttpContext.Current.Session["ProgramId"].ToString());

            string connection_type = dtWidget.Rows[0]["ConType"].ToString();
            cDAL widget_db = get_widget_connection(connection_type.ToUpper());
            //string widget_count_type = dtWidget.Rows[0]["CountType"].ToString();
            //if (widget_count_type.ToLower() == "cost")
            //    widget_count_count = cCommon.SetFormat(widget_db.GetObject(query), cCommon.Format.ForQty).ToString();
            //else
            string count_value = cCommon.SetFormat(widget_db.GetObject(query), cCommon.Format.ForQty).ToString();
            if (count_value.Length == 0)
                count_value = "0";
            widget_count_count = count_value;
        }

        public void Get_Group_Counts_Widget(string widget_id)
        {
            cDAL portal_db = new cDAL(cDAL.ConnectionType.ACTIVE);

            string query = string.Empty;

            query = "SELECT A.WidgetTitle, A.WidgetQuery, A.ReportURL, A.CountType, A.BackgroundColor, A.ConType  ";
            query += "FROM SRM.Widgets A ";
            query += "WHERE A.GroupId = " + widget_id + " ";
            DataTable dtWidget = portal_db.GetData(query);
            string connection_type = dtWidget.Rows[0]["ConType"].ToString();
            cDAL widget_db = get_widget_connection(connection_type.ToUpper());
            lst_widget_group_counts = new List<ArrayList>();
            ArrayList count_widget = new ArrayList();

            for (int i = 0; i < dtWidget.Rows.Count; i++)
            {
                count_widget = new ArrayList();
                count_widget.Add(dtWidget.Rows[i]["WidgetTitle"].ToString());
                count_widget.Add(dtWidget.Rows[i]["ReportURL"].ToString());
                count_widget.Add(dtWidget.Rows[i]["CountType"].ToString());
                count_widget.Add(dtWidget.Rows[i]["BackgroundColor"].ToString());

                query = dtWidget.Rows[i]["WidgetQuery"].ToString();
                query = change_query_params(query);
                string count_value = cCommon.SetFormat(widget_db.GetObject(query), cCommon.Format.ForQty).ToString();
                if (count_value.Length == 0)
                    count_value = "0";
                count_widget.Add(count_value);
                lst_widget_group_counts.Add(count_widget);
            }
        }

        public void Get_List_Widget(string widget_id)
        {
            cDAL portal_db = new cDAL(cDAL.ConnectionType.ACTIVE);


            #region SQL
            string query = string.Empty;
            query = "SELECT WidgetQuery, ConType FROM SRM.Widgets WHERE WidgetId = " + widget_id + " ";
            DataTable dtWidget = portal_db.GetData(query);

            query = dtWidget.Rows[0]["WidgetQuery"].ToString();
            query = change_query_params(query);


            string connection_type = dtWidget.Rows[0]["ConType"].ToString();
            cDAL widget_db = get_widget_connection(connection_type.ToUpper());
            DataTable dt = widget_db.GetData(query);
            lst_widget_list = cCommon.ConvertDtToArrayList(dt);
            #endregion
        }

        public void Get_HyperList_Widget(string widget_id)
        {
            cDAL portal_db = new cDAL(cDAL.ConnectionType.ACTIVE);

            string query = string.Empty;

            query = "SELECT WidgetQuery, ConType FROM SRM.Widgets WHERE WidgetId = " + widget_id + " ";
            DataTable dtWidget = portal_db.GetData(query);

            query = dtWidget.Rows[0]["WidgetQuery"].ToString();
            query = change_query_params(query);

            string connection_type = dtWidget.Rows[0]["ConType"].ToString();
            cDAL widget_db = get_widget_connection(connection_type.ToUpper());

            DataTable dt = widget_db.GetData(query);
            lst_widget_hyperlist = cCommon.ConvertDtToArrayList(dt);
        }

        public void Get_Table_Widget(string widget_id)
        {
            cDAL portal_db = new cDAL(cDAL.ConnectionType.ACTIVE);


            string query = string.Empty;

            query = "SELECT WidgetQuery, ColumnFormat, ColumnFormat, ConType FROM SRM.Widgets WHERE WidgetId = " + widget_id + " ";
            DataTable dtWidget = portal_db.GetData(query);
            string table_headers = dtWidget.Rows[0]["ColumnFormat"].ToString();

            query = dtWidget.Rows[0]["WidgetQuery"].ToString();
            query = change_query_params(query);


            widget_table_column_format = dtWidget.Rows[0]["ColumnFormat"].ToString();

            string connection_type = dtWidget.Rows[0]["ConType"].ToString();
            cDAL widget_db = get_widget_connection(connection_type.ToUpper());
            DataTable dt = widget_db.GetData(query);
            widget_table_cols_count = dt.Columns.Count;
            lst_widget_table = cCommon.ConvertDtToArrayList(dt);
        }

        public void Get_Chart_Widget(string widget_id)
        {
            cDAL portal_db = new cDAL(cDAL.ConnectionType.ACTIVE);


            string query = string.Empty;

            query = "SELECT WidgetTitle, WidgetQuery, ColumnFormat, BackgroundColor, ConType FROM SRM.Widgets WHERE WidgetId = " + widget_id + " ";
            DataTable dtWidget = portal_db.GetData(query);


            query = dtWidget.Rows[0]["WidgetQuery"].ToString();
            query = change_query_params(query);
            query = query.Replace("<programId>", HttpContext.Current.Session["ProgramId"].ToString());

            string connection_type = dtWidget.Rows[0]["ConType"].ToString();
            cDAL widget_db = get_widget_connection(connection_type.ToUpper());
            DataTable dt = widget_db.GetData(query);
            if (dt.Rows.Count > 0)
            {
                string count_value = dt.Rows[0]["units"].ToString();
                if (count_value.Length == 0)
                    count_value = "0";
                widget_count_count = count_value;
            }
            else
                widget_count_count = "0";
            
            chart_data = new Dictionary<string, object>();
            List<Dictionary<string, object>> data_sets = new List<Dictionary<string, object>>();


            string[] x;
            //if (dt.Rows[0]["x"] == string.Empty)
            //    x = new string[dt.Columns.Count - 1];
            //else
            x = new string[dt.Rows.Count];
            string[] headers = dtWidget.Rows[0]["ColumnFormat"].ToString().Split(',');
            string[] back_color = dtWidget.Rows[0]["BackgroundColor"].ToString().Split(',');
            chart_title = dtWidget.Rows[0]["WidgetTitle"].ToString();


            Dictionary<string, object> item;
            string[] y;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                item = new Dictionary<string, object>();
                item["label"] = headers[i - 1];
                y = new string[dt.Rows.Count];
                //if (dt.Rows[0]["x"] == string.Empty)
                //    x[i - 1] = headers[i - 1];

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    //if (dt.Rows[j]["x"] != string.Empty)
                    x[j] = dt.Rows[j]["x"].ToString(); //e.g. no group, android parts

                    y[j] = dt.Rows[j][dt.Columns[i].ColumnName].ToString();
                }
                item["data"] = y;
                item["backgroundColor"] = back_color[i - 1]; //background color define;
                data_sets.Add(item);
            }

            chart_data["labels"] = x;
            chart_data["datasets"] = data_sets;
        }

        public void Get_Report_Info(string link)
        {
            if (!link.Contains("\\\\"))
                link = link.Replace("\\", "\\\\");
            if (link.ToLower().Contains("error"))
            {
                report_URL = link;
                return;
            }
            link = change_link_params(link);
            if (link.ToLower().Contains("http://") || link.ToLower().Contains("http://"))
            {
                report_URL = link;
                report_is_internal = false;
                return;
            }
            cDAL portal_db = new cDAL(cDAL.ConnectionType.ACTIVE);

            string query = string.Empty;


            query = "SELECT RptCode as report_code, MnuTitle as report_title, MnuTitleShort as report_title_short,";
            query += "MnuTarget as report_target, DesignedBy as report_designed_by   ";
            query += "FROM SRM.Mnu ";
            query += "WHERE MnuHyperlink = '" + link + "' ";
            DataTable dt = portal_db.GetData(query);
            report_code = dt.Rows[0]["report_code"].ToString();
            report_title_short = dt.Rows[0]["report_title_short"].ToString();
            report_target = dt.Rows[0]["report_target"].ToString();
            report_designed_by = dt.Rows[0]["report_designed_by"].ToString();
            report_URL = link;
        }

        public void Save_Employee_Widgets(List<Employee_Widgets> lst)
        {
            cDAL portal_db = new cDAL(cDAL.ConnectionType.ACTIVE);

            string query = "";
            //INSERTING WIDGETS FOR SINGLE EMPLOYEE
            int employee_id = Convert.ToInt32(HttpContext.Current.Session["SigninId"]);

            query = "DELETE FROM SRM.WidgetsEmp WHERE EmpId = " + employee_id + " ";
            // query += "AND WidgetId IN (SELECT WidgetId FROM EP.Widgets WHERE Company = '" + HttpContext.Current.Session["CompanyCode"].ToString() + "')";
            portal_db.AddQuery(query);
            if (lst != null)
            {
                foreach (var row in lst)
                {
                    query = "INSERT INTO SRM.WidgetsEmp (EmpID, WidgetId,  WidgetPositionY, WidgetPositionX, ";
                    query += "WidgetSizeX, WidgetSizeY) VALUES (";
                    query += "" + Convert.ToInt32(HttpContext.Current.Session["SigninId"]) + ",";
                    query += "'" + row.WidgetId.Remove(0, 1) + "',";
                    query += "'" + row.WidgetPositionY + "',";
                    query += "'" + row.WidgetPositionX + "',";
                    query += "'" + row.WidgetSizeX + "',";
                    query += "'" + row.WidgetSizeY + "'";
                    query += ")";
                    portal_db.AddQuery(query);
                }
            }
            portal_db.Commit();
        }

        public List<Dictionary<string, object>> ConvertDtToList(DataTable dt)
        {
            List<Dictionary<string, object>>
            lstRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> dictRow = null;

            foreach (DataRow dr in dt.Rows)
            {
                dictRow = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dictRow.Add(col.ColumnName, dr[col]);
                }
                lstRows.Add(dictRow);
            }
            return lstRows;
        }

        private string change_query_params(string query)
        {
            //string epic_company = HttpContext.Current.Session["CompanyCode"].ToString();
            //string epic_plant = HttpContext.Current.Session["CompanyPlant"].ToString();
            int employee_id = Convert.ToInt32(HttpContext.Current.Session["SigninId"]);
            //string PrgramName = HttpContext.Current.Session["ProgramName"].ToString();
            //string win_login = HttpContext.Current.Session["LogonUser"].ToString();

            //query = query.Replace("@Company", "'" + epic_company + "'");
            //query = query.Replace("@Plant", "'" + epic_plant + "'");
            query = query.Replace("@EmpId", "'" + employee_id.ToString() + "'");
            //query = query.Replace("@ProgramName", "'" + PrgramName + "'");
            //query = query.Replace("@WinLogin", "'" + win_login + "'");
            //query = query.Replace("\r\n", "");
            return query;
        }

        private string change_link_params(string link)
        {
            //string epic_company = HttpContext.Current.Session["CompanyCode"].ToString();
            //string epic_plant = HttpContext.Current.Session["CompanyPlant"].ToString();
            int employee_id = Convert.ToInt32(HttpContext.Current.Session["SigninId"]);
            //string win_login = HttpContext.Current.Session["LogonUser"].ToString();

            //link = link.Replace("@Company", epic_company);
            //link = link.Replace("@Plant", epic_plant);
            link = link.Replace("@EmpId", employee_id.ToString());
            //link = link.Replace("@WinLogin", win_login);

            return link;
        }
        private cDAL get_widget_connection(string connection_type)
        {
            switch (connection_type)
            {
                case "PROD":
                    return new cDAL(cDAL.ConnectionType.ACTIVE);
                    break;
                case "TEST":
                    return new cDAL(cDAL.ConnectionType.ACTIVE);
                default:
                    return new cDAL(cDAL.ConnectionType.ACTIVE);
                    break;
            }

        }

        public bool SamsunHorizon()
        {
            return true;
        }

        public void getStations()
        {
            DateTime ClientDate = DateTime.Now;

            ClientDate = ClientDate.ToUniversalTime();
            ClientDate = ClientDate.AddHours(1);
            string s = ClientDate.ToString("HH:mm:ss");
            //string Timeonly = ClientDate.ToLongTimeString();

            //string s = Timeonly.ToString);

            var now = TimeSpan.Parse(s);
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = "SELECT RECNUM, STATION, SHIFT, TARGET, crnt, PASS, LOSS, HOLD FROM RPT.TEST ";
            dtStations = new DataTable();
            dtStations = oDAL.GetData(query);
        }
        public void getTotalCrnt()
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = "SELECT SHIFT, SUM(crnt) AS CURRENT_UNIT, SUM(TARGET) AS TARGET  FROM RPT.TEST GROUP BY SHIFT ";
            dtCurrent = new DataTable();
            dtCurrent = oDAL.GetData(query);
        }
        public bool getChart()
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string query = "SELECT STATION, SHIFT, TARGET, crnt, PASS, LOSS, HOLD FROM RPT.TEST  ";
            dtChart = new DataTable();
            dtChart = oDAL.GetData(query);
            dataChart = cCommon.ConvertDtToArrayList(cCommon.GenerateTransposedTable(dtChart));
            if (!oDAL.HasErrors)
                return true;
            else
                return false;
        }

        #endregion
    }
}