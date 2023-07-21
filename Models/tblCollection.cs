using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelleryWebApplication.Models
{
    public class tblCollection:BaseModel
    {
        [ForeignKey("tblMaterialCategory")]
        public int Category_id { get; set; }
        public tblMaterialCategory tblMaterialCategory { get; set; }
        public string ProductType { get; set; }
        public string Collection_Name { get; set; }
        public string Slug { get; set; }
        public string Label { get; set; }
        public string OnlineStatus { get; set; }


    }
}
