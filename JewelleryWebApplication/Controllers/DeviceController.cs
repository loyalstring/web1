using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
using JewelleryWebApplication.Models.APIModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JewelleryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        public readonly ItblProductDetailsRepository _productDetailsRepository;
        public readonly IDeviceRepository _deviceRepository;
        public DeviceController(IDeviceRepository deviceRepository, ItblProductDetailsRepository productDetailsRepository)
        {
            _deviceRepository = deviceRepository;
            _productDetailsRepository = productDetailsRepository;
        }

        [HttpGet("GetAllDevice")]
        public async Task<IActionResult> GetAllDevice()
        {
            var datadevice = _deviceRepository.All().Where(x => x.Mobile == "8806588921" && x.Email == "vishwaspalve93@gmail.com" && x.SerialNo == "HC720E200901330").FirstOrDefault();
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

        [HttpPost("AddProducts")]
        public async Task<IActionResult> AddProducts(List<tblProductsDetails> products)
        {
            if (ModelState.IsValid)
            {
                var datadevice = _deviceRepository.All().Where(x => x.Mobile == "8806588921" && x.Email == "vishwaspalve93@gmail.com" && x.SerialNo == "HC720E200901330").FirstOrDefault();
                if (datadevice != null) { 
                    List<tblProductsDetails> list = new List<tblProductsDetails>();
                    foreach (var item in products)
                    {
                        tblProductsDetails tblProductsDetails = new tblProductsDetails();
                        tblProductsDetails.barcodeNumber = item.barcodeNumber;
                        tblProductsDetails.mrp = item.mrp;
                        tblProductsDetails.purity = item.purity;
                        tblProductsDetails.fixedamount = item.fixedamount;
                        tblProductsDetails.box = item.box;
                        tblProductsDetails.fixedwastage = item.fixedwastage;
                        tblProductsDetails.grossWeight = item.grossWeight;
                        tblProductsDetails.itemCode = item.itemCode;
                        tblProductsDetails.huidcode = item.huidcode;
                        tblProductsDetails.makinggm = item.makinggm;
                        tblProductsDetails.makingper = item.makingper;
                        tblProductsDetails.netWeight = item.netWeight;
                        tblProductsDetails.partycode = item.partycode;
                        tblProductsDetails.stoneamount = item.stoneamount;
                        tblProductsDetails.stoneweight = item.stoneweight;
                    tblProductsDetails.DiamondWeight = item.DiamondWeight;
                    tblProductsDetails.DiamondAmount= item.DiamondAmount;
                    tblProductsDetails.DiamondSize= item.DiamondSize;
                    tblProductsDetails.DiamondPeaces = item.DiamondPeaces;
                    tblProductsDetails.Clarity=item.Clarity;
                    tblProductsDetails.Certificate=item.Certificate;
                    tblProductsDetails.DiamondRate=item.DiamondRate;
                    tblProductsDetails.Colour=item.Colour;
                    tblProductsDetails.Shape=item.Shape;
                    tblProductsDetails.SettingType=item.SettingType;
                 
                        list.Add(tblProductsDetails);
                    }
                    await _productDetailsRepository.BulkInsertAsync(list);

                    return Ok(new { status = "Success", data = list });
                }
                else
                {
                    return Ok(new { message = "This device is not activate" });
                }
            }
            return BadRequest();
        }


        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct()
        {
            var productdata = _productDetailsRepository.All().ToList();
            if (productdata != null)
            {
                return Ok(new { status = "Success", data = productdata });

            }
            return Ok(new { status = "Success", data = "No Data" });

        }

    }
}
