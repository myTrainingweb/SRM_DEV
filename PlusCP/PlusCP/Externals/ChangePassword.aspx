<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="PlusCP.Externals.ChangePassword" %>

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
            width: 100%;
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
     <a href="https://www.reconext.com/"  target="_blank">
         <img src="../Content/images/logo.png" style=" margin-left:8px;margin-top:5px; height:40px; margin-bottom:3px;" />
                    </a>
        <hr style="background-color: #325FAB; border: 0px; min-height: 4px; margin-top: 4px;" />
    </div>
    <div class="col-md-10 showform" style="border-radius: 10px; border: 10px solid #f3f3f3; box-shadow: 0px 2px 2px rgba(0,0,0,0.3); padding: 25px; margin-left: 30%; margin-top: 0px; width: 350px;visibility:hidden;">
        <table style="margin-left: 45px;">
            <tr ">
                <td class="modal-title">
                    <h4 style="padding-bottom:10px;text-align:center;color:#47AE50;">Reset Password</h4>
                </td>
               
            </tr>
            <tr> <td><label id="lblnewpwd">New Password:</label></td></tr>
            <tr>
               
                <td  class="form-group">
                    <input type="password" class="form-control"  maxlength="16" id="newPwd" style="width: 200px;" />
                </td>
            </tr>
             <tr> <td><label id="lblconfirmpwd">Confirm Password:</label></td></tr>
            <tr>
                <td class="form-group">
                    <input type="password" class="form-control"  maxlength="16" id="confirmPwd" style="width: 200px;" /></td>
            </tr>
             <tr>
                  <td class="form-group">
  </td></tr> 
              <tr>
                <td class="form-group">
        <input type="text" id="txtCaptcha"  style="text-align: center; border: none; font-weight: bold; font-size: 30px; font-family:Helvetica Neue,Helvetica,Arial,sans-serif !important; color:#325FAB; background-color:#e0ffd1; border:medium; border-color:#c2ffa3; width:200px; margin-bottom:2px;" readonly/>  
         </td></tr>
            <tr>
                <td class="form-group" style="padding-top:15px;">
       Input symbols 
                </td></tr>
              <tr>
                <td class="form-group">
        <input type="text" id="txtCompare" class="form-control" />  
                    </td></tr>
            <%--<tr>
                <td>
                    <input type="button" class="form-control" id="btnrefresh" value="Refresh" onclick="GenerateCaptcha();" style="width:90px; height:30px; font-size: 14px;  font-weight: bold; margin-left:55px; margin-bottom:10px"/>  
                    </td></tr>--%>
           <tr>
                <td>
            <%--<input id="btnValid" type="button" value="Check" onclick="alert(ValidCaptcha());" />--%> 
                     </td></tr>
            <tr>
                <td>
                    <button type="button" class="button" id="btndone" style="background-color: #53bd5c; color: white;">Submit</button>
                </td>
            </tr>
            <tr>
                <td class="label" style="text-align:center;">
                    <label id="lblMsg" class="optionMsg" />
                </td>
            </tr>
        </table>
    </div>
     <asp:Label ID="lblsignid"  runat="server" ForeColor="White"></asp:Label>
    <div class="col-md-2  showmessage  " id="message" style="margin-left:10px; margin-top: 50px; width: 500px; height:370px; visibility:hidden">
        <h3>Password must contain the following:</h3>
        <p id="letter" class="invalid">A lowercase letter</p>
        <p id="capital" class="invalid">A capital (uppercase) letter</p>
        <p id="number" class="invalid">A number</p>
        <p id="length" class="invalid">Minimum 8 characters (Max 15)</p>
        <p id="specialChar" class="invalid">Special characters</p>
    </div>
    <div class="tryagain" style="visibility:hidden"  ><span class="text-red"><b>Your request has expired.</b></span></div>
</body>
</html>

