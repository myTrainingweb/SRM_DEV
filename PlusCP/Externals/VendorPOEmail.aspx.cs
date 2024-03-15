using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlusCP.Externals
{
    public partial class VendorPOEmail : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                string GUID = "";
                string Action = "";
                GUID = Request.QueryString["GUID"].ToString();
                Action = Request.QueryString["Action"].ToString();
                DataTable dt = new DataTable();
                dt = GetData(GUID);
                dgvVendor.DataSource = dt;
                dgvVendor.DataBind();

                if (Action == "Accept")
                {
                    tblChange.Visible = false;
                }
                else if (Action == "Reject")
                {
                    tblChange.Visible = true;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            DataTable dt = new DataTable();
            string Vendor = "";
            string PONo = "";
            string PartNo = "";
            string Qty = "";
            string DueDate = "";
            string Price = "";

            string GUID = "";
            string Action = "";
            string sql = "";
            double Qtyvalue = 0;
            double PriceValue = 0;
            Action = Request.QueryString["Action"].ToString();
            GUID = Request.QueryString["GUID"].ToString();

            sql = "Select * from [SRM].[VendorCommunication] Where GUID = '<GUID>' ";
            sql = sql.Replace("<GUID>", GUID);
            DataTable dtVendorData = new DataTable();
            dtVendorData = oDAL.GetData(sql);
            dt = GetData(GUID);
            if (dtVendorData.Rows.Count == 0)
            {
                sql = @"INSERT INTO [SRM].[VendorCommunication] (VendorName, PONo, PartNo, Qty, DueDate, Price, GUID)
                        VALUES ('<VendorName>','<PONo>','<PartNo>','<Qty>','<DueDate>','<Price>','<GUID>' )";
              
                foreach (DataRow row in dt.Rows)
                {
                    Vendor = row["vendorName"].ToString();
                    PONo = row["PONO"].ToString();
                    PartNo = row["PartNo"].ToString();
                    Qty = row["Qty"].ToString();
                    DueDate = row["DueDate"].ToString();
                    Price = row["Price"].ToString();
                    Qtyvalue = Convert.ToDouble(Qty);
                    PriceValue = Convert.ToDouble(Price);
                    int POFinalPrice = (int)PriceValue;

                    sql = sql.Replace("<VendorName>", Vendor);
                    sql = sql.Replace("<PONo>", PONo);
                    sql = sql.Replace("<PartNo>", PartNo);
                    sql = sql.Replace("<GUID>", GUID);


                }
                if (Action == "Accept")
                {
                    sql = sql.Replace("<Qty>", Qtyvalue.ToString());
                    sql = sql.Replace("<DueDate>", DueDate);
                    sql = sql.Replace("<Price>", PriceValue.ToString());
                    oDAL.Execute(sql);

                }
                else if (Action == "Change")
                {
                    sql = sql.Replace("<Qty>", txtQty.Value);
                    sql = sql.Replace("<DueDate>", txtDueDate.Value);
                    sql = sql.Replace("<Price>", txtPrice.Value);
                    oDAL.Execute(sql);
                }
            }
            else
            {
                foreach (DataRow row in dt.Rows)
                {
                   
                    Qty = row["Qty"].ToString();
                    DueDate = row["DueDate"].ToString();
                    Price = row["Price"].ToString();
                    Qtyvalue = Convert.ToDouble(Qty);
                    PriceValue = Convert.ToDouble(Price);
                    int POFinalPrice = (int)PriceValue;

                }
                sql = @"UPDATE [SRM].[VendorCommunication]
                          SET [Qty] = '<Qty>'
                             ,[DueDate] = '<DueDate>'
                             ,[Price] = '<Price>'
                        WHERE GUID = '<GUID>'";
                if (Action == "Accept")
                {
                    sql = sql.Replace("<Qty>", Qtyvalue.ToString());
                    sql = sql.Replace("<DueDate>", DueDate);
                    sql = sql.Replace("<Price>", PriceValue.ToString());
                    sql = sql.Replace("<GUID>", GUID);
                    oDAL.Execute(sql);
                }
                else if(Action == "Change")
                {
                    sql = sql.Replace("<Qty>", txtQty.Value);
                    sql = sql.Replace("<DueDate>", txtDueDate.Value);
                    sql = sql.Replace("<Price>", txtPrice.Value);
                    sql = sql.Replace("<GUID>", GUID);
                    oDAL.Execute(sql);
                }

            }

            updatePO(GUID, Action);
            Response.Write("<script language='javascript'> { window.close(); }</script>");
        }

        public DataTable GetData(string GUID)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string sql = @"select vendorName, CONCAT(PONum,'-',[LineNo],'-',RelNo) AS PONO, PartNo,Qty,DueDate, Price  
from[SRM].[BuyerPO] WHERE GUID = '<GUID>' ";
            sql = sql.Replace("<GUID>", GUID);

            DataTable dt = new DataTable();
            dt = oDAL.GetData(sql);

            return dt;
        }

        public void updatePO(string GUID, string Action)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string sql = "UPDATE [SRM].[BuyerPO] SET POSTATUS = '<STATUS>' WHERE GUID = '<GUID>' ";
            sql = sql.Replace("<GUID>", GUID);

            if (Action == "Accept")
            {
                sql = sql.Replace("<STATUS>", "Update");
            }
            else if (Action == "Change")
            {
                sql = sql.Replace("<STATUS>", "Change Update");
            }

            oDAL.Execute(sql);

        }

    }
}