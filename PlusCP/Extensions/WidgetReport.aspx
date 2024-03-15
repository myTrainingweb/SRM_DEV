<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WidgetReport.aspx.cs" Inherits="IP.Extensions.WidgetReport" %>

<!DOCTYPE html>
<link href="../Content/css/bootstrap.min.css" rel="stylesheet" />
<link href="../Content/css/bootstrap-multiselect.css" rel="stylesheet" />
<link href="../Content/css/Site.css" rel="stylesheet" />
<link href="../Content/css/datatable.min.css" rel="stylesheet" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Plus-RS</title>
    <link href="../Content/css/_all-skins.min.css" rel="stylesheet" />
    <link href="../Content/css/AdminLTE.min.css" rel="stylesheet" />
    <link href="../Content/css/all.css" rel="stylesheet" />
    <link href="../Content/css/bootstrap-multiselect.css" rel="stylesheet" />
    <link href="../Content/css/responsive-tables.min.css" rel="stylesheet" />
    <link href="../Content/css/Standards.css" rel="stylesheet" />
    <link href="../Content/css/MyDataGrid.css" rel="stylesheet" />


    <style type="text/css">
        #btnExport {
            padding-top: 5px;
        }
        td {
        padding-right:3px !important;
         padding-left:3px !important;
        }
    </style>
</head>

<body>


    <form id="form1" runat="server">

        <div class="box-header with-border bg-blue-gradient">
            <button type="button" class="btn btn-box-tool" title="Option Page">
                <span class="fa fa-list fa-lg white"></span>
            </button>
            <strong>
                <asp:Label ID="lblTitle" Text="PO List" runat="server" /></strong>
            <strong>
                <asp:Label runat="server" ID="lblReportTitle"></asp:Label></strong>
            <div class="box-tools pull-right" style="position: absolute !important;">
                <div id="buttons" class="box-tools pull-right">
                </div>
            </div>
        </div>


        <asp:GridView ID="gvlist" OnInit="gvlist_Init" OnRowDataBound="gvlist_RowDataBound" OnPreRender="gvlist_PeRender" OnSelectedIndexChanged="gvlist_SelectedIndexChanged" runat="server" Width="100%" CssClass="MyDataGrid" PagerStyle-CssClass="pager"
            HeaderStyle-CssClass="header" RowStyle-CssClass="rows" FooterStyle-BackColor="White" BorderColor="WhiteSmoke" CellPadding="10" >
        </asp:GridView>
       
    </form>



    <script type="text/javascript">

        function mngRequestStarted(ajaxManager, eventArgs) {
            if (eventArgs.get_eventTarget().indexOf("btnExport") >= 0) {
                eventArgs.EnableAjax = false;
            }
            else {
                document.body.setAttribute('background-color', 'black');
            }
        }
    </script>



</body>
</html>
