using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JewelleryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        private readonly IStaffRepository _staffRepository;
        private readonly IRateRepository _rateRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerDetailsRepository _customerDetailsRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IMaterialCategoryRepository _materialCategoryRepository;
        public StaffController(IWebHostEnvironment webHostEnvironment, IOrdersRepository ordersRepository, ICustomerDetailsRepository customerDetailsRepository,
            IRateRepository rateRepository, IPurityRepository purityRepository, IProductRepository productRepository, IMaterialCategoryRepository materialCategoryRepository, IProductTypeRepository productTypeRepository, IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
            _ordersRepository = ordersRepository;
            _environment = webHostEnvironment;
            _rateRepository = rateRepository;
            _productRepository = productRepository;
            _customerDetailsRepository = customerDetailsRepository;
            _materialCategoryRepository = materialCategoryRepository;

        }

        [HttpPost("AddStaff")]
        public async Task<IActionResult> AddStaff(tblStaff model)
        {

            if (ModelState.IsValid)
            {
                tblStaff s = new tblStaff();

                s.FirstName = model.FirstName;
                s.LastName = model.LastName;
                s.Department = model.Department;
                s.Role = model.Role;
                s.StatusType = false;
                s.Staff_login_id = model.Staff_login_id;
                s.Mobile = model.Mobile;
                s.Password = s.Mobile;
                s.OnlineStatus = "Active";
                await _staffRepository.InsertAsync(s);
                return Ok(new { status = "Success", data = s });
            }
            return BadRequest();
        }
    }
}
