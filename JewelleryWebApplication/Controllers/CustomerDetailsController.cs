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

namespace JewelleryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerDetailsController : ControllerBase
    {
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
        public CustomerDetailsController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, ILogger<RegisterModel> logger, IRazorViewToStringRenderer razorViewToStringRenderer,IWebHostEnvironment webHostEnvironment, IOrdersRepository ordersRepository, ICustomerDetailsRepository customerDetailsRepository,
            IRateRepository rateRepository, IPurityRepository purityRepository, IProductRepository productRepository, IMaterialCategoryRepository materialCategoryRepository, IStaffRepository staffRepository)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _environment = webHostEnvironment;
            _ordersRepository = ordersRepository;
            _customerDetailsRepository = customerDetailsRepository;
            _rateRepository=rateRepository;
            _productRepository=productRepository;
            _materialCategoryRepository=materialCategoryRepository;

        }
        [HttpGet]
        [Route("GetAllCustomer")]
        public async Task<IActionResult> GetAllCustomer()
        {
            var Customerdata =await _customerDetailsRepository.All().ToListAsync();
            if (Customerdata!=null)
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
                Customer.PerAdd = model.PerAdd;
                Customer.CurrAdd = model.CurrAdd;
                Customer.Mobile = model.Mobile;
                Customer.Email = model.Email;
                var emailexist = data.Where(x => x.Email.ToUpper() == Customer.Email.ToUpper()).FirstOrDefault();
                if (emailexist != null)
                {
                    return Ok(new { message = "email already exist" });
                }
                Customer.Password = model.Mobile;

                Customer.Customer_login_id = Customer.Email;
                Customer.PinCode = model.PinCode;
                Customer.DOB = model.DOB;
                Customer.OnlineStatus = "Active";
                await _customerDetailsRepository.InsertAsync(Customer);
             //   await CreateUserAsync(Customer.Email, Customer.Customer_login_id, Customer.Mobile,Customer.Password);

                await sendEmail(Customer.Email);

                return Ok(new { status = "Success", data = Customer });
            }
            return BadRequest();
        }

        private async Task CreateUserAsync(string Email, string UserName, string Mobile,string Password)
        {
            var user = new ApplicationUser { UserName = Email, Email = Email,PhoneNumber=Mobile,PasswordHash=Password, IsEnabled = true };

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
                Customer.PerAdd = model.PerAdd;
                Customer.CurrAdd = model.CurrAdd;
                Customer.Mobile = model.Mobile;
                Customer.Mobile = model.Mobile;
                Customer.Email = model.Email;
             
                var emailexist = data.Where(x => x.Email.ToUpper() == Customer.Email.ToUpper()).FirstOrDefault();
                if (emailexist != null)
                {
                    return Ok(new { message = "email already exist" });
                }
                    Customer.OrderCount = model.OrderCount;
                    Customer.OrderId= model.OrderId;
                    Customer.OrderStatus = model.OrderStatus;
                    Customer.OrderValue = model.OrderValue;
                Customer.Password = model.Password;
                Customer.Customer_login_id = Customer.Customer_login_id;
                Customer.PinCode = model.PinCode;
                Customer.DOB = model.DOB;


                await _customerDetailsRepository.InsertAsync(Customer);

             //   await sendEmail(Customer.Email);
                return Ok(new { status = "Success", data = Customer });
            }
            return BadRequest();
        }
        //Task ConfirmEmailAsync(tblCustomerDetails user, string code);
        //Task GenerateEmailConfirmationTokenAsync(string email);
        private async Task sendEmail( string Email)
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
            var Customerdata = _customerDetailsRepository.All().Where(x=>x.Id==model.Id).FirstOrDefault();
         
            if (Customerdata != null)
            {
                if (ModelState.IsValid)
                {
                    Customerdata.Password = model.Password;
                   await _customerDetailsRepository.UpdateAsync(Customerdata,Customerdata.Id);
                    return Ok(new { status = "Success", data = Customerdata });
                }
            }
            return BadRequest();
        }

        [HttpPost("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(tblCustomerDetails model)
        {
            var customer =_customerDetailsRepository.All().Where(x=>x.Id==model.Id).FirstOrDefault();
             if(customer!=null)
            {
                if (model.FirstName != null)
                {
                    customer.FirstName = model.FirstName;
                }
              else  if (model.OnlineStatus != null)
                {
                    customer.OnlineStatus = model.OnlineStatus;
                }
                else   if (model.MiddleName != null)
                {
                    customer.MiddleName = model.MiddleName;
                }
              else  if (model.LastName != null)
                {
                    customer.LastName = model.LastName;
                }
              else  if (model.PerAdd != null)
                {
                    customer.PerAdd = model.PerAdd;
                }
             else   if (model.CurrAdd != null)
                {
                    customer.CurrAdd = model.CurrAdd;
                }
              else  if (model.Mobile != null)
                {
                    customer.Mobile = model.Mobile;
                }
                else if(model.Email != null)
                {
                    if (model.Email.ToUpper() == customer.Email.ToUpper())
                    {
                        return Ok( new{message="Email Already Exist"});
                    }
                    customer.Email = model.Email;
                    customer.Customer_login_id = customer.Email;
                }
                else if (model.DOB != null)
                {
                    customer.DOB = model.DOB;
                }
                else if (model.PinCode != null)
                {
                    customer.PinCode = model.PinCode;
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
               
                    customer.PerAdd = model.PerAdd;
                
                    customer.CurrAdd = model.CurrAdd;
             
                    customer.Mobile = model.Mobile;
                
                    if (model.Email.ToUpper() == customer.Email.ToUpper())
                    {
                        return Ok(new { message = "Email Already Exist" });
                    }
                    customer.Email = model.Email;
                    customer.Customer_login_id = customer.Email;
                
                    customer.DOB = model.DOB;
                customer.Password = model.Password;

                customer.PinCode = model.PinCode;
                
                    customer.Gender = model.Gender;
               
                    customer.OrderValue = model.OrderValue;
               
                    customer.OrderId = model.OrderId;
                
                    customer.OrderStatus = model.OrderStatus;
               
                    customer.OrderCount = "Active";
                

                await _customerDetailsRepository.UpdateAsync(customer, customer.Id);
                return Ok(new { status = "success", data = customer });

            }

            return BadRequest();

        }

    }
}
