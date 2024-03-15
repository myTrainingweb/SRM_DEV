<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VendorPOEmail.aspx.cs" Inherits="PlusCP.Externals.VendorPOEmail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" runat="server">
<head runat="server">
    <link href="../Content/css/all.css" rel="stylesheet" />
    <link href="../Content/css/_all-skins.min.css" rel="stylesheet" />
    <link href="../Content/css/AdminLTE.min.css" rel="stylesheet" />
    <link href="../Content/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../Content/css/all.css" rel="stylesheet" />
    <link href="../Content/css/Site.css" rel="stylesheet" />
    <script src="../Scripts/jquery-3.3.1.js"></script>
    <script src="../Scripts/jquery.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <script src="../Scripts/adminlte.js"></script>

    <title>Plus Portal</title>

    <style>
        /* Custom styles for the grid */
        .gridview-container {
            width: 100%;
            margin: auto;
            font-family: Arial, sans-serif;
        }

        .gridview {
            width: 100%;
            border-collapse: collapse;
        }

            .gridview th, .gridview td {
                padding: 10px;
                border: 1px solid #ddd;
                text-align: left;
            }

            .gridview th {
                background-color: #f2f2f2;
                font-weight: bold;
            }

            .gridview tr:nth-child(even) {
                background-color: #f9f9f9;
            }

            .gridview tr:hover {
                background-color: #f2f2f2;
            }

        .button {
            width: 100px;
            padding: 12px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
            margin-top: 6px;
            margin-bottom: 16px;
            background-color: green;
            color: white;
        }
    </style>

</head>
<body runat="server">
    <div style="padding-top: 1px;">
        <%-- <img src="~/Content/Images/clogo2.jpg" style=" margin-left:5px; width:150px; height:50px;" />--%>

        <img src="../Content/images/SRMLogo.png" style="margin-left: 8px; margin-top: 5px; height: 40px; margin-bottom: 3px;" />

        <hr style="background-color: #325FAB; border: 0px; min-height: 4px; margin-top: 4px;" />
    </div>
    <div class="col-md-10 showform" style="border-radius: 10px; border: 10px solid #f3f3f3; box-shadow: 0px 2px 2px rgba(0,0,0,0.3); padding: 25px; margin-top: 0px; width: 100%;">
        <form id="form1" runat="server">

            <div class="gridview-container">
                <div style="vertical-align: middle; text-align: center; font-size: 20px; color: green">
                    <label>Vendor Submission Form</label>
                </div>

                <asp:GridView ID="dgvVendor" runat="server" CssClass="gridview" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField DataField="vendorName" HeaderText="Vendor" />
                        <asp:BoundField DataField="PONO" HeaderText="PO No." />
                        <asp:BoundField DataField="PartNo" HeaderText="Part No." />
                        <asp:BoundField DataField="Qty" HeaderText="Qty" />
                        <asp:BoundField DataField="DueDate" HeaderText="Due Date" />
                        <asp:BoundField DataField="Price" HeaderText="Price" />
                    </Columns>
                </asp:GridView>
                <br />
                <div>
                      <table runat="server" id="tblChange" style="margin-left: 45px;">
            <tr ">
                <td class="modal-title">
                    <h4 style="padding-bottom:10px;text-align:center;color:#47AE50;">Change PO Details</h4>
                </td>
            </tr>
         
             <tr> <td><label id="lblDueDate">New Due Date:</label></td></tr>
            <tr>
                <td class="form-group">
                    <input type="date" class="form-control" runat="server"  id="txtDueDate"  /></td>
            </tr>
            <tr> <td><label id="lblQty">New Qty:</label></td></tr>
            <tr>
                <td class="form-group">
                    <input type="text" class="form-control" runat="server"  id="txtQty"  /></td>
            </tr>
           
         <tr> <td><label id="lblPrice">New Price:</label></td></tr>
            <tr>
                <td class="form-group">
                    <input type="text" class="form-control" runat="server" id="txtPrice"  /></td>
            </tr>

        </table>
                </div>

                <div style="text-align: center; vertical-align: middle">
                    <asp:Button ID="btnSubmit" class="button" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
                </div>

            </div>
        </form>
    </div>
    <asp:Label ID="lblsignid" runat="server" ForeColor="White"></asp:Label>

    <div class="tryagain" style="visibility: hidden"><span class="text-red"><b>Your request has expired.</b></span></div>
</body>
</html>

<script type="text/javascript">

    //----For User Request----//
    var PO = '';
    var partNo = '';
    var dueDate = '';
    var qty = '';
    var price = '';
    var buyer = '';

    window.load = load();
    function load() {
        //$(".showform").hide();
        $(".showmessage").hide();
        $(".tryagain").hide();
      //var signid = '<%= Request.QueryString["PO"]%>';
      <%--  PO = document.getElementById("<%=lblPONo.ClientID %>").innerHTML;--%>


    }









    //$(".button").click(function () {
    //    var retCaptcha;
    //    var retVal = PasswordIsValid();
    //    if (retVal == true) {
    //        retCaptcha = ValidCaptcha();
    //    }

    //    if (retVal == true && retCaptcha == true) {
    //        var newPwd = $("#newPwd").val();
    //        var confirmPwd = $('#confirmPwd').val();

    //        $.ajax({
    //            type: 'POST',
    //            url: '/Home/ChangeExternalPassword',
    //            data: { newPwd: newPwd, confirmPwd: confirmPwd, signId: signId },
    //            success: function (data) {
    //                $('#lblMsg').empty();
    //                $("#newPwd").val = '';
    //                $('#confirmPwd').val = '';
    //                $('#lblMsg').append(data);
    //                if (data == "Old and new password cannot be same.") {
    //                    $('#lblMsg').removeClass('SeaGreenColor');
    //                }
    //                if (data == "Password has been reset.") {
    //                    $('#lblMsg').addClass('SeaGreenColor');
    //                    window.location.href = '/Home/Logout';
    //                }
    //                else
    //                    $('#lblMsg').removeClass('SeaGreenColor');
    //            },
    //            fail: function () {
    //                alert('fail');
    //            },
    //            error: function (r) {
    //                alert('error');
    //            }

    //        });
    //    }
    //});




</script>

