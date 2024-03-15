using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;

namespace PlusCP.Externals
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        { 
            if(!Page.IsPostBack)
            {
                //string signId = "mohsin";
                string signId = Encoding.UTF8.GetString(Convert.FromBase64String(Request.QueryString["id"]));
                lblsignid.Text = signId;

            }

        }
    }
}