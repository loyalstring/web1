using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
using JewelleryWebApplication.Models.APIModel;
using JewelleryWebApplication.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JewelleryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        public readonly ItblSecretRepository _tblSecretRepository;
        public readonly ItblProductDetailsRepository _productDetailsRepository;
        public readonly IDeviceRepository _deviceRepository;
        public DeviceController(ItblSecretRepository tblSecretRepository, IDeviceRepository deviceRepository, ItblProductDetailsRepository productDetailsRepository)
        {
            _tblSecretRepository = tblSecretRepository;
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
                //  var datadevice = _deviceRepository.All().Where(x => x.Mobile == "8806588921" && x.Email == "vishwaspalve93@gmail.com" && x.SerialNo == "HC720E200901330").FirstOrDefault();
                //   if (datadevice != null) { 
                List<tblSecret> li = new List<tblSecret>();
                List<tblProductsDetails> list = new List<tblProductsDetails>();
                    foreach (var item in products)
                    { tblSecret tblSecret=new tblSecret();
                        tblProductsDetails tblProductsDetails = new tblProductsDetails();
                       // tblProductsDetails.createdDate = item.createdDate;
                        tblProductsDetails.createdBy = item.createdBy;
                        tblProductsDetails.tidValue = item.tidValue;
                        tblProductsDetails.epcValue = item.epcValue;
                        tblProductsDetails.category = item.category;
                        tblProductsDetails.product = item.product;
                        tblProductsDetails.purity = item.purity;
                        tblProductsDetails.barcodeNumber = item.barcodeNumber;
                        tblProductsDetails.itemCode = item.itemCode;
                        tblProductsDetails.box = item.box;
                        tblProductsDetails.grossWeight = item.grossWeight;
                        tblProductsDetails.netWeight = item.netWeight;
                        tblProductsDetails.stoneweight = item.stoneweight;
                        tblProductsDetails.makinggm = item.makinggm;
                        tblProductsDetails.makingper = item.makingper;
                    tblProductsDetails.fixedamount = item.fixedamount;
                    tblProductsDetails.fixedwastage = item.fixedwastage;
                    tblProductsDetails.stoneamount = item.stoneamount;
                    tblProductsDetails.mrp = item.mrp;
                    tblProductsDetails.hudicode = item.hudicode;
                    tblProductsDetails.partycode = item.partycode;
                    tblProductsDetails.updatedDate = item.updatedDate;
                    tblProductsDetails.updatedBy = item.updatedBy;
                    tblProductsDetails.tagstate = item.tagstate;
                    tblProductsDetails.tagtransaction = item.tagtransaction;
                        tblProductsDetails.status = item.status;
                    tblSecret.TID = tblProductsDetails.tidValue;
                    tblSecret.BarcodeNumber = tblProductsDetails.barcodeNumber;
                        list.Add(tblProductsDetails);
                        li.Add(tblSecret);
                    }
                    await _productDetailsRepository.BulkInsertAsync(list);
                await _tblSecretRepository.BulkInsertAsync(li);
                    return Ok(new { status = "Success", data = list });
                //}
                //else
                //{
                //    return Ok(new { message = "This device is not activate" });
                //}
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
