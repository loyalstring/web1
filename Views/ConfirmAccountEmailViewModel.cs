using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryWebApplication.Views
{
    public class ConfirmAccountEmailViewModel
    {
            public ConfirmAccountEmailViewModel(string confirmEmailUrl, string email, string password)
            {
                ConfirmEmailUrl = confirmEmailUrl;
                Email = email;
                Password = password;
            }

            public string ConfirmEmailUrl { get; set; }

            public string Email { get; set; }
            public string Password { get; set; }
    }
}
