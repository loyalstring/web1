using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace JewelleryWebApplication.Models
{
    public class tblOrder:BaseModel
    {
       
        [ForeignKey("tblCustomerDetails")]
        public int Customer_Id { get; set; }
        public tblCustomerDetails tblCustomerDetails { get; set; }
      
        [ForeignKey("tblProduct")]
        public int Product_id { get; set; }   
       public tblProduct tblProduct { get; set; }

        public decimal Price { get; set; }
        public string PaymentMode { get; set; }
        public string OrderStatus { get; set; }
        public string DeliveryAddress { get; set; }
        public string Offer { get; set; }
 
        public int Qty { get; set; }
        public string GovtTax { get; set; }
        public decimal ReceivedAmt { get; set; }
        public string OnlineStatus { get; set; }
        public string orderNumber { get; set; }
        public string Rate { get; set; }
      
        public string GrossWt { get; set;}
        public string NetWt { get; set; }
        public string StoneWt { get; set; }
        public string MRP { get; set; }
        public string CategoryName { get; set; }
        public string InvoiceNo { get; set; }
    }
    public class tblProductlist
    {
        public int Product_id { get; set; }
        public int Qty { get; set; }
       

    }
}
