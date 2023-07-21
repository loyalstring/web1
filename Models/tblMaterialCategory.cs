using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace JewelleryWebApplication.Models
{
    public  class tblMaterialCategory:BaseModel
    {
      
    
        public string Name { get; set; }
        public string Description { get; set; }

        [ForeignKey("tblStaff")]
        public int Entryby_Staff_id { get; set; }
        public tblStaff tblStaff { get; set; }
        public string Label { get; set; }
        public string ItemType { get; set; }
        public string Material { get; set; }
     
        public string ParentsCategory { get; set; }
        public string Slug { get; set; }
        public string HSNCode { get; set; }
        public string ShortCode { get; set; }
        public string OnlineStatus { get; set; }
    }
}
