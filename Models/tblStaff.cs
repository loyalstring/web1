using JewelleryWebApplication.Base.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace JewelleryWebApplication.Models
{
    public class tblStaff:BaseModel
    {
     
        public string FirstName { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public bool StatusType { get; set; }
        public string Staff_login_id { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string OnlineStatus { get; set; }



    }
}
