using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace JewelleryWebApplication.Models
{
    public class tblCustomerDetails:BaseModel
    {       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PerAdd { get; set; }
        public string CurrAdd { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public string Customer_login_id { get; set; }
      
        public string DOB { get; set; }
        public string MiddleName { get; set; }
        public string PinCode { get; set; }
        public string Gender { get; set; }
        public string OrderCount { get; set; }
        public string OrderValue { get; set; }
        public string OrderStatus { get; set; }
        public string OrderId { get; set; }
        public string OnlineStatus { get; set; }

    }
}
