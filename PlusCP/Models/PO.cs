using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace PlusCP.Models
{
    public class PO
    {

        #region Fields
        //[Display(Name = "Part No.:")]
        //public string partNo { get; set; }
        [Display(Name = "Email :")]
        public string EmailId { get; set; }

        [Display(Name = "Buyer Id :")]
        public string BuyerId { get; set; }

        [Display(Name = "Buyer Company :")]
        public string BuyerCompany { get; set; }

        [Display(Name = "Buyer Name :")]
        public string BuyerName { get; set; }

        [Display(Name = "Line :")]
        public string Line { get; set; }

        [Display(Name = "Rel :")]
        public string Release { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }
        [Display(Name = "PO No.")]
        public string PONo { get; set; }

        [Display(Name = "VendorId")]
        public string VendorId { get; set; }

        [Display(Name = "Vendor")]
        public string Vendor { get; set; }

        [Display(Name = "Part No.")]
        public string PartNo { get; set; }

        [Display(Name = "Part Desc.")]
        public string PartDesc { get; set; }

        [Display(Name = "UOM")]
        public string UOM { get; set; }

        [Display(Name = "Order Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Due Date")]
        public string DueDate { get; set; }

        [Display(Name = "Qty")]
        public string Qty { get; set; }

        [Display(Name = "Contact Reason")]
        public string ContactReason { get; set; }

        [Display(Name = "Price")]
        public string Price { get; set; }

        [Display(Name = "OrgId")]
        public string OrgId { get; set; }

        // Supplier variables
        [Display(Name = "Current Qty")]
        public string CurrentQty { get; set; }

        [Display(Name = "Committed Qty")]
        public string CommitQty { get; set; }

        [Display(Name = "Proposed Qty")]
        public string ProposedQty { get; set; }

        [Display(Name = "Current Price")]
        public string CurrentPrice { get; set; }

        [Display(Name = "Committed Price")]
        public string CommitPrice { get; set; }

        [Display(Name = "Proposed Price")]
        public string ProposedPrice { get; set; }

        [Display(Name = "Current DueDate")]
        public string CurrentDueDate { get; set; }

        [Display(Name = "Committed DueDate")]
        public string CommitDueDate { get; set; }

        [Display(Name = "Proposed DueDate")]
        public string ProposedDueDate { get; set; }

        public string ReportTitle { get; set; }
        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }
        public string filterString { get; set; }
        public string ErrorMessage { get; set; }
        public List<Hashtable> lstPO { get; set; }
        public List<Hashtable> lstUpdate { get; set; }

        public List<object> lstMst = new List<object>();
        #endregion
        #region Methods 
        public bool GetList(DataTable dt, string POStatus)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            dt.Columns.Add("History");
            dt.Columns.Add("Message");

            filterString = " > " + POStatus + "";
            if (POStatus == "Late")
            {
                // Create a new DataTable to store the overdue records
                DataTable overdueDataTable = dt.Clone(); // Clone structure (columns)

                // Query to get rows where ArrivedDate is greater than DueDate
                var overdueRows = from row in dt.AsEnumerable()
                                  where Convert.ToDateTime(row["RcvDtl_ArrivedDate"]) > Convert.ToDateTime(row["PORel_DueDate"])
                                  select row;

                // Populate the overdueDataTable with the overdue rows
                foreach (var row in overdueRows)
                {
                    overdueDataTable.ImportRow(row);
                }

                if (oDAL.HasErrors)
                {
                    ErrorMessage = oDAL.ErrMessage;
                    return false;
                }
                else
                {
                    if (overdueDataTable.Rows.Count > 0)
                        lstPO = cCommon.ConvertDtToHashTable(overdueDataTable);

                    return true;
                }
            }
            else if (POStatus == "All Open")
            {
                if (oDAL.HasErrors)
                {
                    ErrorMessage = oDAL.ErrMessage;
                    return false;
                }
                else
                {
                    if (dt.Rows.Count > 0)
                        lstPO = cCommon.ConvertDtToHashTable(dt);

                    return true;
                }
            }
            else if (POStatus == "Pending")
            {
                DataTable table1 = new DataTable();
                string sql = @"select * from [SRM].[BuyerPO] ";
                table1 = oDAL.GetData(sql);
                // Create a new DataTable to store non-matching rows
                DataTable nonMatchingRows = dt.Clone(); // Clone the structure of dt2

                // Iterate through rows of dt2
                foreach (DataRow row2 in dt.Rows)
                {
                    // Check if the row in dt2 has a matching record in dt1
                    bool isMatching = table1.AsEnumerable()
                                          .Any(row1 => row2["POHeader_PONum"].ToString() == row1["PONum"].ToString() &&
                                                       row2["PODetail_POLine"].ToString() == row1["LineNo"].ToString() &&
                                                       row2["PORel_PORelNum"].ToString() == row1["RelNo"].ToString());

                    // If it's not matching, add the row to nonMatchingRows
                    if (!isMatching)
                    {
                        nonMatchingRows.ImportRow(row2);
                    }
                }

                if (oDAL.HasErrors)
                {
                    ErrorMessage = oDAL.ErrMessage;
                    return false;
                }
                else
                {
                    if (nonMatchingRows.Rows.Count > 0)
                        lstPO = cCommon.ConvertDtToHashTable(nonMatchingRows);

                    return true;
                }
            }
            else if (POStatus == "Awaiting Response")
            {
                DataTable table1 = new DataTable();
                string sql = @"select * from [SRM].[BuyerPO]
Where GUID NOT IN(Select GUID from [SRM].[VendorCommunication]) ";
                table1 = oDAL.GetData(sql);
                // Create a new datatable to store matching rows
                DataTable matchingRows = dt.Clone(); // Clone the structure of dt2
                if (table1.Rows.Count > 0)
                {
                    foreach (DataRow row1 in table1.Rows)
                    {
                        string PONo = (string)row1["PONum"];
                        string PartNo = (string)row1["PartNo"];
                        string LineNo = (string)row1["LineNo"];
                        string RelNo = (string)row1["RelNo"];
                        // Check if the parts match any row in dt2
                        DataRow[] foundRows = dt.Select($"POHeader_PONum = '{PONo}' AND PODetail_POLine = '{LineNo}' AND PORel_PORelNum = '{RelNo}' AND PODetail_PartNum = '{PartNo}' ");

                        if (foundRows.Length > 0)
                        {
                            foreach (DataRow foundRow in foundRows)
                            {
                                matchingRows.ImportRow(foundRow);
                            }
                        }

                    }

                }
                if (oDAL.HasErrors)
                {
                    ErrorMessage = oDAL.ErrMessage;
                    return false;
                }
                else
                {
                    if (matchingRows.Rows.Count > 0)
                        lstPO = cCommon.ConvertDtToHashTable(matchingRows);

                    return true;
                }
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstPO = cCommon.ConvertDtToHashTable(dt);

                return true;
            }

        }

        public void InsertBuyerCommunication(string POSender, string POReceiever, string POSubject, string POBody, string GUID, string POCreatedBy)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string sql = @"INSERT INTO [SRM].[BuyerCommunication]
           ([Sender]
           ,[Receiver]
           ,[Subject]
           ,[Body]
           ,[GUID]
           ,[CreatedBy]
)
     VALUES
           ('<Sender>'
           ,'<Receiver>'
           ,'<Subject>'
           ,'<Body>'
           ,'<GUID>'
           ,'<CreatedBy>'
)";

            sql = sql.Replace("<Sender>", POSender);
            sql = sql.Replace("<Receiver>", POReceiever);
            sql = sql.Replace("<Subject>", POSubject);
            sql = sql.Replace("<Body>", POBody);
            sql = sql.Replace("<GUID>", GUID.ToString());
            sql = sql.Replace("<CreatedBy>", POCreatedBy);

            oDAL.Execute(sql);
        }

        public void AddInBuyers(string PONo, string POline, string PORelNo, string POPartNo, string POpartDesc, string POQty,
            string POprice, string POUOM, string PODueDate, string POOrderDate, string POBuyer,
            string POVendorId, string POVendorName, string POVendorEmail, string POBuyerId, string POContactReason, string POCreatedBy, string newGUID)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string sql = @"INSERT INTO [SRM].[BuyerPO]
           ([PONum]
           ,[LineNo]
           ,[RelNo]
           ,[PartNo]
           ,[PartDesc]
           ,[Qty]
           ,[Price]
           ,[UOM]
           ,[DueDate]
           ,[OrderDate]
           ,[POStatus]
           ,[Buyer]
           ,[VendorId]
           ,[VendorName]
           ,[VendorEmail]
           ,[BuyerId]
           ,[ContactReason]
           ,[OrgId]
           ,[GUID]
           ,[CreatedBy]
		   )
     VALUES
           ('<PONum>'
           ,'<LineNo>'
           ,'<RelNo>'
           ,'<PartNo>'
           ,'<PartDesc>'
           ,'<Qty>'
           ,'<Price>'
           ,'<UOM>'
           ,'<DueDate>'
           ,'<OrderDate>'
           ,'<POStatus>'
           ,'<Buyer>'
           ,'<VendorId>'
           ,'<VendorName>'
           ,'<VendorEmail>'
           ,'<BuyerId>'
           ,'<ContactReason>'
           ,'<OrgId>'
           ,'<GUID>'
           ,'<CreatedBy>'
		   )";

            DateTime ConvertedDueDate = DateTime.Parse(PODueDate);
            DateTime ConvertedOrderDate = new DateTime();
            if (!string.IsNullOrEmpty(OrderDate))
                ConvertedOrderDate = DateTime.Parse(POOrderDate);

            double Qtyvalue = Convert.ToDouble(POQty);
            double PriceValue = Convert.ToDouble(POprice);
            int POFinalPrice = (int)PriceValue;

            int Quantity = (int)Qtyvalue;


            sql = sql.Replace("<PONum>", PONo);
            sql = sql.Replace("<LineNo>", POline);
            sql = sql.Replace("<RelNo>", PORelNo);
            sql = sql.Replace("<PartNo>", POPartNo);
            sql = sql.Replace("<PartDesc>", POpartDesc);
            sql = sql.Replace("<Qty>", Qtyvalue.ToString());
            sql = sql.Replace("<Price>", POFinalPrice.ToString());
            sql = sql.Replace("<UOM>", POUOM);
            sql = sql.Replace("<DueDate>", ConvertedDueDate.ToString());
            sql = sql.Replace("<OrderDate>", "");
            sql = sql.Replace("<POStatus>", "New");
            sql = sql.Replace("<Buyer>", POBuyer);
            sql = sql.Replace("<VendorId>", POVendorId);
            sql = sql.Replace("<VendorName>", POVendorName);
            sql = sql.Replace("<VendorEmail>", POVendorEmail);

            sql = sql.Replace("<BuyerId>", POBuyerId);
            sql = sql.Replace("<ContactReason>", POContactReason);
            sql = sql.Replace("<OrgId>", "1");
            sql = sql.Replace("<GUID>", newGUID.ToString());
            sql = sql.Replace("<CreatedBy>", POCreatedBy);

            oDAL.Execute(sql);
        }


        public bool GetUpdateData()
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            DataTable dt = new DataTable();

            string sql = "";
            sql = @"Select B.GUID, CONCAT(B.PONum,'-', B.[LineNo],'-', B.RelNo) PONo, B.PartNo, B.PartDesc,B.VendorName, B.Qty CurrentQty, B.Price CurrentPrice,B.DueDate CurrentDueDate, V.Qty CommitQty, V.Price CommitPrice,
