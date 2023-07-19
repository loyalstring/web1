using JewelleryWebApplication.Base.Model;

namespace JewelleryWebApplication.Models
{
    public class tblProductsDetails : BaseModel
    {
        public String purity { get; set; }
        public String barcodeNumber { get; set; }
        public String itemCode { get; set; }
        public String box { get; set; }
        public String grossWeight { get; set; }
        public String netWeight { get; set; }
        public String stoneweight { get; set; }
        public String makinggm { get; set; }
        public String makingper { get; set; }
        public String fixedamount { get; set; }
        public String fixedwastage { get; set; }
        public String stoneamount { get; set; }
        public String mrp { get; set; }
        public String huidcode { get; set; }
        public String partycode { get; set; }
        public String DiamondWeight { get; set;}
        public String DiamondPeaces { get; set; }
        public String DiamondRate { get; set; }
        public String DiamondAmount { get; set; }
        public String Colour { get; set; }
        public String Clarity { get; set; }
        public String SettingType { get; set;}
        public String Shape { get; set; }
        public String DiamondSize { get; set; }
        public string Certificate { get; set; }

    }
}
