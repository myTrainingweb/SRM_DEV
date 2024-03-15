using System;
using System.Collections.Generic;
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
                string PO = "";
                string buyer = "";
                string partNo = "";
                string dueDate = "";
                string qty = "";
                string price = "";

                PO = Request.QueryString["PO"].ToString();
                buyer = Request.QueryString["Buyer"].ToString();
                partNo = Request.QueryString["PartNo"].ToString();
                dueDate = Request.QueryString["DueDate"].ToString();
                qty = Request.QueryString["Qty"].ToString();
                price = Request.QueryString["price"].ToString();

                lblPONo.Text = PO;
                lblsBuyer.Text = buyer;
                lblsPartNo.Text = partNo;
                lblsDueDate.Text = dueDate;
                lblsQty.Text = qty;
                lblsPrice.Text = price;
            }
        }
    }
}