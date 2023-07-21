using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelleryWebApplication.Models
{
    public class tblBox:BaseModel
    {
       
        public string MetalName { get; set; }
        public string BoxName { get; set; }
        public string EmptyWeight { get;set; }
        public string ProductName { get; set; }

        public string OnlineStatus { get; set; }
    }
}
