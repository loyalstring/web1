using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JewelleryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        public readonly IDeviceRepository _deviceRepository;
        public DeviceController(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;

        }

        [HttpGet("GetAllDevice")]
        public async Task<IActionResult> GetAllDevice()
        { 
            var datadevice = _deviceRepository.All().ToList();
            if (datadevice != null)
            {
                return Ok(new { status = "Success", data = datadevice });
            }
        
            return Ok(new { status = "Success", data = "No data" });
        }
        [HttpPost("AddDevice")]
        public async Task<IActionResult> AddDevice(Device model)
        {
            if (ModelState.IsValid)
            {
                Device device = new Device();
                device.MacId = model.MacId;
                device.SerialNo = model.SerialNo;
                device.Name = model.Name;
                device.Email = model.Email;
                device.Address = model.Address;
                device.Mobile = model.Mobile;
                await _deviceRepository.InsertAsync(device);
                return Ok(new { status = "Success", data = device });
            }
            return BadRequest();
        }


    }
}
