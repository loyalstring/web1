using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace JewelleryWebApplication.Models
{
    public class tblRates:BaseModel
    {

   
        public decimal todaysrate { get; set; }

        [ForeignKey("tblStaff")]
        public int Staff_id { get; set; }
        public tblStaff tblStaff { get; set; }
      
        [ForeignKey("tblMaterialCategory")]
        public int Category_id { get; set; }
        public tblMaterialCategory tblMaterialCategory { get; set; }
        public string Category_Label { get; set; }
        public string Purity { get; set; }

        public string OnlineStatus { get; set; }
    }
}