V.DueDate CommitDueDate
from [SRM].[BuyerPO] B
LEFT JOIN [SRM].[VendorCommunication] V ON B.GUID = V.GUID
 ";

            dt = oDAL.GetData(sql);
            dt.Columns.Add("Acction");


            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstUpdate = cCommon.ConvertDtToHashTable(dt);

                return true;
            }
        }

        public void UpdateHasActionAHR(string Action, string GUID)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            DataTable dt = new DataTable();

            string sql = "";
            sql = @"Update [SRM].[BuyerPO] SET HasAction = '<HasAction>' Where GUID = '<GUID>' ";

            sql = sql.Replace("<HasAction>", Action);
            sql = sql.Replace("<GUID>", GUID);

            oDAL.Execute(sql);
            sql = @"SELECT B.PONum, B.[LineNo], B.relNo, B.PartNo, V.Qty, V.Price, V.DueDate, B.CreatedBy from [SRM].[BuyerPO] B
                    Inner Join[SRM].[VendorCommunication] V ON B.GUID = V.GUID
                    WHERE V.GUID = '<GUID>'";
            sql = sql.Replace("<GUID>", GUID);
            dt = oDAL.GetData(sql);
            string vPONo = "";
            string vLine = "";
            string vRel = "";
            string vPart = "";
            string vQty = "";
            string vPrice = "";
            string vDueDate = "";
            string vCreatedBy = "";
            if (dt.Rows.Count > 0)
            {
                 vPONo = Convert.ToString(dt.Rows[0]["PONum"]);
                 vLine = Convert.ToString(dt.Rows[0]["LineNo"]);
                 vRel = Convert.ToString(dt.Rows[0]["relNo"]);
                 vPart = Convert.ToString(dt.Rows[0]["PartNo"]);
                 vQty = Convert.ToString(dt.Rows[0]["Qty"]);
                 vPrice = Convert.ToString(dt.Rows[0]["Price"]);
                 vDueDate = Convert.ToString(dt.Rows[0]["DueDate"]);
                 vCreatedBy = Convert.ToString(dt.Rows[0]["CreatedBy"]);

                string vmessage = "";
                if (Action == "Accept")
                    vmessage = "You request of this PO " + vPONo + "-" + vLine + "-" + vRel + " has been Accepted";
                else if (Action == "Reject")
                    vmessage = "You request of this PO " + vPONo + "-" + vLine + "-" + vRel + " has been Rejected";
                else if (Action == "Hold")
                    vmessage = "You request of this PO " + vPONo + "-" + vLine + "-" + vRel + " has been Hold";

                sendVendorEmailAHR(vCreatedBy, vPONo, vPart, vQty, vDueDate, vPrice, vmessage, GUID);
            }
           
          
        }


        public void sendVendorEmail(string EuserName, string EPONo, string EpartNo, string EQty, string EDueDate, string EPrice, string Emessage, string GUID)
        {
            // Sender's email address and password
            string senderEmail = "yousufdev4@gmail.com";
            string senderPassword = "qvav wxwd wcho wofj";


            var subject = "PO Information against this PONo. " + EPONo + "";
            // Recipient's email address
            string recipientEmail = "yousufsaleemshahani1994@gmail.com";

            // SMTP server settings (e.g., for Gmail)
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587; // Port 587 for Gmail SMTP
            bool enableSsl = true;

            // Create a new MailMessage
            MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;



            //URL
            string Accepturl = @"http://localhost:61844/Externals/VendorPOEmail.aspx?GUID=<GUID>&Action=Accept";
            string Changeurl = @"http://localhost:61844/Externals/VendorPOEmail.aspx?GUID=<GUID>&Action=Change";
            Accepturl = Accepturl.Replace("<GUID>", GUID.ToString());
            Changeurl = Changeurl.Replace("<GUID>", GUID.ToString());


            // HTML body containing the form
            string htmlBody = $@"<!DOCTYPE html>
                            <html>
                            <head>
                                <title>Email Body</title>
                            </head>
                            <body>
                                <h2>Purchase Order Details</h2>
                                <table style=""border - collapse: collapse; border: 1px solid #000;"">
                                    <tr style=""border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Buyer:</strong></td>
                                        <td style=""border: 1px solid #000;""><BuyerName></td>
                                    </tr>
                                    <tr style=""border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>PO No.:</strong></td>
                                        <td style=""border: 1px solid #000;""><PONo></td>
                                    </tr>
                                    <tr style=""border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Part No.:</strong></td>
                                        <td style=""border: 1px solid #000;""><PartNo></td>
                                    </tr>
                                    <tr style=""border - collapse: collapse; border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Qty:</strong></td>
                                        <td style=""border: 1px solid #000;""><Qty></td>
                                    </tr>
                                    <tr style=""border - collapse: collapse; border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Due Date:</strong></td>
                                        <td style=""border: 1px solid #000;""><DueDate></td>
                                    </tr>
                                    <tr style=""border - collapse: collapse; border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Price:</strong></td>
                                        <td style=""border: 1px solid #000;""><Price></td>
                                    </tr>
                                     <tr style=""border - collapse: collapse; border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Message:</strong></td>
                                        <td style=""border: 1px solid #000;""><Message></td>
                                    </tr>
                                </table>
                                <p>                                
                                    <a href="" <Accept_URL>""  target=""_blank"" style=""display: inline-block; padding: 10px 15px; background-color: #4CAF50; color: white; text-align: center; text-decoration: none; border: none; border-radius: 5px; cursor: pointer;"">Accept</a>
                                    <a href="" <Change_URL>""  target=""_blank"" style=""display: inline-block; padding: 10px 15px; background-color: gray; color: white; text-align: center; text-decoration: none; border: none; border-radius: 5px; cursor: pointer;"">Change</a>
                                </p>
                            </body>
                            </html>";

            htmlBody = htmlBody.Replace("<BuyerName>", EuserName);
            htmlBody = htmlBody.Replace("<PONo>", EPONo);
            htmlBody = htmlBody.Replace("<PartNo>", EpartNo);
            htmlBody = htmlBody.Replace("<Qty>", EQty);
            htmlBody = htmlBody.Replace("<DueDate>", EDueDate);
            htmlBody = htmlBody.Replace("<Price>", EPrice);
            htmlBody = htmlBody.Replace("<Message>", Emessage);
            htmlBody = htmlBody.Replace("<Accept_URL>", Accepturl);
            htmlBody = htmlBody.Replace("<Change_URL>", Changeurl);

            mailMessage.Body = htmlBody;

            // Create a SmtpClient instance
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
            try
            {
                // Send the email
                InsertBuyerCommunication(senderEmail, recipientEmail, subject, htmlBody, GUID.ToString(), EuserName);
                smtpClient.Send(mailMessage);

            }
            catch (Exception ex)
            {

            }
        }
        public void sendVendorEmailAHR(string EuserName, string EPONo, string EpartNo, string EQty, string EDueDate, string EPrice, string Emessage, string GUID)
        {
            // Sender's email address and password
            string senderEmail = "yousufdev4@gmail.com";
            string senderPassword = "qvav wxwd wcho wofj";


            var subject = "PO Information against this PONo. " + EPONo + "";
            // Recipient's email address
            string recipientEmail = "yousufsaleemshahani1994@gmail.com";

            // SMTP server settings (e.g., for Gmail)
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587; // Port 587 for Gmail SMTP
            bool enableSsl = true;

            // Create a new MailMessage
            MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;






            // HTML body containing the form
            string htmlBody = $@"<!DOCTYPE html>
                            <html>
                            <head>
                                <title>Email Body</title>
                            </head>
                            <body>
                                <h2>Purchase Order Details</h2>
                                <table style=""border - collapse: collapse; border: 1px solid #000;"">
                                    <tr style=""border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Buyer:</strong></td>
                                        <td style=""border: 1px solid #000;""><BuyerName></td>
                                    </tr>
                                    <tr style=""border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>PO No.:</strong></td>
                                        <td style=""border: 1px solid #000;""><PONo></td>
                                    </tr>
                                    <tr style=""border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Part No.:</strong></td>
                                        <td style=""border: 1px solid #000;""><PartNo></td>
                                    </tr>
                                    <tr style=""border - collapse: collapse; border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Qty:</strong></td>
                                        <td style=""border: 1px solid #000;""><Qty></td>
                                    </tr>
                                    <tr style=""border - collapse: collapse; border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Due Date:</strong></td>
                                        <td style=""border: 1px solid #000;""><DueDate></td>
                                    </tr>
                                    <tr style=""border - collapse: collapse; border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Price:</strong></td>
                                        <td style=""border: 1px solid #000;""><Price></td>
                                    </tr>
                                     <tr style=""border - collapse: collapse; border: 1px solid #000;"">
                                        <td style=""border: 1px solid #000;""><strong>Message:</strong></td>
                                        <td style=""border: 1px solid #000;""><Message></td>
                                    </tr>
                                </table>
                             
                            </body>
                            </html>";

            htmlBody = htmlBody.Replace("<BuyerName>", EuserName);
            htmlBody = htmlBody.Replace("<PONo>", EPONo);
            htmlBody = htmlBody.Replace("<PartNo>", EpartNo);
            htmlBody = htmlBody.Replace("<Qty>", EQty);
            htmlBody = htmlBody.Replace("<DueDate>", EDueDate);
            htmlBody = htmlBody.Replace("<Price>", EPrice);
            htmlBody = htmlBody.Replace("<Message>", Emessage);

            mailMessage.Body = htmlBody;

            // Create a SmtpClient instance
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
            try
            {
                // Send the email
                InsertBuyerCommunication(senderEmail, recipientEmail, subject, htmlBody, GUID.ToString(), EuserName);
                smtpClient.Send(mailMessage);

            }
            catch (Exception ex)
            {

            }
        }

        public void UpdatePropose(string uGUID, string uQty, string uPrice, string uDueDate, string uMessage, string userName, string uPONo, string uPartNo)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            //DateTime udate = DateTime.ParseExact(uDueDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            string sql = @"UPDATE [SRM].[BuyerPO]
                           SET 
                               [Qty] = '<Qty>'
                              ,[Price] = '<Price>'
                              ,[DueDate] = '<DueDate>'
                              ,[POStatus] = '<POStatus>'
                              ,[HasAction] = '<HasAction>'
                         WHERE GUID = '<GUID>'";

            sql = sql.Replace("<Qty>", uQty);
            sql = sql.Replace("<Price>", uPrice);
            sql = sql.Replace("<DueDate>", uDueDate.ToString());
            sql = sql.Replace("<POStatus>", "New");
            sql = sql.Replace("<HasAction>", "Propose");
            sql = sql.Replace("<GUID>", uGUID);
            oDAL.Execute(sql);


            sendVendorEmail(userName, uPONo, uPartNo, uQty, uDueDate, uPrice, uMessage, uGUID);

        }



        #endregion
    }
}