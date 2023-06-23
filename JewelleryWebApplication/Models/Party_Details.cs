using JewelleryWebApplication.Base.Model;

namespace JewelleryWebApplication.Models
{
    public class Party_Details:BaseModel
    {
       public string supplier_code { get; set; }
        public string supplierType { get; set; }
        public string supplier_name { get; set; }
       
        public string party_pan_no { get; set; }
        public string party_adhar_no { get; set; }
        public string contact_no { get;set; }
      

        public string email_id { get; set; }
        public string address { get; set; }
        public string state { get; set; }
        public string city { get; set; }

        public string firm_name { get; set; }
        public string firm_details { get; set; }
        public string gst_no { get; set; }
        public string central_gst_no { get; set; }
        public string OnlineStatus { get; set; }

    }
}
