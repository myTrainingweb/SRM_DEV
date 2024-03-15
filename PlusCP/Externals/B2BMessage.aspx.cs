using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Net;
using System.Web.Services;
using System.Xml;
using PlusCP.Models;

namespace PlusCP.Externals
{
    public partial class B2BMessage : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            string hdrId = Request.QueryString["hdrId"].ToString();
            string rptName = Request.QueryString["rptName"].ToString();
            GetProcessResult(hdrId, rptName);
        }
        private HttpWebRequest GetWebRequest()
        {
            string conType = HttpContext.Current.Session["CONN_TYPE"].ToString();
            string serviceUrl = string.Empty;
            if (conType == "Tran")
                serviceUrl = "http://b2b-uat-internal.teleplan.com/BiztalkMessaging/Service.svc";// make web request for the asmx web service
            else if (conType == "Prod")
                serviceUrl = "";
            else
                serviceUrl = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceUrl);
            request.Headers.Add(@"SOAP:Action");
            request.ContentType = "text/xml;charset=\"utf-8\"";
            request.Accept = "text/xml";
            request.Method = "POST";
            return request;
        }
        public string GetProcessResult(string hdrId, string rptName)
        {
            string user = HttpContext.Current.Session["Username"].ToString();
            XmlDocument soapEnvelopeXml = new XmlDocument();
            string xmlToLoad = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
            "xmlns:proc=\"http://ProcessFailedMessages.ProcessReq\"> " +
            "<soapenv:Header />" +
            "<soapenv:Body>" +
            "<proc:ProcessReq>" +
            "<MessageId>" + hdrId + "</MessageId>" +
            "<User>" + user + "</User>" +
            "<Processed>N</Processed>" +
            "<Database>" + rptName + "</Database>" +
            "</proc:ProcessReq>" +
            "</soapenv:Body>" +
            "</soapenv:Envelope>"; soapEnvelopeXml.XmlResolver = null;
            soapEnvelopeXml.LoadXml(xmlToLoad); try
            {
                HttpWebRequest request = GetWebRequest();
                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string xmlString = rd.ReadToEnd();
                        XmlDocument soapResult = new XmlDocument();
                        soapResult.XmlResolver = null;
                        soapResult.LoadXml(xmlString);
                        //Response.ContentType = "text/plain; charset=utf-8";
                        return "<xml>" + soapResult.GetElementsByTagName("ns0:ProcessResp")[0].InnerXml + "</xml>";


                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid"))
                    return "Not Implemented";
                else
                    return "some error occurred";
            }
        }


        [WebMethod]
        public static string B2BSendMessage(string hdrId, string rptName)
        {

            return ("");
        }
    }
}



