using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelleryWebApplication.Models
{
    public class OrderItemDetails:BaseModel
    {
        [ForeignKey("tblOrder")]
        public int OrderId { get; set; }
        public tblOrder tblOrder { get; set; }

        [ForeignKey("tblProduct")]
        public int ProductId { get; set; }
        public tblProduct tblProduct { get; set; }
        public string OrderNumber { get; set; }
        public string MRP { get; set; }

        [ForeignKey("tblCustomerDetails")]
        public int Customer_Id { get; set; }
        public tblCustomerDetails tblCustomerDetails { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
      //  public string OrderStatus { get; set; }
        public string CategoryName { get; set; }
        public string ItemCode { get; set; }
        public string HSNCode { get; set; }
        public string grosswt { get; set; }
        public string NetWt { get; set; }
        public string StoneWeight { get; set; }
        public string Purity { get; set; }
        public string makingchrg { get; set; }
        public string Rate { get; set; }
        public string StoneAmount { get; set; }

    }
}
