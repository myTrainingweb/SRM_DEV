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

    <title>SRM Portal</title>
    <style>
        /* Style all input fields */
        input {
            width: 100%;
            padding: 12px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
            margin-top: 6px;
            margin-bottom: 16px;
        }

        button {
            width: 50%;
            padding: 12px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
            margin-top: 6px;
            margin-bottom: 16px;
        }
        /* Style the submit button */
        input[type=submit] {
            background-color: #4CAF50;
            color: white;
        }

        /* Style the container for inputs */
        .container {
            background-color: #f1f1f1;
            padding: 20px;
        }

        /* The message box is shown when the user clicks on the password field */
        #message {
            /*display: none;*/
            background: #f1f1f1;
            color: #000;
            position: relative;
            padding: 20px;
            margin-top: 10px;
        }

            #message p {
                padding: 7px 40px;
                font-size: 15px;
                font-family: "Helvetica Neue",Helvetica,Arial,sans-serif
            }

        /* Add a green text color and a checkmark when the requirements are right */
        .valid {
            color: green;
        }

            .valid:before {
                position: relative;
                left: -35px;
                content: "✔";
            }

        /* Add a red text color and an "x" when the requirements are wrong */
        .invalid {
            color: red;
        }

            .invalid:before {
                position: relative;
                left: -35px;
                content: "✖";
            }

        .SeaGreenColor {
            color: #00b3b3;
        }
    </style>


</head>
<body runat="server" >
    <div style="padding-top: 1px;">
        <%-- <img src="~/Content/Images/clogo2.jpg" style=" margin-left:5px; width:150px; height:50px;" />--%>
         <img src="../Content/images/SRMLogo.png" style=" margin-left:8px;margin-top:5px; height:40px; margin-bottom:3px;" />
                    
        <hr style="background-color: #325FAB; border: 0px; min-height: 4px; margin-top: 4px;" />
    </div>
    <div class="col-md-12 showform" style="border-radius: 10px; border: 10px solid #f3f3f3; box-shadow: 0px 2px 2px rgba(0,0,0,0.3); padding: 25px; margin-left: 30%; margin-top: 0px; width: 700px;">
    
        <table style="margin-left: 45px;">
    <tr>
        <td class="modal-title" colspan="6">
            <h4 style="padding-bottom:10px;text-align:center;color:#47AE50;">Vendor Submission Form</h4>
        </td>
    </tr>
    <tr>
        <td>
            <label id="lblPO">PO No.:</label>
        </td>
        <td class="form-group">
            <input type="text" class="form-control" maxlength="16" id="PONo" style="width: 200px;" />
        </td>
        <td style="width: 50px;"></td> <!-- Add space between fields -->
        <td>
            <label id="lblPartNo">Part No.:</label>
        </td>
        <td class="form-group">
            <input type="text" class="form-control" maxlength="16" id="PartNo" style="width: 200px;" />
        </td>
    </tr>
    <tr>
        <td>
            <label id="lblBuyer">Buyer:</label>
        </td>
        <td class="form-group">
            <input type="text" class="form-control" maxlength="16" id="Buyer" style="width: 200px;" />
        </td>
        <td style="width: 50px;"></td> <!-- Add space between fields -->
        <td>
            <label id="lblDueDate">Due Date:</label>
        </td>
        <td class="form-group">
            <input type="text" class="form-control" maxlength="16" id="DueDate" style="width: 200px;" />
        </td>
    </tr>
    <tr>
        <td>
            <label id="lblQty">Qty:</label>
        </td>
        <td class="form-group">
            <input type="text" class="form-control" maxlength="16" id="Qty" style="width: 200px;" />
        </td>
        <td style="width: 50px;"></td> <!-- Add space between fields -->
        <td>
            <label id="lblPrice">Price:</label>
        </td>
        <td class="form-group">
            <input type="text" class="form-control" maxlength="16" id="Price" style="width: 200px;" />
        </td>
    </tr>
    <tr>
        <td colspan="6" style="text-align: center;">
            <button type="button" class="button" id="btndone" style="background-color: #53bd5c; color: white;">Submit</button>
        </td>
    </tr>
    <tr>
        <td colspan="4" class="label" style="text-align:center;">
            <label id="lblMsg" class="optionMsg"></label>
        </td>
    </tr>
</table>


    </div>
     <asp:Label ID="lblPONo"  runat="server" ForeColor="White"></asp:Label>
     <asp:Label ID="lblsBuyer"  runat="server" ForeColor="White"></asp:Label>
     <asp:Label ID="lblsPartNo"  runat="server" ForeColor="White"></asp:Label>
     <asp:Label ID="lblsDueDate"  runat="server" ForeColor="White"></asp:Label>
     <asp:Label ID="lblsQty"  runat="server" ForeColor="White"></asp:Label>
     <asp:Label ID="lblsPrice"  runat="server" ForeColor="White"></asp:Label>
    <div class="tryagain" style="visibility:hidden"  ><span class="text-red"><b>Your request has expired.</b></span></div>
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
        PO = document.getElementById("<%=lblPONo.ClientID %>").innerHTML;
        partNo = document.getElementById("<%=lblsPartNo.ClientID %>").innerHTML;

        buyer = document.getElementById("<%=lblsBuyer.ClientID %>").innerHTML;
        dueDate = document.getElementById("<%=lblsDueDate.ClientID %>").innerHTML;
        qty = document.getElementById("<%=lblsQty.ClientID %>").innerHTML;
        price = document.getElementById("<%=lblsPrice.ClientID %>").innerHTML;



        $("#PONo").val(PO);
        $("#PartNo").val(partNo);
        $("#Buyer").val(buyer);
        $("#DueDate").val(dueDate);
        $("#Qty").val(qty);
        $("#Price").val(price);

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

