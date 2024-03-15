using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;
using System.Collections;

namespace IP.Extensions
{
    public partial class WidgetReport : System.Web.UI.Page
    {
        cDAL oDAL;
        
       
        public List<ArrayList> lst_widgets { get; set; }
        private string title;
        public string widgetTitle { get { return title; } }

        public string header { get; set; }
        public int counter { get; set; }
        public string rptHyperlink { get; set; }
        public string rptQuery { get; set; }


        protected void Page_Load(object sender, EventArgs e)

        {
            if (!Page.IsPostBack)
            {
                counter = 0;
                ShowData();
            }

        }

        public void ShowData()
        {
            oDAL = new cDAL(cDAL.ConnectionType.INIT);
            string widget_id = Request.QueryString["widget_id"];
            lblReportTitle.Text = Request.QueryString["widget_Title"];
            string query = string.Empty;
            query = @"SELECT WidgetTitle,ReportColumn,ReportHeader, ReportLink FROM EP.widgets WHERE WidgetId =  " + widget_id;
            DataTable dtWidget = oDAL.GetData(query);
            string widget_query = dtWidget.Rows[0]["ReportColumn"].ToString().Replace("@ProgramId", Session["ProgramId"].ToString());
            lblReportTitle.Text = dtWidget.Rows[0]["WidgetTitle"].ToString();
            header = dtWidget.Rows[0]["ReportHeader"].ToString();
            rptHyperlink = dtWidget.Rows[0]["ReportLink"].ToString();
            oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            DataTable dt = oDAL.GetData(widget_query);


            gvlist.DataSource = dt;
            gvlist.DataBind();



        }

        protected void gvlist_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            oDAL = new cDAL(cDAL.ConnectionType.INIT);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                for (int i = 0; i < gvlist.HeaderRow.Cells.Count; i++)
                {
                    string headerText = gvlist.HeaderRow.Cells[i].Text;

                    if (headerText.Contains("RightAlign"))
                    {
                        e.Row.Cells[i].Width = Unit.Pixel(110);
                        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                    }
                    else if (headerText.Contains("INT"))
                    {
                        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                    }
                    //else if (headerText.Contains("Hyperlink"))
                    //{
                    //    HyperLink the_url = new HyperLink();
                    //    string link = rptHyperlink;
                    //    DataRowView dr = (DataRowView)e.Row.DataItem;
                    //    string id = dr["CO1"].ToString();
                    //    string CR = dr["CO2_Hyperlink"].ToString();
                    //    link = link.Replace("@id", id);
                    //    link = link.Replace("@CR", CR);
                    //    the_url.NavigateUrl = link;
                    //    the_url.Text = e.Row.Cells[i].Text;
                    //    e.Row.Cells[i].Controls.Add(the_url);
                    //}
                }
            }

        }


        protected void gvlist_PeRender(object sender, EventArgs e)
        {
            oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            DataTable dt = oDAL.GetData(header);
            for (int i = 0; i < gvlist.HeaderRow.Cells.Count; i++)
            {
                gvlist.HeaderRow.Cells[i].Text = dt.Columns[i].ToString();
            }


        }

        protected void gvlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string designation = gvlist.SelectedRow.Cells[0].Text;
        }
        protected void gvlist_Init(object sender, EventArgs e)
        {
            lblTitle.Text = Request.QueryString["widget_Title"];

        }
    }
}