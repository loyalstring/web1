using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace JewelleryWebApplication.Models
{
    public class tblProductType:BaseModel
    {
      
        [ForeignKey("tblMaterialCategory")]
        public int Category_id { get; set; }
        public tblMaterialCategory tblMaterialCategory { get; set; }
        public string ProductTitle { get; set; }
        public string Label { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }

        public string HSNCode { get; set; }
        public string OnlineStatus { get; set; }

    }
}
