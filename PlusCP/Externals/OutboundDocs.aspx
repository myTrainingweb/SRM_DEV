<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutboundDocs.aspx.cs" Inherits="PlusCP.Externals.OutboundDocs" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="Microsoft.Win32" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    /// <summary>
    ///  To open file from shared path 
    ///  In email shared path link was not working so we do it as follows.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        string contract = Request.QueryString["Contract"];
        string DocPath = Request.QueryString["DocPath"];
       

        if (string.IsNullOrEmpty(DocPath))
            return;

        
            FileInfo oFile = new FileInfo(DocPath);
            if (oFile.Exists)
            {
                string contentType = "application/unknown";
                string ext = Path.GetExtension(DocPath).ToLower();
                RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    contentType = regKey.GetValue("Content Type").ToString();

                byte[] content = File.ReadAllBytes(DocPath);

                try
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.AddHeader("Content-Disposition", "inline; filename=" + oFile.Name);
                    Response.AddHeader("Content-Length", oFile.Length.ToString());
                    Response.ContentType = contentType;
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(content);
                    Response.Flush();
                    Response.End();
                }
                catch (HttpException exp)
                {
                   
                }
            }
        
    }
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
