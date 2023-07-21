using JewelleryWebApplication.Base.Model;

namespace JewelleryWebApplication.Models
{
    public class tblProductsDetails : BaseModel
    {
      
        public string createdBy { get; set; }
        public string tidValue { get; set; }
        public string epcValue { get; set; }
        public string category { get; set; }
        public string product { get; set; }
        public string purity { get; set; }
        public string barcodeNumber { get; set; }
        public string itemCode { get; set; }
        public string box { get; set; }
        public decimal grossWeight { get; set; }
        public decimal netWeight { get; set; }
        public decimal stoneweight { get; set; }
        public string makinggm { get; set; }
        public string makingper { get; set; }
        public string fixedamount { get; set; }
        public string fixedwastage { get; set; }
        public string stoneamount { get; set; }
        public string mrp { get; set; }
        public string hudicode { get; set; }
        public string partycode { get; set; }

        public string updatedDate { get; set; }
        public string updatedBy { get; set; }

        public string tagstate { get; set; }
        public string tagtransaction { get; set; }

        public string status { get; set; }

    }
}
