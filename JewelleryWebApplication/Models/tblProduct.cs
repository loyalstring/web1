using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace JewelleryWebApplication.Models
{
    public  class tblProduct: BaseModel
    {
      
        public string Product_Name { get; set; }
        public string Label { get; set; }
        public string hsn_code { get; set; }
        public string description { get; set; }
        public int MinQuantity { get; set; }
         public string Images { get; set; }

        public string ImageList1 { get; set; }
        public string ImageList2 { get; set; }
        public string ImageList3 { get; set; }
        public string ImageList4 { get; set; }
        public string ImageList5 { get; set; }

        public decimal MinWeight { get; set; }
    
        [ForeignKey("tblStaff")]
        public int Entryby_Staff_id { get; set; }
        public tblStaff tblStaff { get; set; }

        [ForeignKey("tblPurity")]
        public int PurityId { get; set; }
        public tblPurity tblPurity { get; set; }

        public string ItemCode { get; set; }
        public string Product_No { get; set; }

        [ForeignKey("tblMaterialCategory")]
        public int Category_id { get; set; }
        public tblMaterialCategory tblMaterialCategory { get; set; }
        public string Material { get; set; }
        public string Gm { get; set; }
        public string Size { get; set; }
        public decimal grosswt { get; set; }
        public string purity { get; set; }
        public string collection { get; set; }
        public string occasion { get; set; }
        public string gender { get; set; }
        public string product_type { get; set; }
        public string Making_Percentage { get; set; }
        public string Making_Fixed_Amt { get; set; }
        public string Making_Fixed_Wastage { get; set; }
        public string Making_per_gram { get; set; }

        public string StoneWeight { get; set; }
        public string StoneAmount { get; set; }
        public string Featured { get; set; }
        public string ItemType { get; set; }
        public string Category_Name { get; set; }
        public int Pieces { get; set; }
        public string HUIDCode { get; set; }
        public decimal NetWt { get; set; }
        public string Product_Code { get; set; }
    
        public int MRP { get; set; }

        [NotMapped]
        public int Quantity { get; set;}

        [ForeignKey("tblProductType")]
        public int ProductTypeId { get; set; }
        public tblProductType tblProductType { get; set; }
        [ForeignKey("tblCollection")]
        public int CollectionId { get; set; }
        public tblCollection tblCollection { get; set; }
        [ForeignKey("Party_Details")]
        public int PartyTypeId { get; set; }
        public Party_Details Party_Details { get; set; }

        [ForeignKey("tblBox")]
        public int BoxId { get; set; }
        public tblBox tblBox { get; set; }
        public string OnlineStatus { get; set; }

        public string TID { get; set; }
        public string BarcodeNumber { get; set; }
        public string DiamondWeight { get; set; }
        public string DiamondPeaces { get; set; }
        public string DiamondRate { get; set; }
        public string DiamondAmount { get; set; }
        public string Colour { get; set; }
        public string Clarity { get; set; }
        public string SettingType { get; set; }
        public string Shape { get; set; }
        public string DiamondSize { get; set; }
        public string Certificate { get; set; }
    }
}
