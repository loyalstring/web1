using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using JewelleryWebApplication.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using JewelleryWebApplication.Services;
using JewelleryWebApplication.Views;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal;
using System.Web;
using Castle.Core.Resource;
using JewelleryWebApplication.Models.APIModel;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace JewelleryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerDetailsController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        public static IWebHostEnvironment _environment;
        private readonly IEmailSender _emailSender;
        private readonly IRateRepository _rateRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerDetailsRepository _customerDetailsRepository;
        private readonly IOrdersRepository _ordersRepository;

        private readonly IMaterialCategoryRepository _materialCategoryRepository;
        public CustomerDetailsController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IEmailSender emailSender, ILogger<RegisterModel> logger, IRazorViewToStringRenderer razorViewToStringRenderer, IWebHostEnvironment webHostEnvironment, IOrdersRepository ordersRepository, ICustomerDetailsRepository customerDetailsRepository,
            IRateRepository rateRepository, IPurityRepository purityRepository, IProductRepository productRepository, IMaterialCategoryRepository materialCategoryRepository, IStaffRepository staffRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _environment = webHostEnvironment;
            _ordersRepository = ordersRepository;
            _customerDetailsRepository = customerDetailsRepository;
            _rateRepository = rateRepository;
            _productRepository = productRepository;
            _materialCategoryRepository = materialCategoryRepository;

        }
        [HttpGet]
        [Route("GetAllCustomer")]
        public async Task<IActionResult> GetAllCustomer()
        {
            var Customerdata = await _customerDetailsRepository.All().ToListAsync();
            if (Customerdata != null)
            {
                return Ok(new { data = Customerdata });
            }
            return Ok(new { status = "Success", data = "No Data" });
        }
        [HttpPost("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(tblCustomerDetails model)
        {
            var Customerdata = _customerDetailsRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (Customerdata != null)
            {
                return Ok(new { status = "Success", data = Customerdata });
            }
            return Ok(new { data = "No Data" });
        }

        [HttpPost("CustomerByLoginId")]
        public async Task<IActionResult> CustomerByLoginId(tblCustomerDetails model)
        {
            var Customerdata = _customerDetailsRepository.All().Where(x => x.Customer_login_id == model.Customer_login_id).FirstOrDefault();
            if (Customerdata != null)
            {
                return Ok(new { status = "Success", data = Customerdata });
            }
            return Ok(new { data = "No Data" });
        }


        [HttpPost("AddCustomer")]
        public async Task<IActionResult> AddCustomer(CustomerViewModel model)
        {
            var data = _customerDetailsRepository.All().ToList();

            if (ModelState.IsValid)
            {
                tblCustomerDetails Customer = new tblCustomerDetails();
                Customer.FirstName = model.FirstName;
                Customer.MiddleName = model.MiddleName;
                Customer.LastName = model.LastName;
                Customer.Gender = model.Gender;
                Customer.perAddStreet = model.perAddStreet;

                Customer.currAddStreet = model.currAddStreet;
                Customer.currAddTown = model.currAddTown;
                Customer.currAddPinCode = model.currAddPinCode;
                Customer.currAddState = model.currAddState;
                Customer.perAddTown = model.perAddTown;
                Customer.perAddState = model.perAddState;
                Customer.currAddPinCode = model.currAddPinCode;
                Customer.Mobile = model.Mobile;
                Customer.Email = model.Email;
                var emailexist = data.Where(x => x.Email.ToUpper() == Customer.Email.ToUpper()).FirstOrDefault();
                if (emailexist != null)
                {
                    return Ok(new { message = "email already exist" });
                }
                Customer.Password = model.Password;

                Customer.Customer_login_id = Customer.Email;
             
                Customer.DOB = model.DOB;
                Customer.OnlineStatus = "Inactive";

                await _customerDetailsRepository.InsertAsync(Customer);
                //   await CreateUserAsync(Customer.Email, Customer.Customer_login_id, Customer.Mobile,Customer.Password);
                // await sendEmailConfirmation(Customer.Email);
                await sendEmail(Customer.Email);

                return Ok(new { status = "Success", data = Customer });
            }
            return BadRequest();
        }
        [HttpPost("UpdateAcoount")]
        public async Task<IActionResult> UpdateAcoount(ConfirmAccountViewModel model)
        {
            var user = _customerDetailsRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (user != null)
            {
                user.Id = model.Id;
                user.StatusType = model.StatusType;
                user.OnlineStatus = model.OnlineStatus;
                await _customerDetailsRepository.UpdateAsync(user, user.Id);
                await sendEmailConfirmation(user.Email);
                return Ok(new { status = "Success", data = user });

            }
            return BadRequest();

        }
        [HttpPost("ConfirmAcoount")]
        public async Task<IActionResult> ConfirmAcoount(string OnlineStatus, bool StatusType, int Id)
        {
            var user = _customerDetailsRepository.All().Where(x => x.Id == Id).FirstOrDefault();
            if (user != null)
            {
                user.Id = Id;
                user.StatusType = StatusType;
                user.OnlineStatus = OnlineStatus;
                await _customerDetailsRepository.UpdateAsync(user, user.Id);
                await sendEmailConfirmation(user.Email);
                return Ok(new { status = "Success", data = user });

            }
            return BadRequest();

        }



        private async Task sendEmailConfirmation(string Email)
        {

            var user = _customerDetailsRepository.All().Where(x => x.Email == Email).FirstOrDefault();
            var viewPath = Path.Combine(_environment.WebRootPath + "/Templates", "ConfirmAccount.html");
            var template = System.IO.File.ReadAllText(viewPath);
            //  string activationUrl = $"https://localhost:7020/api/CustomerDetails/ConfirmAacoount?OnlineStatus={"Active"}&StatusType={true}&Id={user.Id}";
            var activationUrl = "https://mkgharejewellers.com";
            template = template.Replace("XXXCallUrlXXX", activationUrl);

            // template = template.Replace("XXXXvalueXXX", user.OrderValue);
            // template = template.Replace("XXXStatusXXX", user.OrderStatus);
            await _emailSender.SendEmailAsync(user.Email, "Confirm Account", $"" + template + "");
        }


        private async Task CreateUserAsync(string Email, string UserName, string Mobile, string Password)
        {
            var user = new ApplicationUser { UserName = Email, Email = Email, PhoneNumber = Mobile, PasswordHash = Password, IsEnabled = true };

            var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                // here we assign the new user the "Admin" role 
                await _userManager.AddToRoleAsync(user, "Customer");

                _logger.LogInformation("User created a new account with password.");
                var code = _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page(
                     "/Views/ConfirmEmail",
                      pageHandler: null,
                     values: new { userId = user.Id, code = code },
                    protocol: Request.Scheme);

                var confirmAccountModel = new ConfirmAccountEmailViewModel($"{callbackUrl}", user.Email, user.PasswordHash);


                string body = await _razorViewToStringRenderer.RenderViewToStringAsync("~/Views/ConfirmAccount.cshtml", confirmAccountModel);
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", body);
            }
        }

        [HttpPost("AddCustomerList")]
        public async Task<IActionResult> AddCustomerList(tblCustomerDetails model)
        {
            var data = _customerDetailsRepository.All().ToList();

            if (ModelState.IsValid)
            {
                tblCustomerDetails Customer = new tblCustomerDetails();
                Customer.FirstName = model.FirstName;
                Customer.MiddleName = model.MiddleName;
                Customer.LastName = model.LastName;
                Customer.Gender = model.Gender;
                //Customer.PerAdd = model.PerAdd;
                //Customer.CurrAdd = model.CurrAdd;
                Customer.Mobile = model.Mobile;
                Customer.Mobile = model.Mobile;
                Customer.Email = model.Email;

                var emailexist = data.Where(x => x.Email.ToUpper() == Customer.Email.ToUpper()).FirstOrDefault();
                if (emailexist != null)
                {
                    return Ok(new { message = "email already exist" });
                }
                Customer.OrderCount = model.OrderCount;
                Customer.OrderId = model.OrderId;
                Customer.OrderStatus = model.OrderStatus;
                Customer.OrderValue = model.OrderValue;
                Customer.Password = model.Password;
                Customer.Customer_login_id = Customer.Customer_login_id;
                Customer.perAddStreet = model.perAddStreet;

                Customer.currAddStreet = model.currAddStreet;
                Customer.currAddTown = model.currAddTown;
                Customer.currAddPinCode = model.currAddPinCode;
                Customer.currAddState = model.currAddState;
                Customer.perAddTown = model.perAddTown;
                Customer.perAddState = model.perAddState;
                Customer.currAddPinCode = model.currAddPinCode;
                Customer.DOB = model.DOB;


                await _customerDetailsRepository.InsertAsync(Customer);

                //   await sendEmail(Customer.Email);
                return Ok(new { status = "Success", data = Customer });
            }
            return BadRequest();
        }
        //Task ConfirmEmailAsync(tblCustomerDetails user, string code);
        //Task GenerateEmailConfirmationTokenAsync(string email);
        private async Task sendEmail(string Email)
        {
            string Message = "<p>Your Registration Sucessfully Completed </p>";
            var user = _customerDetailsRepository.All().Where(x => x.Email == Email).FirstOrDefault();
            var viewPath = Path.Combine(_environment.WebRootPath + "/Templates", "Registered.html");
            var template = System.IO.File.ReadAllText(viewPath);
            template = template.Replace("XXXABCXXX", Message);
            template = template.Replace("XXXCallUrlXXX", "<p style=\"color:#ffffff\">For queries contact us</p>");
            template = template.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
            template = template.Replace("XXXXorderXXX", user.Password);
            // template = template.Replace("XXXXvalueXXX", user.OrderValue);
            // template = template.Replace("XXXStatusXXX", user.OrderStatus);
            await _emailSender.SendEmailAsync(user.Email, "THANK YOU FOR REGISTERING", $"" + template + "");
        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> SetPassword(tblCustomerDetails model)
        {
            var Customerdata = _customerDetailsRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();

            if (Customerdata != null)
            {
                if (ModelState.IsValid)
                {
                    Customerdata.Password = model.Password;
                    await _customerDetailsRepository.UpdateAsync(Customerdata, Customerdata.Id);
                    return Ok(new { status = "Success", data = Customerdata });
                }
            }
            return BadRequest();
        }

        [HttpPost("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(tblCustomerDetails model)
        {
            var customer = _customerDetailsRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (customer != null)
            {
                if (model.FirstName != null)
                {
                    customer.FirstName = model.FirstName;
                }
                else if (model.OnlineStatus != null)
                {
                    customer.OnlineStatus = model.OnlineStatus;
                }
                else if (model.MiddleName != null)
                {
                    customer.MiddleName = model.MiddleName;
                }
                else if (model.LastName != null)
                {
                    customer.LastName = model.LastName;
                }
                else if (model.perAddStreet != null)
                {
                    customer.perAddStreet = model.perAddStreet;
                }
                else if (model.currAddStreet != null)
                {
                    customer.currAddStreet = model.currAddStreet;
                }
                else if (model.Mobile != null)
                {
                    customer.Mobile = model.Mobile;
                }
                else if (model.Email != null)
                {
                    if (model.Email.ToUpper() == customer.Email.ToUpper())
                    {
                        return Ok(new { message = "Email Already Exist" });
                    }
                    customer.Email = model.Email;
                    customer.Customer_login_id = customer.Email;
                }
                else if (model.DOB != null)
                {
                    customer.DOB = model.DOB;
                }
                else if (model.perAddPinCode != null)
                {
                    customer.perAddPinCode = model.perAddPinCode;
                }
                else if (model.Gender != null)
                {
                    customer.Gender = model.Gender;
                }
                else if (model.OrderValue != null)
                {
                    customer.OrderValue = model.OrderValue;
                }
                else if (model.OrderId != null)
                {
                    customer.OrderId = model.OrderId;
                }
                else if (model.OrderStatus != null)
                {
                    customer.OrderStatus = model.OrderStatus;
                }
                else if (model.OrderCount != null)
                {
                    customer.OrderCount = "Active";
                }

                await _customerDetailsRepository.UpdateAsync(customer, customer.Id);
                return Ok(new { status = "success", data = customer });

            }

            return BadRequest();

        }

        [HttpPost("UpdateCustomers")]
        public async Task<IActionResult> UpdateCustomers(tblCustomerDetails model)
        {
            var customer = _customerDetailsRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (customer != null)
            {
                customer.Id = model.Id;
                customer.FirstName = model.FirstName;

                customer.OnlineStatus = model.OnlineStatus;

                customer.MiddleName = model.MiddleName;

                customer.LastName = model.LastName;

                customer.perAddStreet = model.perAddStreet;

                customer.currAddStreet = model.currAddStreet;
                customer.currAddTown = model.currAddTown;
                customer.currAddPinCode = model.currAddPinCode;
                customer.currAddState = model.currAddState;
                customer.perAddTown = model.perAddTown;
                customer.perAddState = model.perAddState;
                customer.perAddPinCode = model.perAddPinCode;
                customer.Mobile = model.Mobile;


                customer.Email = model.Email;
                customer.Customer_login_id = customer.Customer_login_id;

                customer.DOB = model.DOB;
                customer.Password = model.Password;

              

                customer.Gender = model.Gender;

                customer.OrderValue = model.OrderValue;

                customer.OrderId = model.OrderId;

                customer.OrderStatus = model.OrderStatus;

                customer.OrderCount = model.OrderCount;


                await _customerDetailsRepository.UpdateAsync(customer, customer.Id);
                return Ok(new { status = "success", data = customer });

            }

            return BadRequest();

        }
        [HttpGet("signin-facebook")]
        public IActionResult SignInFacebook()
        {
            var redirectUrl = "/"; // Redirect URL after successful authentication
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return Challenge(properties, "Facebook");
        }
        [HttpGet("signin-google")]
        public IActionResult SignInGoogle()
        {
            var redirectUrl = "/"; // Redirect URL after successful authentication
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }


        //[HttpPost("forgotpassword")]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(model.Email);
        //        if (user != null)
        //        {
        //            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //            var callbackUrl = $"{_appSettings.ClientUrl}/resetpassword?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";

        //            await _emailService.SendPasswordResetEmailAsync(user.Email, callbackUrl);

        //            return Ok(new { Message = "Password reset email sent." });
        //        }

        //        return NotFound(new { Error = "User not found." });
        //    }

        //    return BadRequest(ModelState);
        //}

        [HttpPost("GetOtp")]

        public async Task<IActionResult> GetOtp(CustomerViewModel model)
        {
            var Customerdata = _customerDetailsRepository.All().Where(x => x.Email == model.Email).FirstOrDefault();
            if (Customerdata != null)
            {
                Random generator = new Random();
                int r = generator.Next(100000, 1000000);
                await sendEmailotp(Customerdata.Email, r);
                return Ok(new { status = "Success", data = Customerdata, r });
            }
            return Ok(new { status = "Success", data = "No Data" });
        }
        private async Task sendEmailotp(string Email, int otp)
        {

            var user = _customerDetailsRepository.All().Where(x => x.Email == Email).FirstOrDefault();
            var viewPath = Path.Combine(_environment.WebRootPath + "/Templates", "otp.html");
            var template = System.IO.File.ReadAllText(viewPath);

            template = template.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
            template = template.Replace("XXXXotpXXX", otp.ToString());
            // template = template.Replace("XXXXvalueXXX", user.OrderValue);
            // template = template.Replace("XXXStatusXXX", user.OrderStatus);
            await _emailSender.SendEmailAsync(user.Email, "One-Time Password (OTP)  ", $"" + template + "");
        }
  
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallback")
            };

            return Challenge(authenticationProperties, "Google");
        }

        [HttpGet("GoogleLoginCallback")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");
            if (authenticateResult?.Principal is not null)
            {
                var emailClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Email);
                var nameClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Name);

                // Retrieve user email and name
                var email = emailClaim?.Value;
                var name = nameClaim?.Value;

                // Handle the authenticated user, e.g., create or update user account

                // Return token or perform other actions based on your application logic

                // Cleanup authentication cookie
                await HttpContext.SignOutAsync("Google");
            }

            return Unauthorized();
        }
    }
}
