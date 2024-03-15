using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlusCP.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PlusCP.Controllers
{
    public class POController : Controller
    {
        // GET: PO
        PO oPO = new PO();
        public ActionResult Option()
        {
            oPO = new PO();
            return View(oPO);
        }

        public ActionResult Index(string RptCode, string menuTitle)
        {
            oPO = new PO();
            TempData["ReportTitle"] = menuTitle;
            TempData["RptCode"] = RptCode;
            ViewBag.ReportTitle = menuTitle;
            return View(oPO);

        }



        //public async Task<JsonResult> GetList(string POStatus)
        //{
        //    DataTable dt = new DataTable();
        //    string menuTitle = string.Empty;
        //    string RptCode;
        //    string apiUrl = "https://centralusdtapp33.epicorsaas.com/saas838/api/v1/BaqSvc/OpenPO_BuyerSupplier";
        //    string username = "ApiUser";
        //    string password = "Ap1User1234";

        //    HttpClient client = new HttpClient();
        //    client.BaseAddress = new Uri(apiUrl);
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "vP0OfQwi8m8eKqgsxLcIMukSEuDpJKXnAOvq8eIc4xd8q");
        //    //Set Basic Auth

        //    var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);
        //    HttpResponseMessage getdata = await client.GetAsync("OpenPO_BuyerSupplier");
        //    if (getdata.IsSuccessStatusCode == true)
        //    {
        //        string jsonstring = getdata.Content.ReadAsStringAsync().Result;
        //        dt = Tabulate(jsonstring);

        //        oPO.GetList(dt, POStatus);

        //        var jsonResult = Json(oPO, JsonRequestBehavior.AllowGet);
        //        jsonResult.MaxJsonLength = int.MaxValue;
        //        //LOAD MRU & LOG QUERY
        //        if (TempData["ReportTitle"] != null && TempData["RptCode"] != null)
        //        {
        //            menuTitle = TempData["ReportTitle"] as string;
        //            RptCode = TempData["RptCode"].ToString();
        //            TempData.Keep();
        //            //cLog oLog = new cLog();
        //            //oLog.SaveLog(menuTitle, Request.Url.PathAndQuery, RptCode);
        //        }
        //        return jsonResult;
        //    }
        //    else
        //    {
        //        return Json("");
        //    }
        //}

        public async Task<JsonResult> GetList(string POStatus)
        {
            DataTable dt = new DataTable();
            string menuTitle = string.Empty;
            string RptCode;

            var client = new RestClient("https://centralusdtapp33.epicorsaas.com");
            var request = new RestRequest("/saas838/api/v1/BaqSvc/OpenPO_BuyerSupplier", Method.Get);
            // Add basic authentication header
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("ApiUser:Ap1User1234")));

            var response = client.Execute(request);

            if (response.IsSuccessStatusCode == true)
            {
                string jsonstring = response.Content;
                dt = Tabulate(jsonstring);

                oPO.GetList(dt, POStatus);

                var jsonResult = Json(oPO, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                //LOAD MRU & LOG QUERY
                if (TempData["ReportTitle"] != null && TempData["RptCode"] != null)
                {
                    menuTitle = TempData["ReportTitle"] as string;
                    RptCode = TempData["RptCode"].ToString();
                    TempData.Keep();
                    //cLog oLog = new cLog();
                    //oLog.SaveLog(menuTitle, Request.Url.PathAndQuery, RptCode);
                }
                return jsonResult;
            }
            else
            {
                return Json("");
            }
        }

        public JsonResult SendEmail(string PONo, string Vendor, string PartNo,  string emaiAddress,string DueDate, string Qty,string Price, string message)
        {
            // Sender's email address and password
            string senderEmail = "yousufdev4@gmail.com";
            string senderPassword = "qvav wxwd wcho wofj";

            string userName = Session["Username"].ToString();
            var subject = "PO Information against this PONo. " + PONo + "";
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
            string Baseurl = @"http://localhost:61844/Externals/VendorPOEmail.aspx?PO=<PO>&Buyer=<Buyer>&PartNo=<PartNo>&DueDate=<DueDate>&Qty=<Qty>&Price=<Price>";
            Baseurl = Baseurl.Replace("<PO>", PONo);
            Baseurl = Baseurl.Replace("<Buyer>", userName);
            Baseurl = Baseurl.Replace("<PartNo>", PartNo);
            Baseurl = Baseurl.Replace("<DueDate>", DueDate);
            Baseurl = Baseurl.Replace("<Qty>", Qty);
            Baseurl = Baseurl.Replace("<Price>", Price);
            
            // HTML body containing the form
            string htmlBody = $@"<!DOCTYPE html>
                            <html>
                            <head>
                                <title>Email Body</title>
                            </head>
                            <body>
                                <h2>Purchase Order Details</h2>
                                <table>
                                    <tr>
                                        <td><strong>Buyer:</strong></td>
                                        <td><BuyerName></td>
                                    </tr>
                                    <tr>
                                        <td><strong>PO No.:</strong></td>
                                        <td><PONo></td>
                                    </tr>
                                    <tr>
                                        <td><strong>Part No.:</strong></td>
                                        <td><PartNo></td>
                                    </tr>
                                    <tr>
                                        <td><strong>Qty:</strong></td>
                                        <td><Qty></td>
                                    </tr>
                                    <tr>
                                        <td><strong>Due Date:</strong></td>
                                        <td><DueDate></td>
                                    </tr>
                                    <tr>
                                        <td><strong>Price:</strong></td>
                                        <td><Price></td>
                                    </tr>
                                     <tr>
                                        <td><strong>Message:</strong></td>
                                        <td><Message></td>
                                    </tr>
                                </table>
                                <p>
                                    <a href=""<URL>"" target=""_blank"">Click here to view the full purchase order details</a>
                                </p>
                            </body>
                            </html>";

            htmlBody = htmlBody.Replace("<BuyerName>", userName);
            htmlBody = htmlBody.Replace("<PONo>", PONo);
            htmlBody = htmlBody.Replace("<PartNo>", PartNo);
            htmlBody = htmlBody.Replace("<Qty>", Qty);
            htmlBody = htmlBody.Replace("<DueDate>", DueDate);
            htmlBody = htmlBody.Replace("<Price>", Price);
            htmlBody = htmlBody.Replace("<Message>", message);
            htmlBody = htmlBody.Replace("<URL>", Baseurl);

            mailMessage.Body = htmlBody;

            // Create a SmtpClient instance
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

            try
            {
                // Send the email
                smtpClient.Send(mailMessage);
                oPO.SendEmail(PONo, senderEmail, emaiAddress, subject, htmlBody, userName);
                oPO.AddInBuyers(userName, Vendor, PartNo, PONo, Qty, DueDate, Price, userName);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Some Error";
            }

            var jsonResult = Json("Send", JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            
            return jsonResult;
        }

        public static DataTable Tabulate(string json)
        {
            var jsonLinq = JObject.Parse(json);
            //int counter = 0;
            // Find the first array using Linq
            var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
            var trgArray = new JArray();
            foreach (JObject row in srcArray.Children<JObject>())
            {
                var cleanRow = new JObject();
                foreach (JProperty column in row.Properties())
                {
                    // Only include JValue types
                    if (column.Value is JValue)
                    {
                        cleanRow.Add(column.Name, column.Value);
                    }
                }
                trgArray.Add(cleanRow);
                //counter = counter + 1;

                //if (counter < 5)
                //    trgArray.Add(cleanRow);
                //else
                //    break;

            }

            return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
        }

        public static DataTable ConvertJsonToDataTable(string json)
        {
            // Parse the JSON
            JToken token = JToken.Parse(json);

            // Initialize DataTable
            DataTable dataTable = new DataTable();

            if (token.Type == JTokenType.Array)
            {
                // If JSON is an array, extract column names from the properties of the first object
                JArray jsonArray = (JArray)token;
                if (jsonArray.Count > 0)
                {
                    JObject firstObject = (JObject)jsonArray.First;
                    foreach (JProperty property in firstObject.Properties())
                    {
                        dataTable.Columns.Add(property.Name, typeof(string)); // Adjust type as needed
                    }

                    // Populate the DataTable with JSON data
                    var rows = jsonArray.Select(j => ((JObject)j).Properties()
                                        .Select(p => p.Value.ToString()).ToArray());
                    foreach (var row in rows)
                    {
                        dataTable.Rows.Add(row);
                    }
                }
            }
            else if (token.Type == JTokenType.Object)
            {
                // If JSON is a single object, extract column names from its properties
                JObject jsonObject = (JObject)token;
                foreach (JProperty property in jsonObject.Properties())
                {
                    dataTable.Columns.Add(property.Name, typeof(string)); // Adjust type as needed
                }

                // Populate a single row in the DataTable
                dataTable.Rows.Add(jsonObject.Properties()
                                        .Select(p => p.Value.ToString()).ToArray());
            }
            else
            {
                throw new ArgumentException("Invalid JSON format.");
            }

            return dataTable;
        }
    }
}