<script type="text/javascript">
    window.onload = GenerateCaptcha;
    //----For User Request----//
    var signId = '';
    window.load = load();
    function load() {
        $(".showform").hide();
        $(".showmessage").hide();
        $(".tryagain").hide();
      //var signid = '<%= Request.QueryString["id"]%>';
        signId = document.getElementById("<%=lblsignid.ClientID %>").innerHTML;
        $.ajax({
            type: 'POST',
            url: '/Home/GetExternalRequest',
            data: { signId: signId },
            success: function (data) {
                $('#lblMsg').empty();
                if (data == "Success") {
                    $(".tryagain").hide();
                    $(".showform").show();
                    $(".showform").css("visibility", "visible");
                    $(".showmessage").show();
                    $(".showmessage").css("visibility", "visible");
                }
                else {
                    $(".showform").hide();
                    $(".showmessage").hide();
                    $(".tryagain").show();
                    $(".tryagain").css("visibility", "visible");
                }
            },
            fail: function () {
                alert('fail');
            },
            error: function (r) {
                alert('error');
            }

        });
    }
    //----END-----//

    //---For Password Handling----//
    var isLowerCase;
    var isUpperCase;
    var isNumber;
    var isMaxLenth;
    var isSpecialChar;
    var letter = document.getElementById("letter");
    var capital = document.getElementById("capital");
    var number = document.getElementById("number");
    var length = document.getElementById("length");
    var specialChar = document.getElementById("specialChar");

    var newPwd = document.getElementById("newPwd");

    $("#newPwd").on('keypress', function () {
        if (event.keyCode == 32) {
            return false;
        }
    });

    $("#newPwd").on('keyup', function () {

        validatePassword();
    });

    function validatePassword() {
        // Validate lowercase letters
        var lowerCaseLetters = /[a-z]/g;
        if (newPwd.value.match(lowerCaseLetters)) {
            letter.classList.remove("invalid");
            letter.classList.add("valid");
            isLowerCase = true;
        } else {
            isLowerCase = false;
            letter.classList.remove("valid");
            letter.classList.add("invalid");
        }

        //  // Validate capital letters
        var upperCaseLetters = /[A-Z]/g;
        if (newPwd.value.match(upperCaseLetters)) {
            capital.classList.remove("invalid");
            capital.classList.add("valid");
            isUpperCase = true;
        } else {
            isUpperCase = false
            capital.classList.remove("valid");
            capital.classList.add("invalid");
        }

        // Validate numbers
        var numbers = /[0-9]/g;
        if (newPwd.value.match(numbers)) {
            number.classList.remove("invalid");
            number.classList.add("valid");
            isNumber = true;
        } else {
            isNumber = false;
            number.classList.remove("valid");
            number.classList.add("invalid");
        }

        // Validate length
        if (newPwd.value.length >= 8) {
            length.classList.remove("invalid");
            length.classList.add("valid");
            isMaxLenth = true;
        } else {
            isMaxLenth = false;
            length.classList.remove("valid");
            length.classList.add("invalid");
        }

        var specialChars = /[?=.*[!#$%&?@]/;
        if (newPwd.value.match(specialChars)) {
            specialChar.classList.remove("invalid");
            specialChar.classList.add("valid");
            isSpecialChar = true;
        } else {
            isSpecialChar = false;
            specialChar.classList.remove("valid");
            specialChar.classList.add("invalid");
        }
    }

    //----END----//


    //---- For Validating User Input-----//
    function PasswordIsValid() {

        if (isLowerCase == false) {
            document.getElementById("message").style.display = "block";
            return false
        }

        if (isUpperCase == false) {
            document.getElementById("message").style.display = "block";
            return false
        }

        if (isNumber == false) {
            document.getElementById("message").style.display = "block";
            return false
        }

        if (isMaxLenth == false) {
            document.getElementById("message").style.display = "block";
            return false
        }
        if (isSpecialChar == false) {
            document.getElementById("message").style.display = "block";
            return false
        }
        var newPwd = $("#newPwd").val();
        var confirmPwd = $('#confirmPwd').val();

        
        

        if (newPwd.length == 0) {
            $('#lblMsg').empty();
            $('#lblMsg').append('Enter new password.');
            return false;
        }

        if (confirmPwd.length == 0) {
            $('#lblMsg').empty();
            $('#lblMsg').append('Enter confirm password.');
            return false;
        }


        if (newPwd != confirmPwd) {
            $('#lblMsg').empty();
            $('#lblMsg').append('Password does not match.');
            return false;
        }

        //if (str1.length == 0) {
        //    $('#lblMsg').empty();
        //    $('#lblMsg').append('Enter Captcha.');
        //    return false;
        //}

        //if (str1 != str2) {
        //    $('#lblMsg').empty();
        //    $('#lblMsg').append('Enter Valid Captcha.');
        //    return false;
        //}

        else {
            $('#lblMsg').empty();
            return true;
        }
    }

    $(".button").click(function () {
        var retCaptcha;
        var retVal = PasswordIsValid();
        if (retVal == true) {
            retCaptcha = ValidCaptcha();
        }
       
        if (retVal == true && retCaptcha == true) {
            var newPwd = $("#newPwd").val();
            var confirmPwd = $('#confirmPwd').val();

            $.ajax({
                type: 'POST',
                url: '/Home/ChangeExternalPassword',
                data: { newPwd: newPwd, confirmPwd: confirmPwd, signId: signId },
                success: function (data) {
                    $('#lblMsg').empty();
                    $("#newPwd").val = '';
                    $('#confirmPwd').val = '';
                    $('#lblMsg').append(data);
                    if (data == "Old and new password cannot be same.") {
                        $('#lblMsg').removeClass('SeaGreenColor');
                    }
                    if (data == "Password has been reset.") {
                        $('#lblMsg').addClass('SeaGreenColor');
                        window.location.href = '/Home/Logout';
                    }
                    else
                        $('#lblMsg').removeClass('SeaGreenColor');
                },
                fail: function () {
                    alert('fail');
                },
                error: function (r) {
                    alert('error');
                }

            });
        }
    });

    /* Function to Generat Captcha */

    $('#txtCaptcha').bind("cut copy paste", function (e) {
        e.preventDefault();
    });


    function GenerateCaptcha() {
        var chr1 = Math.ceil(Math.random() * 10) + '';
        var chr2 = Math.ceil(Math.random() * 10) + '';
        var chr3 = Math.ceil(Math.random() * 10) + '';

        var str = new Array(3).join().replace(/(.|$)/g, function () { return ((Math.random() * 36) | 0).toString(36)[Math.random() < .4 ? "toString" : "toLowerCase"](); });
        var captchaCode = str + chr1 + '' + chr2 + '' + chr3;
        document.getElementById("txtCaptcha").value = captchaCode
    }

    /* Validating Captcha Function */
    function ValidCaptcha() {
        var str1 = removeSpaces(document.getElementById('txtCaptcha').value);
        var str2 = removeSpaces(document.getElementById('txtCompare').value);

        if (str2.length == 0) {
            $('#lblMsg').empty();
            $('#lblMsg').append('Please, enter a valid captcha.');
            return false;
        }

        if (str1 != str2) {
            $('#lblMsg').empty();
            $('#lblMsg').append('Please, enter a valid captcha.');
            return false;
        }


        if (str1 == str2)
            return true;
        
    }

    

    /* Remove spaces from Captcha Code */
    function removeSpaces(string) {
        return string.split(' ').join('');
    } 
    //----- END-------//
</script>
