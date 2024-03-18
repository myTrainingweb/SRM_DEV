<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailVerification.aspx.cs" Inherits="PlusCP.Externals.EmailVerification" %>

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
         <img src="../Content/images/SRMLogo.png" style=" margin-left:8px;margin-top:5px; height:40px; margin-bottom:3px;" />
                    
        <hr style="background-color: #325FAB; border: 0px; min-height: 4px; margin-top: 4px;" />
    </div>

    <div class="col-md-12 showform" style="color:green; border-radius: 10px; border: 10px solid #f3f3f3; box-shadow: 0px 2px 2px rgba(0,0,0,0.3); padding: 25px; margin-left: 20%; margin-top: 0px; width: 700px;">
            <h1 style="text-align:center;">The email has been verified!</h1>
         <h4 style="text-align:center;color:black;">Thank you for verify you email address.</h4>
          <a href="http://localhost:61844/Home/Login" style="text-align:center;" id="Verified">Click Here to login</a>
    </div>
 
    <div class="tryagain" style="visibility:hidden"  ><span class="text-red"><b>Your request has expired.</b></span></div>
</body>
</html>

<script type="text/javascript">

    $(document).ready(function () {
        var IsVerified = "1";
        var tokenId = '';
        var tokenId = '<%= Request.QueryString["id"]%>';


        $.ajax({
            type: 'POST',
            url: '/Home/IsVerified',
            data: {
                tokenId: tokenId,
                IsVerified: IsVerified
            },
            success: function (data) {
                $('#lblMsg').empty();

            }

        });

    });

</script>