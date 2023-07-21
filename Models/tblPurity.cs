using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace JewelleryWebApplication.Models
{
    public class tblPurity:BaseModel
    {
     
     
        public string Purity { get; set; }
   
        public string Category { get; set; }
        public string Label { get; set; }
        public string TodaysRate { get; set; }
        public string OnlineStatus { get; set; }
    }
}
