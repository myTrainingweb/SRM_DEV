using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
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

        [Display(Name = "Buyer Company :")]
        public string BuyerCompany { get; set; }

        [Display(Name = "Buyer Name :")]
        public string BuyerName { get; set; }

        [Display(Name = "Rel :")]
        public string Release { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }
        [Display(Name = "PO No.")]
        public string PONo { get; set; }

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

       

        [Display(Name = "Price")]
        public string Price { get; set; }

        public string ReportTitle { get; set; }
        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }
        public string filterString { get; set; }
        public string ErrorMessage { get; set; }
        public List<Hashtable> lstPO { get; set; }

        public List<object> lstMst = new List<object>();
        #endregion
        #region Methods 
        public bool GetList(DataTable dt, string POStatus)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            dt.Columns.Add("History");
            dt.Columns.Add("Message");
           
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

        public void SendEmail(string PONo, string sender, string receiver,string subject, string body, string createdBy)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string sql = @"INSERT INTO [dbo].[UserEmail]
           ([Sender]
           ,[Receiver]
           ,[Subject]
           ,[Body]
           ,[PONo]
           ,[CreatedBy])
     VALUES
           ('<Sender>'
           ,'<Receiver>'
           ,'<Subject>'
           ,'<Body>'
           ,'<PONo>'
           ,'<CreatedBy>')";

            sql = sql.Replace("<Sender>", sender);
            sql = sql.Replace("<Receiver>", receiver);
            sql = sql.Replace("<Subject>", subject);
            sql = sql.Replace("<Body>", body);
            sql = sql.Replace("<PONo>", PONo);
            sql = sql.Replace("<CreatedBy>", createdBy);

            oDAL.Execute(sql);
        }

        public void AddInBuyers(string Buyer, string Vendor, string PartNo, string PONo, string Qty, string DueDate, string Price, string createdBy)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
            string sql = @"INSERT INTO [SRM].[Buyer]
           ([Buyer]
           ,[Vendor]
           ,[PartNo]
           ,[PONum]
           ,[Qty]
           ,[DueDate]
           ,[Price]
           ,[CreatedBy])
     VALUES
           ('<Buyer>'
           ,'<Vendor>'
           ,'<PartNo>'
           ,'<PONum>'
           ,'<Qty>'
           ,'<DueDate>'
           ,'<Price>'
           ,'<CreatedBy>')";

            DateTime dateValue = DateTime.Parse(DueDate);
            double Qtyvalue = Convert.ToDouble(Qty);
            double PriceValue = Convert.ToDouble(Price);
            int Quantity = (int)Qtyvalue;
            int FinalPrice = (int)PriceValue;

            sql = sql.Replace("<Buyer>", Buyer);
            sql = sql.Replace("<Vendor>", Vendor);
            sql = sql.Replace("<PartNo>", PartNo);
            sql = sql.Replace("<PONum>", PONo);
            sql = sql.Replace("<Qty>", Quantity.ToString());
            sql = sql.Replace("<DueDate>", dateValue.ToString());
            sql = sql.Replace("<Price>", PriceValue.ToString());
            sql = sql.Replace("<CreatedBy>", createdBy);

            oDAL.Execute(sql);
        }


        #endregion
    }
}