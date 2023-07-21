using Microsoft.AspNetCore.Identity;

namespace JewelleryWebApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public int CompanyId { get; set; }
        public bool? IsEnabled { get; set; }
    
    }
}
