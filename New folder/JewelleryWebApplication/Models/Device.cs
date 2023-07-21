using JewelleryWebApplication.Base.Model;

namespace JewelleryWebApplication.Models
{
    public class Device :BaseModel
    {
        public string SerialNo { get; set; }

        public string MacId { get;set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }


    }
}
