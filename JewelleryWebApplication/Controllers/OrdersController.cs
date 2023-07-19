
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
using JewelleryWebApplication.Models.ViewModel;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Mail;
using System.Net;
using MailKit;
using System.Security.Cryptography;
using ContentDisposition = MimeKit.ContentDisposition;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure;
using iText.Html2pdf;
using System.IO;
using iText.Commons.Actions.Data;
using JewelleryWebApplication.Models.APIModel;
using Castle.Core.Resource;
using DocumentFormat.OpenXml.Drawing.Charts;
using JewelleryWebApplication.Repository;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.Azure;
using iText.Commons.Actions.Contexts;
using DocumentFormat.OpenXml.Vml;

namespace JewelleryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {  // public readonly IUnitOfWork _unitOfWork;
        private readonly IOrdersItemDetailsRepository _ordersItemDetailsRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IOrdersRepository _ordersRepository;
        private readonly ICustomerDetailsRepository _customerDetailsRepository;
        private readonly IRateRepository _rateRepository;
        private readonly IPurityRepository _purityRepository;
        private readonly IMaterialCategoryRepository _materialCategoryRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IProductRepository _productrepository;
        private readonly IEmailSender _emailSender;
        private readonly EmailSettings _emailSettings;


        public OrdersController(IOrdersItemDetailsRepository ordersItemDetailsRepository, ICollectionRepository collectionRepository, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IOrdersRepository ordersRepository, ICustomerDetailsRepository customerDetailsRepository, IRateRepository rateRepository, IPurityRepository purityRepository, IProductRepository productRepository, IMaterialCategoryRepository materialCategoryRepository, IProductTypeRepository productTypeRepository, IStaffRepository staffRepository, IEmailSender emailSender, IOptions<EmailSettings> emailSettings)
        {
            //    _unitOfWork = unitOfWork;
            _ordersItemDetailsRepository = ordersItemDetailsRepository;
            _collectionRepository = collectionRepository;
            _environment = webHostEnvironment;
            _configuration = configuration;
            _ordersRepository = ordersRepository;
            _customerDetailsRepository = customerDetailsRepository;
            _rateRepository = rateRepository;
            _purityRepository = purityRepository;
            _materialCategoryRepository = materialCategoryRepository;
            _productTypeRepository = productTypeRepository;
            _staffRepository = staffRepository;
            _productrepository = productRepository;
            _emailSender = emailSender;
            _emailSettings = emailSettings.Value;

        }


        [HttpGet("fetchAllOrders")]
        public async Task<IActionResult> fetchAllOrders()
        {
            var ordersdata = _ordersRepository.All().Include(x => x.tblProduct).Include(x => x.tblCustomerDetails).ToList();
            if (ordersdata != null)
            {
                return Ok(new { status = "Success", data = ordersdata });
            }
            return Ok(new { data = "No Data" });
        }

        [HttpGet("fetchAllRates")]
        public async Task<IActionResult> fetchAllRates()
        {
            var ratesdata = _rateRepository.All().ToList();
            if (ratesdata != null)
            {
                return Ok(new { status = "Success", data = ratesdata });
            }
            return Ok(new { data = "No Data" });
        }

        [HttpGet("fetchTodayRates")]
        public async Task<IActionResult> fetchTodayRates()
        {

            var ratesdata = _purityRepository.All().ToList().OrderByDescending(x => x.CreatedOn).FirstOrDefault();

            if (ratesdata != null)
            {
                return Ok(new { status = "Success", data = ratesdata });
            }
            return Ok(new { status = "Success", messege = "no data " });

        }
        [HttpGet("ratestodayallcategories")]
        public async Task<IActionResult> ratestodayallcategories()
        {

            var ratesdata = _rateRepository.All().Include(x => x.tblMaterialCategory).ToList().OrderByDescending(x => x.LastUpdated).Distinct();
            if (ratesdata != null)
            {
                return Ok(new { status = "success", data = ratesdata });

            }
            return BadRequest();
            //var category = _materialCategoryRepository.All().ToList();
            //if (category != null)
            //{

            //    return Ok(new { status = "Success", data = ratesdata, category });
            //}


            //  }

        }


        [HttpPost("ratestodaybycategory")]

        public async Task<IActionResult> ratestodayabycategoriesId(tblMaterialCategory model)
        {

            var ratesdata = _rateRepository.All().Include(x => x.tblMaterialCategory).Where(x => x.Category_id == model.Id).OrderByDescending(x => x.CreatedOn).FirstOrDefault();


            if (ratesdata != null)

            {
                return Ok(new { status = "Success", data = ratesdata });
            }
            return Ok(new { data = "No Data" });
        }

        [HttpPost("ratesallbycategoryId")]
        public async Task<IActionResult> ratesallbycategoryId(tblMaterialCategory model)
        {

            var ratesdata = _rateRepository.All().Include(x => x.tblMaterialCategory).Where(x => x.Category_id == model.Id).FirstOrDefault();


            if (ratesdata != null)

            {
                return Ok(new { status = "Success", data = ratesdata });
            }
            return Ok(new { data = "No Data" });
        }
        [HttpPost("AllOrdersByCustomerId")]
        public async Task<IActionResult> AllOrdersByCustomerId(tblOrder model)
        {
            var Ordersdata = _ordersRepository.All().Include(x => x.tblProduct).Include(x => x.tblCustomerDetails).Where(x => x.Customer_Id == model.Customer_Id).ToList();
            if (Ordersdata != null)
            {
                return Ok(new { status = "Success", data = Ordersdata });
            }
            return Ok(new { data = "No Data" });
        }

        [HttpPost("InsertOrders")]
        public async Task<IActionResult> InsertOrders(tblOrder model)
        {

            var productdata = _productrepository.All().Include(x => x.tblMaterialCategory).Where(x => x.Id == model.Product_id).FirstOrDefault();
            var puritydata = _purityRepository.All().Where(x => x.Id == productdata.PurityId).FirstOrDefault();
            var emaildata = _customerDetailsRepository.All().Where(x => x.Id == model.Customer_Id).FirstOrDefault();
            string productlabel = _materialCategoryRepository.All().Where(x => x.Id == productdata.Category_id).Select(X => X.Name).FirstOrDefault();

            int count = _ordersRepository.All().Select(x => x.CategoryName).Count();
            char categorycode = productlabel[0];
            int x = count + 1;
            Random generator = new Random();
            int r = generator.Next(100000, 1000000);
            var orderNumber = categorycode + r.ToString();

            DateTime currentDate = DateTime.Now;
            int startMonth = 4;
            int currentMonth = currentDate.Month;
            int year;
            if (currentMonth >= startMonth)
            {
                year = currentDate.Year;
            }
            else
            {
                year = currentDate.Year - 1;
            }
            int endYear = year + 1;
            string financialYear = $"{year}-{endYear}";
            var invoiceno = x + "/FY" + financialYear;

            if (ModelState.IsValid)
            {
                tblOrder order = new tblOrder();
                var isexist = _ordersRepository.All().Where(x => x.orderNumber.ToUpper() == orderNumber.ToUpper()).FirstOrDefault();
                if (isexist != null)
                {
                    return Ok(new { message = "Regenerate orders" });
                }
                order.orderNumber = orderNumber.ToString();
                order.InvoiceNo = invoiceno;
                order.Customer_Id = model.Customer_Id;
                order.Product_id = model.Product_id;
                order.Qty = model.Qty;
                order.Price = model.Price;
                order.PaymentMode = model.PaymentMode;
                order.Offer = model.Offer;
                order.GovtTax = model.GovtTax;
                order.ReceivedAmt = model.ReceivedAmt;
                order.OnlineStatus = "Active";
                order.OrderStatus = model.OrderStatus;
                order.Rate = puritydata.TodaysRate;
                order.GrossWt = productdata.grosswt.ToString();
                order.NetWt = productdata.NetWt.ToString();
                order.StoneWt = productdata.StoneWeight;
                order.CategoryName = productdata.Category_Name;
                order.MRP = productdata.MRP.ToString();


                await _ordersRepository.InsertAsync(order);

                // await sendEmail1pdf(order.Id,order.Product_id, order.Customer_Id, order.OrderStatus);
                //  await  sendEmail(order.Id, order.Product_id, order.Customer_Id, order.OrderStatus);
                // await sendEmail(order.Id,order.Product_id,order.Customer_Id,order.OrderStatus);
                return Ok(new { Status = "Success", data = order });
            }
            return BadRequest();
        }



        [HttpPost("InsertRates")]
        public async Task<IActionResult> InsertRates(tblRates model)
        {
            if (ModelState.IsValid)
            {
                tblRates rate = new tblRates();
                var staff = _staffRepository.All().Select(x => x.Id).First();

                rate.Category_id = model.Category_id;
                rate.Category_Label = model.Category_Label;
                rate.Purity = model.Purity;
                rate.todaysrate = model.todaysrate;
                rate.Staff_id = staff;
                rate.OnlineStatus = "Active";
                await _rateRepository.InsertAsync(rate);
                return Ok(new { Status = "Success", data = rate });
            }
            return BadRequest();
        }
        [HttpPost("UpdateOrders")]
        public async Task<IActionResult> UpdateOrders(tblOrder model)
        {
            var orderdata = _ordersRepository.All().Where(x => x.orderNumber == model.orderNumber).FirstOrDefault();
            if (orderdata != null)
            {
                if (model.OrderStatus == "Paid")
                {
                    orderdata.OrderStatus = "Paid";
                    await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);
                    await sendEmail(orderdata.Id);
                    return Ok(new { Status = "Success", data = orderdata });
                }

                else if (model.OrderStatus == "Delivered")
                {

                    orderdata.OnlineStatus = model.OnlineStatus;
                    orderdata.OrderStatus = "Delivered";
                    await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);

                    await sendEmail(orderdata.Id);
                    return Ok(new { Status = "Success", data = orderdata });
                }
                else if (model.OrderStatus == "Payment Failed")
                {

                    orderdata.OnlineStatus = model.OnlineStatus;
                    orderdata.OrderStatus = "Payment Failed";
                    await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);
                    await sendEmail(orderdata.Id);
                    return Ok(new { Status = "Success", data = orderdata });
                }
                else if (model.OrderStatus == "Shipped")
                {

                    orderdata.OnlineStatus = model.OnlineStatus;
                    orderdata.OrderStatus = "Shipped";
                    await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);
                    await sendEmail(orderdata.Id);
                    return Ok(new { Status = "Success", data = orderdata });

                }

            }


            return BadRequest();
        }
        private async Task sendEmail(int Id)
        {
            //string Message = "Your Order  -" + Status + "  Sucessfully";
            var orderdata = _ordersRepository.All().Where(x => x.Id == Id).FirstOrDefault();
            var user = _customerDetailsRepository.All().Where(x => x.Id == orderdata.Customer_Id).FirstOrDefault();
            var productdata = _productrepository.All().Where(x => x.Id == orderdata.Product_id).FirstOrDefault();
            var puritydata = _purityRepository.All().Where(x => x.Id == productdata.PurityId).FirstOrDefault();
            var categorylabel = _materialCategoryRepository.All().Where(x => x.Id == productdata.Category_id).Select(x => x.Label).FirstOrDefault();
            var producttype = _productTypeRepository.All().Where(x => x.Id == productdata.ProductTypeId).FirstOrDefault();
            decimal totalsaleamount;
            decimal netamount;
            decimal cgstamount;
            decimal totalstwt;
            decimal totalnetwt;
            decimal totaltax;
            decimal sgstamount;
            decimal Making_Percentage;
            decimal Making_per_gram;
            decimal Making_Fixed_Amt;
            decimal totalgrwt;
            decimal grossTotalRate;
            decimal makingchrg;
            decimal MRP;
            if (productdata.MRP != 0)
            {
                var cgst = 1.5;
                var sgst = 1.5;

                MRP = Convert.ToDecimal(productdata.MRP) * Convert.ToDecimal(orderdata.Qty);
                cgstamount = (Convert.ToDecimal(cgst) * MRP) / 100;
                sgstamount = (Convert.ToDecimal(sgst) * MRP) / 100;
                totaltax = cgstamount + sgstamount;
                totalsaleamount = MRP * orderdata.Qty;
                totalnetwt = productdata.NetWt;
                var finalPrice = MRP + totaltax;
                grossTotalRate = totalsaleamount;
                netamount = finalPrice;
                totalgrwt = productdata.grosswt * orderdata.Qty;
                totalstwt = Convert.ToDecimal(productdata.StoneWeight) * orderdata.Qty;
                Making_Percentage = 0;
                Making_per_gram = 0;
                Making_Fixed_Amt = 0;
                makingchrg = 0;
            }
            else
            {
                var netGoldRate = (Convert.ToDecimal(productdata.NetWt) * Convert.ToDecimal(puritydata.TodaysRate)) / 10;
                var makingCharges1 = Convert.ToDecimal(productdata.NetWt) * Convert.ToDecimal(productdata.Making_per_gram);
                var makingCharges2 = (netGoldRate * Convert.ToDecimal(productdata.Making_Percentage)) / 100;
                var makingCharges3 = Convert.ToDecimal(productdata.Making_Fixed_Amt);
                var makingCharges4 = (Convert.ToDecimal(puritydata.TodaysRate) * Convert.ToDecimal(productdata.Making_Fixed_Wastage)) / 10;
                makingchrg = makingCharges1 + makingCharges2 + makingCharges3 + makingCharges4;
                var GST = 0.03;
                grossTotalRate = 1;
                grossTotalRate = netGoldRate + makingCharges1 + makingCharges2 + makingCharges3 + makingCharges4 + Convert.ToDecimal(productdata.StoneAmount);
                var GSTAdded = Convert.ToDecimal(GST) * grossTotalRate;
                var finalPrice = grossTotalRate + GSTAdded;

                //var productprice = (Convert.ToInt32(puritydata.TodaysRate) / 10) * productdata.NetWt;
                //var makingperc = productprice * Convert.ToInt32(productdata.Making_Percentage) / 100;
                //var total = productprice + makingperc;
                var cgst = 1.5;
                var sgst = 1.5;
                // var igst = 3;
                totalsaleamount = Convert.ToDecimal(grossTotalRate * orderdata.Qty);
                cgstamount = Convert.ToDecimal(cgst) * totalsaleamount / 100;
                sgstamount = Convert.ToDecimal(sgst) * totalsaleamount / 100;
                // var igstamount = Convert.ToDecimal(igst) * totalsaleamount / 100;
                totalnetwt = productdata.NetWt * orderdata.Qty;
                totalgrwt = productdata.grosswt * orderdata.Qty;
                totalstwt = Convert.ToDecimal(productdata.StoneWeight) * (Convert.ToDecimal(orderdata.Qty));

                totaltax = cgstamount + sgstamount;
                netamount = totalsaleamount + Convert.ToDecimal(totaltax);
                MRP = netamount;
            }
            var data = "";
            var imagePath = productdata.Images.Split(',');
            data = "https://jewellerywebapplications.blob.core.windows.net/images/" + imagePath[0];

            if (orderdata.OrderStatus == "Paid")
            {
                var viewPath = System.IO.Path.Combine(_environment.WebRootPath + "/Templates", "OrderConfirmation.html");
                var template = System.IO.File.ReadAllText(viewPath);

                template = template.Replace("XXXCallUrlXXX", "<p style=\"color:#ffffff\">For queries contact us</p>");
                template = template.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template = template.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddState + " " + user.currAddPinCode);
                template = template.Replace("XXXXemailXXX", user.Email);
                template = template.Replace("XXXXpincodeXXX", user.currAddPinCode);
                template = template.Replace("XXXXquantityXXX", orderdata.Qty.ToString());
                template = template.Replace("XXXXsizeXXX", productdata.Size);
                template = template.Replace("XXXXimagesXXX", data);
                template = template.Replace("XXXXorderXXX", orderdata.InvoiceNo);
                template = template.Replace("XXXXorderdateXXX", orderdata.CreatedOn.ToString("dd/MM/yyyy"));
                template = template.Replace("XXXXmobileXXX", user.Mobile);
                template = template.Replace("XXXXproductnameXXX", productdata.Product_Name);
                template = template.Replace("XXXXitemcodeXXX", productdata.ItemCode);
                //  template = template.Replace("XXXXorderidXXX", orderdata.Id.ToString());
                template = template.Replace("XXXnetamtXXX", netamount.ToString());
                template = template.Replace("XXXStatusXXX", orderdata.OrderStatus);
                template = template.Replace("XXXpriceXXX", MRP.ToString());
                template = template.Replace("XXXqtyXXX", orderdata.Qty.ToString());
                await _emailSender.SendEmailAsync(user.Email, "Order Confirmation", $"" + template + "");
            }
            else if (orderdata.OrderStatus == "Delivered")
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=jewellerywebapplications;AccountKey=ZaTHTvbGMADLIQLKM4ujTLLDwoCMDRGIIfeVBdjPN+taeuEmhqXV51R1IoCdFgOwdMRQRaZ7Axgr+AStBPFF2g==;EndpointSuffix=core.windows.net";
                string containerName = "template";
                string blobName = "Invoice.html";


                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                Response<BlobDownloadInfo> downloadResponse = await blobClient.DownloadAsync();
                var template1 = await new StreamReader(downloadResponse.Value.Content).ReadToEndAsync();
                var viewPath = System.IO.Path.Combine(_environment.WebRootPath + "/Templates", "OrderDelivered.html");
                //  var viewPath1 = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
                var template = System.IO.File.ReadAllText(viewPath);
                //var viewPath1 = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
                //  var viewPath1 = "https://jewellerywebapplications.blob.core.windows.net/template/Invoice.html";
                //   var template1 = System.IO.File.ReadAllText(viewPath1);
                //    var template1 = System.IO.File.ReadAllText(viewPath1);
                //  template = template.Replace("XXXABCXXX", Message);
                template = template.Replace("XXXCallUrlXXX", "<p style=\"color:#ffffff\">For queries contact us</p>");
                template = template.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template = template.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddPinCode);
                template = template.Replace("XXXXemailXXX", user.Email);
                template = template.Replace("XXXXpincodeXXX", user.currAddPinCode);
                template = template.Replace("XXXXquantityXXX", productdata.Quantity.ToString());
                template = template.Replace("XXXXsizeXXX", productdata.Size);
                template = template.Replace("XXXXimagesXXX", data);
                template = template.Replace("XXXXorderXXX", orderdata.InvoiceNo.ToString());
                template = template.Replace("XXXXorderdateXXX", orderdata.CreatedOn.ToString("dd/MM/yyyy"));
                template = template.Replace("XXXXmobileXXX", user.Mobile);
                template = template.Replace("XXXXproductnameXXX", productdata.Product_Name);
                template = template.Replace("XXXXitemcodeXXX", productdata.ItemCode);
                template = template.Replace("XXXXorderidXXX", orderdata.Id.ToString());
                template = template.Replace("XXXStatusXXX", orderdata.OrderStatus);
                template = template.Replace("XXXordervalueXXX", orderdata.ReceivedAmt.ToString());
                template = template.Replace("XXXpriceXXX", MRP.ToString());
                template = template.Replace("XXXqtyXXX", orderdata.Qty.ToString());
                template = template.Replace("XXXnetamtXXX", netamount.ToString());
                template1 = template1.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template1 = template1.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddState + " " + user.currAddPinCode);
                template1 = template1.Replace("XXXXemailXXX", user.Email);
                template1 = template1.Replace("XXXXpincodeXXX", user.currAddPinCode);
                //  template1 = template1.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template1 = template1.Replace("XXXXorderXXX", orderdata.InvoiceNo);
                template1 = template1.Replace("XXXXorderdateXXX", orderdata.CreatedOn.ToString("dd/MM/yyyy"));
                template1 = template1.Replace("XXXXmobileXXX", user.Mobile);
                template1 = template1.Replace("XXXXproductnameXXX", productdata.Category_Name + " " + productdata.Product_Name);
                template1 = template1.Replace("XXXXhsncodeXXX", producttype.HSNCode);
                template1 = template1.Replace("XXXXqtyXXX", orderdata.Qty.ToString());
                template1 = template1.Replace("XXXXpurityXXX", productdata.purity.ToString());
                template1 = template1.Replace("XXXXgrwtXXX", productdata.grosswt.ToString());
                template1 = template1.Replace("XXXXstonewtXXX", productdata.StoneWeight.ToString());
                template1 = template1.Replace("XXXXnetwtXXX", productdata.NetWt.ToString());
                template1 = template1.Replace("XXXXmkpercXXX", makingchrg.ToString());
                template1 = template1.Replace("XXXXsizeXXX", productdata.Size);
                template1 = template1.Replace("XXXXrateXXX", puritydata.TodaysRate);
                template1 = template1.Replace("XXXXtotalamountXXX", totalsaleamount.ToString());
                template1 = template1.Replace("XXXXvalueXXX", user.OrderValue);
                template1 = template1.Replace("XXXStatusXXX", user.OrderStatus);
                template1 = template1.Replace("XXXnetamtXXX", netamount.ToString());
                template1 = template1.Replace("XXXcgstXXX", cgstamount.ToString());
                template1 = template1.Replace("XXXsgstXXX", sgstamount.ToString());
                template1 = template1.Replace("XXXtotaltaxXXX", totaltax.ToString());
                template1 = template1.Replace("XXXXpincodeXXX", user.currAddPinCode);
                template1 = template1.Replace("XXXXtotalXXX", grossTotalRate.ToString());
                template1 = template1.Replace("XXXXinvoiceXXX", orderdata.Id.ToString());
                template1 = template1.Replace("XXXXtotalnetwtXXX", totalnetwt.ToString());
                template1 = template1.Replace("XXXXtotalgrwtXXX", totalgrwt.ToString());
                template1 = template1.Replace("XXXXtotalstwtXXX", totalstwt.ToString());
                //template1 = template1.Replace("XXXXinvoiceXXX", orderdata.Id.ToString());
                //try
                //{
                byte[] pdfBytes;
                var htmlContent = template1;
                using (MemoryStream stream = new MemoryStream())
                {
                    if (string.IsNullOrEmpty(htmlContent))
                    {
                        throw new Exception("HTML content is null or empty.");
                    }

                    ConverterProperties converterProperties = new ConverterProperties();
                    HtmlConverter.ConvertToPdf(htmlContent, stream, converterProperties);

                    pdfBytes = stream.ToArray();
                }
                var sender = new MailAddress("info@mkgharejewellers.com", "Ghare Jewellers");
                var subject = "Order Delivered";
                var client = new SmtpClient(_emailSettings.MailServer)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password),
                    Port = _emailSettings.MailPort,
                    EnableSsl = _emailSettings.SSL
                };
                var mailMessage = new MailMessage
                {
                    From = sender,
                };

                var attachment = new Attachment(new MemoryStream(pdfBytes), "Invoice.pdf", "application/pdf");
                mailMessage.Attachments.Add(attachment);
                mailMessage.To.Add(user.Email);
                mailMessage.Subject = subject;
                mailMessage.Body = template;
                mailMessage.IsBodyHtml = true;

                await client.SendMailAsync(mailMessage);
                // Email sent successfully
                //}
                //catch (Exception ex)
                //{

                //    return ex.Message;
                //}

            }
            else if (orderdata.OrderStatus == "Payment Failed")
            {
                var viewPath = System.IO.Path.Combine(_environment.WebRootPath + "/Templates", "PaymentFailed.html");
                //   var viewPath1 = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
                var template = System.IO.File.ReadAllText(viewPath);


                //    var template1 = System.IO.File.ReadAllText(viewPath1);
                //  template = template.Replace("XXXABCXXX", Message);

                template = template.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template = template.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddState + " " + user.currAddPinCode);
                template = template.Replace("XXXXemailXXX", user.Email);
                template = template.Replace("XXXXpincodeXXX", user.currAddPinCode);
                template = template.Replace("XXXXqtyXXX", orderdata.Qty.ToString());
                template = template.Replace("XXXXsizeXXX", productdata.Size);
                template = template.Replace("XXXXimagesXXX", data);
                template = template.Replace("XXXXorderXXX", orderdata.InvoiceNo.ToString());
                template = template.Replace("XXXXorderdateXXX", user.CreatedOn.ToString("dd/MM/yyyy"));
                template = template.Replace("XXXXmobileXXX", user.Mobile);
                template = template.Replace("XXXXproductnameXXX", productdata.Product_Name);
                template = template.Replace("XXXXitemcodeXXX", productdata.ItemCode);
                template = template.Replace("XXXXorderidXXX", orderdata.Id.ToString());
                template = template.Replace("XXXStatusXXX", orderdata.OrderStatus);
                template = template.Replace("XXXordervalueXXX", orderdata.ReceivedAmt.ToString());
                template = template.Replace("XXXpriceXXX", MRP.ToString());

                await _emailSender.SendEmailAsync(user.Email, "Payment Failure - Order #" + orderdata.Id + "", $"" + template + "");
            }
            else if (orderdata.OrderStatus == "Shipped")
            {
                var viewPath = System.IO.Path.Combine(_environment.WebRootPath + "/Templates", "orderOnTheWay.html");
                //   var viewPath1 = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
                var template = System.IO.File.ReadAllText(viewPath);


                //    var template1 = System.IO.File.ReadAllText(viewPath1);
                //  template = template.Replace("XXXABCXXX", Message);

                template = template.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template = template.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddState);
                template = template.Replace("XXXXemailXXX", user.Email);
                template = template.Replace("XXXXpincodeXXX", user.currAddPinCode);
                template = template.Replace("XXXXquantityXXX", productdata.Quantity.ToString());
                template = template.Replace("XXXXsizeXXX", productdata.Size);
                template = template.Replace("XXXXimagesXXX", data);
                template = template.Replace("XXXXorderXXX", orderdata.InvoiceNo.ToString());
                template = template.Replace("XXXXorderdateXXX", orderdata.CreatedOn.ToString("dd/MM/yyyy"));
                template = template.Replace("XXXXmobileXXX", user.Mobile);
                template = template.Replace("XXXXproductnameXXX", productdata.Product_Name);
                template = template.Replace("XXXXitemcodeXXX", productdata.ItemCode);
                template = template.Replace("XXXXorderidXXX", orderdata.Id.ToString());
                template = template.Replace("XXXStatusXXX", orderdata.OrderStatus);
                template = template.Replace("XXXordervalueXXX", orderdata.ReceivedAmt.ToString());
                template = template.Replace("XXXXtotalamountXXX", totalsaleamount.ToString());
                template = template.Replace("XXXXqtyXXX", orderdata.Qty.ToString());
                template = template.Replace("XXXpriceXXX", MRP.ToString());
                template = template.Replace("XXXnetamtXXX", netamount.ToString());
                await _emailSender.SendEmailAsync(user.Email, "Your Order Has Been Shipped!", $"" + template + "");
            }
        }
        [HttpPost("DownloadPDF")]
        public async Task<IActionResult> DownloadPDF(tblOrder model)
        {
            var orderdata = _ordersRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            var user = _customerDetailsRepository.All().Where(x => x.Id == orderdata.Customer_Id).FirstOrDefault();
            var productdata = _productrepository.All().Where(x => x.Id == orderdata.Product_id).FirstOrDefault();
            var puritydata = _purityRepository.All().Where(x => x.Id == productdata.PurityId).FirstOrDefault();
            var producttype = _productTypeRepository.All().Where(x => x.Id == productdata.ProductTypeId).FirstOrDefault();
            var categorylabel = _materialCategoryRepository.All().Where(x => x.Id == productdata.Category_id).Select(x => x.Label).FirstOrDefault();
            decimal totalsaleamount;
            decimal netamount;
            decimal cgstamount;
            decimal totalstwt;
            decimal totalnetwt;
            decimal totaltax;
            decimal sgstamount;
            decimal Making_Percentage;
            decimal Making_per_gram;
            decimal Making_Fixed_Amt;
            decimal totalgrwt;
            decimal grossTotalRate;
            decimal makingchrg;
            decimal MRP;
            if (productdata.MRP != 0)
            {
                var cgst = 1.5;
                var sgst = 1.5;

                MRP = Convert.ToDecimal(productdata.MRP) * Convert.ToDecimal(orderdata.Qty);
                cgstamount = (Convert.ToDecimal(cgst) * MRP) / 100;
                sgstamount = (Convert.ToDecimal(sgst) * MRP) / 100;
                totaltax = cgstamount + sgstamount;
                totalsaleamount = MRP * orderdata.Qty;
                totalnetwt = productdata.NetWt;
                var finalPrice = MRP + totaltax;
                grossTotalRate = totalsaleamount;
                netamount = finalPrice;
                totalgrwt = productdata.grosswt * orderdata.Qty;
                totalstwt = Convert.ToDecimal(productdata.StoneWeight) * orderdata.Qty;
                Making_Percentage = 0;
                Making_per_gram = 0;
                Making_Fixed_Amt = 0;
                makingchrg = 0;
            }
            else
            {
                var netGoldRate = (Convert.ToDecimal(productdata.NetWt) * Convert.ToDecimal(puritydata.TodaysRate)) / 10;
                var makingCharges1 = Convert.ToDecimal(productdata.NetWt) * Convert.ToDecimal(productdata.Making_per_gram);
                var makingCharges2 = (netGoldRate * Convert.ToDecimal(productdata.Making_Percentage)) / 100;
                var makingCharges3 = Convert.ToDecimal(productdata.Making_Fixed_Amt);
                var makingCharges4 = (Convert.ToDecimal(puritydata.TodaysRate) * Convert.ToDecimal(productdata.Making_Fixed_Wastage)) / 10;
                makingchrg = makingCharges1 + makingCharges2 + makingCharges3 + makingCharges4;
                var GST = 0.03;
                grossTotalRate = 1;

                grossTotalRate = netGoldRate + makingCharges1 + makingCharges2 + makingCharges3 + makingCharges4 + Convert.ToDecimal(productdata.StoneAmount);
                var GSTAdded = Convert.ToDecimal(GST) * grossTotalRate;
                var finalPrice = grossTotalRate + GSTAdded;

                //var productprice = (Convert.ToInt32(puritydata.TodaysRate) / 10) * productdata.NetWt;
                //var makingperc = productprice * Convert.ToInt32(productdata.Making_Percentage) / 100;
                //var total = productprice + makingperc;
                var cgst = 1.5;
                var sgst = 1.5;
                // var igst = 3;
                totalsaleamount = Convert.ToDecimal(grossTotalRate * orderdata.Qty);
                cgstamount = Convert.ToDecimal(cgst) * totalsaleamount / 100;
                sgstamount = Convert.ToDecimal(sgst) * totalsaleamount / 100;
                // var igstamount = Convert.ToDecimal(igst) * totalsaleamount / 100;
                totalnetwt = productdata.NetWt * orderdata.Qty;
                totalgrwt = productdata.grosswt * orderdata.Qty;
                totalstwt = Convert.ToDecimal(productdata.StoneWeight) * (Convert.ToDecimal(orderdata.Qty));

                totaltax = cgstamount + sgstamount;
                netamount = totalsaleamount + Convert.ToDecimal(totaltax);
                MRP = netamount;
            }
            var data = "";
            var imagePath = productdata.Images.Split(',');
            data = "https://jewellerywebapplications.blob.core.windows.net/images/" + imagePath[0];
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=jewellerywebapplications;AccountKey=ZaTHTvbGMADLIQLKM4ujTLLDwoCMDRGIIfeVBdjPN+taeuEmhqXV51R1IoCdFgOwdMRQRaZ7Axgr+AStBPFF2g==;EndpointSuffix=core.windows.net";
            string containerName = "template";
            string blobName = "Invoice.html";


            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            Response<BlobDownloadInfo> downloadResponse = await blobClient.DownloadAsync();
            var template1 = await new StreamReader(downloadResponse.Value.Content).ReadToEndAsync();
            //var viewPath = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
            //  var viewPath1 = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
            //   var template1 = System.IO.File.ReadAllText(viewPath);
            template1 = template1.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
            template1 = template1.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddState);
            template1 = template1.Replace("XXXXemailXXX", user.Email);
            template1 = template1.Replace("XXXXpincodeXXX", user.currAddPinCode);
            template1 = template1.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
            template1 = template1.Replace("XXXXorderXXX", orderdata.InvoiceNo);
            template1 = template1.Replace("XXXXorderdateXXX", user.LastUpdated.ToString("dd/MM/yyyy"));
            template1 = template1.Replace("XXXXmobileXXX", user.Mobile);
            template1 = template1.Replace("XXXXproductnameXXX", productdata.Category_Name + " " + productdata.Product_Name);
            template1 = template1.Replace("XXXXhsncodeXXX", producttype.HSNCode);
            template1 = template1.Replace("XXXXqtyXXX", orderdata.Qty.ToString());
            template1 = template1.Replace("XXXXpurityXXX", productdata.purity.ToString());
            template1 = template1.Replace("XXXXgrwtXXX", productdata.grosswt.ToString());
            template1 = template1.Replace("XXXXstonewtXXX", productdata.StoneWeight.ToString());
            template1 = template1.Replace("XXXXnetwtXXX", productdata.NetWt.ToString());
            template1 = template1.Replace("XXXXmkpercXXX", makingchrg.ToString());
            template1 = template1.Replace("XXXXsizeXXX", productdata.Size);
            template1 = template1.Replace("XXXXrateXXX", puritydata.TodaysRate);
            template1 = template1.Replace("XXXXtotalamountXXX", totalsaleamount.ToString());
            template1 = template1.Replace("XXXXvalueXXX", user.OrderValue);
            template1 = template1.Replace("XXXStatusXXX", user.OrderStatus);
            template1 = template1.Replace("XXXnetamtXXX", netamount.ToString());
            template1 = template1.Replace("XXXcgstXXX", cgstamount.ToString());
            template1 = template1.Replace("XXXsgstXXX", sgstamount.ToString());
            template1 = template1.Replace("XXXtotaltaxXXX", totaltax.ToString());
            template1 = template1.Replace("XXXXpincodeXXX", user.currAddPinCode);
            template1 = template1.Replace("XXXXtotalXXX", grossTotalRate.ToString());
            template1 = template1.Replace("XXXXinvoiceXXX", orderdata.Id.ToString());
            template1 = template1.Replace("XXXXtotalnetwtXXX", totalnetwt.ToString());
            template1 = template1.Replace("XXXXtotalgrwtXXX", totalgrwt.ToString());
            template1 = template1.Replace("XXXXtotalstwtXXX", totalstwt.ToString());
            template1 = template1.Replace("XXXXpaymentmodeXXX", orderdata.PaymentMode.ToString());

            byte[] pdfBytes;
            var htmlContent = template1;
            using (MemoryStream stream = new MemoryStream())
            {
                if (string.IsNullOrEmpty(htmlContent))
                {
                    throw new Exception("HTML content is null or empty.");
                }

                ConverterProperties converterProperties = new ConverterProperties();

                HtmlConverter.ConvertToPdf(htmlContent, stream, converterProperties);

                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", "downloaded.pdf");
        }



        [HttpPost("UpdateRates")]
        public async Task<IActionResult> UpdateRates(tblRates model)
        {
            var ratedata = _rateRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (ratedata != null)
            {
                //ratedata.Category_id = model.Category_id;
                //ratedata.Category_Label = model.Category_Label;
                //ratedata.Purity = model.Purity;
                //rate.todaysrate = model.todaysrate;

                if (model.Category_id != 0)
                {
                    ratedata.Category_id = model.Category_id;
                }

                else if (model.Category_Label != null)
                {
                    ratedata.Category_Label = model.Category_Label;
                }
                else if (model.Purity != null)
                {
                    ratedata.Purity = model.Purity;
                }

                else if (model.todaysrate != 0)
                {
                    ratedata.todaysrate = model.todaysrate;
                }
                else if (model.OnlineStatus != null)
                {
                    ratedata.OnlineStatus = "Active";
                }

                await _rateRepository.UpdateAsync(ratedata, ratedata.Id);

                return Ok(new { Status = "Success", data = ratedata });
            }
            return BadRequest();
        }



        [HttpPost("PaymentRequest")]
        public async Task<IActionResult> PaymentRequest(PaymentViewModel model)
        {
            var total = model.amount * 100;
            string uniqueId = Guid.NewGuid().ToString();
            string orderNumber = model.orderId;
            int customerId = Convert.ToInt32(model.id);
            var payload = new
            {
                merchantId = "GHAREONLINE",
                merchantTransactionId = uniqueId /*"MT7850590068188112"*/,
                merchantUserId = "MUID123",
                amount = total,

                callbackUrl = $"https://www.mkgharejewellers.com/paymentfailedpage?orderId={orderNumber}&custId={customerId}",
                redirectUrl = $"https://www.mkgharejewellers.com/paymentsuccesspage?orderId={orderNumber}&custId={customerId}",
                mobileNumber = model.mobile,
                paymentInstrument = new
                {
                    type = "PAY_PAGE"
                }
            };

            var saltKey = "cdca9558-eb96-46c7-bed3-f96abfdd044d";
            var saltIndex = 1;
            var base64Payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
            var xVerifyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(base64Payload + "/pg/v1/pay" + saltKey));
            var xVerify = BitConverter.ToString(xVerifyBytes).Replace("-", "").ToLower() + "###" + saltIndex;


            var requestPayload = new { request = base64Payload };
            var jsonRequest = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {

                // client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("X-VERIFY", xVerify);
                var response = await client.PostAsync("https://api.phonepe.com/apis/hermes/pg/v1/pay", content);
                var jsonResponse = await response.Content.ReadAsStringAsync();



                if (response.IsSuccessStatusCode)
                {


                    // Handle the successful response from PhonePe API
                    return Ok(jsonResponse);
                }
                else
                {
                    // Handle the error response from PhonePe API
                    return Ok(new { messege = "An error occurred:", data = jsonResponse });
                }
            }
        }


        [HttpGet("PaymentStatus")]
        public async Task<IActionResult> PaymentStatus(PaymentStatusViewModel model)
        {

            var payload = new
            {
                merchantId = "GHAREONLINE",
                merchantTransactionId = model.merchantTransactionId,
                merchantUserId = "MUID123",
                amount = model.amount,
                callbackUrl = "https://www.mkgharejewellers.com/",
                mobileNumber = "9819662720",
                paymentInstrument = new
                {
                    type = "UPI"
                }
            };

            var saltKey = "cdca9558-eb96-46c7-bed3-f96abfdd044d";
            var saltIndex = 1;
            var base64Payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
            var xVerifyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(base64Payload + "/pg/v1/pay" + saltKey));
            var xVerify = BitConverter.ToString(xVerifyBytes).Replace("-", "").ToLower() + "###" + saltIndex;


            var requestPayload = new { request = base64Payload };
            var jsonRequest = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {

                // client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("X-VERIFY", xVerify);
                var response = await client.PostAsync("https://api.phonepe.com/apis/hermes/pg/v1/status/{merchantId}/{merchantTransactionId}", content);
                var jsonResponse = await response.Content.ReadAsStringAsync();



                if (response.IsSuccessStatusCode)
                {
                    // Handle the successful response from PhonePe API
                    return Ok(jsonResponse);
                }
                else
                {
                    // Handle the error response from PhonePe API
                    return Ok(new { messege = "An error occurred:", data = jsonResponse });
                }
            }
        }

        [HttpPost("BulkInsertOrders")]
        public async Task<IActionResult> BulkInsertOrders(tblOrder model)
        {
            var productdata = _productrepository.All().Include(x => x.tblMaterialCategory).Where(x => x.Id == model.Product_id).FirstOrDefault();
            var puritydata = _purityRepository.All().Where(x => x.Id == productdata.PurityId).FirstOrDefault();
            var emaildata = _customerDetailsRepository.All().Where(x => x.Id == model.Customer_Id).FirstOrDefault();
            string productlabel = _materialCategoryRepository.All().Where(x => x.Id == productdata.Category_id).Select(X => X.Name).FirstOrDefault();

            int count = _ordersRepository.All().Where(x => x.CategoryName.Contains(productlabel)).Count();
            char categorycode = productlabel[0];
            int x = count + 1;
            Random generator = new Random();
            int r = generator.Next(100000, 1000000);
            var orderNumber = categorycode + r.ToString();

            DateTime currentDate = DateTime.Now;
            int startMonth = 4;
            int currentMonth = currentDate.Month;
            int year;
            if (currentMonth >= startMonth)
            {
                year = currentDate.Year;
            }
            else
            {
                year = currentDate.Year - 1;
            }
            int endYear = year + 1;
            string financialYear = $"{year}-{endYear}";
            var invoiceno = categorycode + " " + x + "/FY" + financialYear;

            if (ModelState.IsValid)
            {

                List<tblOrder> orders = new List<tblOrder>();

                foreach (var li in orders)
                {
                    tblOrder order = new tblOrder();
                    order.orderNumber = orderNumber.ToString();
                    order.InvoiceNo = invoiceno;
                    order.Customer_Id = li.Customer_Id;
                    order.PaymentMode = li.PaymentMode;
                    order.Offer = li.Offer;
                    order.GovtTax = li.GovtTax;
                    order.ReceivedAmt = li.ReceivedAmt;
                    order.OnlineStatus = "Active";
                    order.OrderStatus = li.OrderStatus;
                    //foreach (var product in order.Products)
                    //{
                    //    var productdetails = _productrepository.All().Include(x => x.tblMaterialCategory).Where(x => x.Id == product.Product_id).FirstOrDefault();
                    //    var puritydetails = _purityRepository.All().Where(x => x.Id == productdetails.PurityId).FirstOrDefault();
                    //    order.Product_id = productdetails.Id;
                    //    order.Qty = productdetails.Quantity;
                    //    order.MRP = productdetails.MRP.ToString();
                    //    order.Rate = puritydetails.TodaysRate;
                    //    order.GrossWt = productdetails.grosswt.ToString();
                    //    order.NetWt = productdetails.NetWt.ToString();
                    //    order.StoneWt = productdetails.StoneWeight;
                    //    order.CategoryName = productdata.Category_Name;
                    //}
                    orders.Add(order);
                }
                await _ordersRepository.BulkInsertAsync(orders);
                return Ok(new { Status = "Success", data = orders });
            }

            return BadRequest();

        }

        [HttpPost("BulkUpdateOrders")]
        public async Task<IActionResult> BulkUpdateOrders(List<tblOrder> list)
        {

            if (ModelState.IsValid)
            {
                List<tblOrder> orders = new List<tblOrder>();
                foreach (var li in list)
                {
                    var orderdata = _ordersRepository.All().Where(x => x.orderNumber == li.orderNumber).FirstOrDefault();
                    if (orderdata != null)
                    {
                        if (li.OrderStatus == "Paid")
                        {
                            orderdata.OrderStatus = "Paid";
                            await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);
                        }
                        else if (li.OrderStatus == "Delivered")
                        {
                            orderdata.OrderStatus = "Delivered";
                            await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);
                        }
                        else if (li.OrderStatus == "Payment Failed")
                        {
                            orderdata.OrderStatus = "Payment Failed";
                            await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);
                        }
                        else if (li.OrderStatus == "Shipped")
                        {
                            orderdata.OrderStatus = "Shipped";
                            await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);
                        }
                    }
                }
                //  await sendEmailmultipleproduct(list);
                return Ok(new { Status = "Success", data = list });

            }

            return BadRequest();
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(tblOrder model)
        {
            var productdata = _productrepository.All().Include(x => x.tblMaterialCategory).Where(x => x.Id == model.Product_id).FirstOrDefault();
            var emaildata = _customerDetailsRepository.All().Where(x => x.Id == model.Customer_Id).FirstOrDefault();

            string productlabel = _materialCategoryRepository.All().Where(x => x.Id == productdata.Category_id).Select(X => X.Name).FirstOrDefault();

            int count = _ordersRepository.All().Where(x => x.CategoryName.Contains(productlabel)).Count();
            char categorycode = productlabel[0];
            int x = count + 1;
            Random generator = new Random();
            int r = generator.Next(100000, 1000000);
            var orderNumber = categorycode + r.ToString();

            DateTime currentDate = DateTime.Now;
            int startMonth = 4;
            int currentMonth = currentDate.Month;
            int year;
            if (currentMonth >= startMonth)
            {
                year = currentDate.Year;
            }
            else
            {
                year = currentDate.Year - 1;
            }
            int endYear = year + 1;
            string financialYear = $"{year}-{endYear}";
            var invoiceno = categorycode + " " + x + "/FY" + financialYear;

            tblOrder tblOrders = new tblOrder();
            tblOrders.OrderStatus = model.OrderStatus;
            tblOrders.DeliveryAddress = emaildata.currAddStreet + emaildata.currAddTown + emaildata.currAddState + emaildata.currAddPinCode;
            tblOrders.orderNumber = orderNumber;
            tblOrders.InvoiceNo = invoiceno;
            tblOrders.CategoryName = model.CategoryName;
            tblOrders.Customer_Id = model.Customer_Id;
            tblOrders.GovtTax = model.GovtTax;
            tblOrders.Offer = model.Offer;
            tblOrders.PaymentMode = model.PaymentMode;
           // tblOrders.Price = model.Price;
            tblOrders.Qty = model.Qty;
            tblOrders.OnlineStatus = "Active";
           // tblOrders.Product_id = model.Product_id;
            tblOrders.ReceivedAmt = model.ReceivedAmt;

            await _ordersRepository.InsertAsync(tblOrders);

            return Ok(new { status = "Success", data = tblOrders });
        }
        [HttpPost("CreateOrderItem")]
        public async Task<IActionResult> CreateOrderItem(List<OrderItemDetails> orderitem)
        {
            List<OrderItemDetails> itemDetails = new List<OrderItemDetails>();
            foreach (var list in orderitem)
            {
                var orders = _ordersRepository.All().Where(x => x.Id == list.OrderId).FirstOrDefault();
                var products = _productrepository.All().Where(x => x.Id == list.ProductId).FirstOrDefault();
                var purities = _purityRepository.All().Where(x => x.Id == products.PurityId).FirstOrDefault();
                var producttype = _productTypeRepository.All().Where(x => x.Id == products.ProductTypeId).FirstOrDefault();
                OrderItemDetails orderItem = new OrderItemDetails();
                orderItem.OrderId = list.OrderId;
                orderItem.OrderNumber = orders.orderNumber;
                orderItem.ProductId = list.ProductId;
                orderItem.MRP = list.MRP;
                orderItem.Customer_Id = orders.Customer_Id;
                orderItem.CategoryName = products.Category_Name;
              //  orderItem.OrderStatus = list.OrderStatus;
                orderItem.ProductName = list.ProductName;
                orderItem.Quantity = list.Quantity;
                orderItem.HSNCode = producttype.HSNCode;
                orderItem.ItemCode = products.ItemCode;
                orderItem.grosswt = list.grosswt;
                orderItem.NetWt = list.NetWt;
                orderItem.StoneWeight = list.StoneWeight;
                orderItem.StoneAmount = list.StoneAmount;
                orderItem.Purity = purities.Purity;
                orderItem.makingchrg = list.makingchrg;
                orderItem.Rate = list.Rate;
                itemDetails.Add(orderItem);

            }
            await _ordersItemDetailsRepository.BulkInsertAsync(itemDetails);
            return Ok(new { status = "Success", data = itemDetails });

        }

        [HttpPost("DownloadInvoice")]
        public async Task<IActionResult> DownloadInvoice(tblOrder model)
        {
            var orderdata = _ordersRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            var orderitem = _ordersItemDetailsRepository.All().Where(x => x.OrderNumber == orderdata.orderNumber).ToList();


            var user = _customerDetailsRepository.All().Where(x => x.Id == orderdata.Customer_Id).FirstOrDefault();
            var productdata = _productrepository.All().Where(x => x.Id == orderdata.Product_id).FirstOrDefault();
      
            var producttype = _productTypeRepository.All().Where(x => x.Id == productdata.ProductTypeId).FirstOrDefault();
            var categorylabel = _materialCategoryRepository.All().Where(x => x.Id == productdata.Category_id).Select(x => x.Label).FirstOrDefault();
            var itemcode = "";
            var productListHtml = "";
            var Quantity = "";
            var puritydata1 = "";
            var grosswt = "";
            var StoneWeight = "";
            var NetWt = "";
            var makingcharg = "";
            var Rate = "";
            var Total = "";
            decimal totalgrwt = 0;
            decimal totalnetwt = 0;
            decimal totalstwt = 0;
            decimal totalsaleamount =0 ;
            decimal netamount=0;
            decimal cgstamount=0;
            // decimal totalstwt;
            //decimal totalnetwt;
            decimal totaltax=0;
            decimal sgstamount = 0;
            decimal Making_Percentage;
            decimal Making_per_gram;
            decimal Making_Fixed_Amt;
            //  decimal totalgrwt;
            decimal grossTotalRate;
            decimal makingchrg;
            decimal MRP;
            decimal total;
            var cgst = 1.5;
            var sgst = 1.5;
          
            //totaltax = cgstamount + sgstamount;
            //netamount = totalsaleamount+totaltax;

         
           
            //var data = "";
            //var imagePath = productdata.Images.Split(',');
            //data = "https://jewellerywebapplications.blob.core.windows.net/images/" + imagePath[0];
            //string connectionString = "DefaultEndpointsProtocol=https;AccountName=jewellerywebapplications;AccountKey=ZaTHTvbGMADLIQLKM4ujTLLDwoCMDRGIIfeVBdjPN+taeuEmhqXV51R1IoCdFgOwdMRQRaZ7Axgr+AStBPFF2g==;EndpointSuffix=core.windows.net";
            //string containerName = "template";
            //string blobName = "Invoice.html";


            //BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            //BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            //BlobClient blobClient = containerClient.GetBlobClient(blobName);

            //Response<BlobDownloadInfo> downloadResponse = await blobClient.DownloadAsync();
            //  var template1 = await new StreamReader(downloadResponse.Value.Content).ReadToEndAsync();
            //var viewPath = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
            var viewPath1 = System.IO.Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
            var template1 = System.IO.File.ReadAllText(viewPath1);

            foreach (var item in orderitem)
            {
                if (item.MRP != null && item.MRP != "")
                {
                    var productsdata = _productrepository.All().Where(x => x.Id == item.ProductId).FirstOrDefault();
                    itemcode += "<p>" + item.HSNCode + "</p>";
                    productListHtml += "<p>" + item.CategoryName + " " + item.ProductName + "</p>";
                    Quantity += "<p>" + item.Quantity + "</p>";
                    puritydata1 += "<p>" + item.Purity + "</p>";
                    grosswt += "<p>" + item.grosswt + "</p>";
                    StoneWeight += "<p>" + item.StoneWeight + "</p>";
                    NetWt += "<p>" + item.NetWt + "</p>";
                    makingcharg += "<p>" + item.makingchrg + "</p>";
                    Rate += "<p>" + item.Rate + "</p>";
                    Total += "<p>" + Convert.ToDecimal(item.MRP) * Convert.ToDecimal(item.Quantity) + "</p>";
                    totalnetwt = totalnetwt + Convert.ToDecimal(item.NetWt);
                    if (item.StoneWeight != null && item.StoneWeight != "")
                    {
                        totalstwt = totalstwt + Convert.ToDecimal(item.StoneWeight);
                    }

                    else
                    {
                        totalstwt = totalstwt + 0;
                    }
                    totalgrwt = totalgrwt + Convert.ToDecimal(item.grosswt);
                    totalsaleamount = totalsaleamount + (Convert.ToDecimal(item.MRP) * Convert.ToDecimal(item.Quantity));
                    cgstamount = (Convert.ToDecimal(cgst) * totalsaleamount) / 100;
                    sgstamount = (Convert.ToDecimal(sgst) * totalsaleamount) / 100;
                    totaltax = cgstamount + sgstamount;
                    netamount = totalsaleamount + totaltax;
                }
                else
                {
                    var productsdata = _productrepository.All().Where(x => x.Id == item.ProductId).FirstOrDefault();
                    var purities = _purityRepository.All().Where(x => x.Id == productsdata.PurityId).FirstOrDefault();
                    var netGoldRate = (Convert.ToDecimal(productsdata.NetWt) * Convert.ToDecimal(item.Rate)) / 10;
                    var makingCharges1 = Convert.ToDecimal(item.NetWt) * Convert.ToDecimal(productsdata.Making_per_gram);
                    var makingCharges2 = (netGoldRate * Convert.ToDecimal(productsdata.Making_Percentage)) / 100;
                    var makingCharges3 = Convert.ToDecimal(productsdata.Making_Fixed_Amt);
                    var makingCharges4 = (Convert.ToDecimal(purities.TodaysRate) * Convert.ToDecimal(productsdata.Making_Fixed_Wastage)) / 10;
                    makingchrg = makingCharges1 + makingCharges2 + makingCharges3 + makingCharges4;
                   
                    Total = (netGoldRate + makingCharges1 + makingCharges2 + makingCharges3 + makingCharges4 + Convert.ToDecimal(item.StoneAmount)).ToString();

                    totalsaleamount = totalsaleamount + Convert.ToDecimal(Total);
                    cgstamount = (Convert.ToDecimal(cgst) * totalsaleamount) / 100;
                    sgstamount = (Convert.ToDecimal(sgst) * totalsaleamount) / 100;
                    totaltax = cgstamount + sgstamount;
                    netamount = totalsaleamount + totaltax;

                }
            }

            template1 = template1.Replace("XXXXproductnameXXX", productListHtml);
            template1 = template1.Replace("XXXXhsncodeXXX", itemcode);
            template1 = template1.Replace("XXXXqtyXXX", Quantity.ToString());
            template1 = template1.Replace("XXXXpurityXXX", puritydata1.ToString());
            template1 = template1.Replace("XXXXgrwtXXX", grosswt.ToString());
            template1 = template1.Replace("XXXXstonewtXXX", StoneWeight.ToString());
            template1 = template1.Replace("XXXXnetwtXXX", NetWt.ToString());
            template1 = template1.Replace("XXXXmkpercXXX", makingcharg.ToString());
            //     template1 = template1.Replace("XXXXsizeXXX", productsdata.Size);
            template1 = template1.Replace("XXXXrateXXX", Rate);

            template1 = template1.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
            template1 = template1.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddState);
            template1 = template1.Replace("XXXXemailXXX", user.Email);
            template1 = template1.Replace("XXXXpincodeXXX", user.currAddPinCode);
            //   template1 = template1.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
            template1 = template1.Replace("XXXXorderXXX", orderdata.InvoiceNo);
            template1 = template1.Replace("XXXXorderdateXXX", user.CreatedOn.ToString("dd/MM/yyyy"));
            template1 = template1.Replace("XXXXmobileXXX", user.Mobile);

            template1 = template1.Replace("XXXXtotalamountXXX", totalsaleamount.ToString());
            //template1 = template1.Replace("XXXXvalueXXX", user.OrderValue);
            //template1 = template1.Replace("XXXStatusXXX", user.OrderStatus);
            template1 = template1.Replace("XXXnetamtXXX", netamount.ToString());
            template1 = template1.Replace("XXXcgstXXX", cgstamount.ToString());
            template1 = template1.Replace("XXXsgstXXX", sgstamount.ToString());
            template1 = template1.Replace("XXXtotaltaxXXX", totaltax.ToString());
            template1 = template1.Replace("XXXXpincodeXXX", user.currAddPinCode);
            template1 = template1.Replace("XXXXtotalXXX", Total.ToString());
            //        template1 = template1.Replace("XXXXinvoiceXXX", orderdata.Id.ToString());
            template1 = template1.Replace("XXXXtotalnetwtXXX", totalnetwt.ToString());
            template1 = template1.Replace("XXXXtotalgrwtXXX", totalgrwt.ToString());
            template1 = template1.Replace("XXXXtotalstwtXXX", totalstwt.ToString());
            template1 = template1.Replace("XXXXpaymentmodeXXX", orderdata.PaymentMode.ToString());

            byte[] pdfBytes;
            var htmlContent = template1;
            using (MemoryStream stream = new MemoryStream())
            {
                if (string.IsNullOrEmpty(htmlContent))
                {
                    throw new Exception("HTML content is null or empty.");
                }

                ConverterProperties converterProperties = new ConverterProperties();

                HtmlConverter.ConvertToPdf(htmlContent, stream, converterProperties);

                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", "downloaded.pdf");
        }

        [HttpPost("UpdateBulkeOrders")]
        public async Task<IActionResult> UpdatBulkeOrders(tblOrder model)
        {
            var orderdata = _ordersRepository.All().Where(x => x.orderNumber == model.orderNumber).FirstOrDefault();
            if (orderdata != null)
            {
                if (model.OrderStatus == "Paid")
                {
                    orderdata.OrderStatus = "Paid";
                    await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);
                    await sendEmails(orderdata.Id);
                    return Ok(new { Status = "Success", data = orderdata });
                }

                else if (model.OrderStatus == "Delivered")
                {

                    orderdata.OrderStatus = "Delivered";
                    await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);

                    await sendEmails(orderdata.Id);
                    return Ok(new { Status = "Success", data = orderdata });
                }
                else if (model.OrderStatus == "Payment Failed")
                {
                    orderdata.OrderStatus = "Payment Failed";
                    await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);
                    await sendEmails(orderdata.Id);
                    return Ok(new { Status = "Success", data = orderdata });
                }
                else if (model.OrderStatus == "Shipped")
                {

                    orderdata.OrderStatus = "Shipped";
                    await _ordersRepository.UpdateAsync(orderdata, orderdata.Id);
                    await sendEmails(orderdata.Id);
                    return Ok(new { Status = "Success", data = orderdata });

                }

            }


            return BadRequest();
        }
        private async Task sendEmails(int Id)
        {
            //string Message = "Your Order  -" + Status + "  Sucessfully";
            var orderdata = _ordersRepository.All().Where(x => x.Id == Id).FirstOrDefault();
            var orderitem = _ordersItemDetailsRepository.All().Where(x => x.OrderNumber == orderdata.orderNumber).ToList();
            var user = _customerDetailsRepository.All().Where(x => x.Id == orderdata.Customer_Id).FirstOrDefault();
            var productdata = _productrepository.All().Where(x => x.Id == orderdata.Product_id).FirstOrDefault();
            var puritydata = _purityRepository.All().Where(x => x.Id == productdata.PurityId).FirstOrDefault();
            var categorylabel = _materialCategoryRepository.All().Where(x => x.Id == productdata.Category_id).Select(x => x.Label).FirstOrDefault();
            var producttype = _productTypeRepository.All().Where(x => x.Id == productdata.ProductTypeId).FirstOrDefault();
            var itemcode = "";
            var productListHtml = "";
            var Quantity = "";
            var puritydata1 = "";
            var grosswt = "";
            var StoneWeight = "";
            var NetWt = "";
            var makingcharg = "";
            var Rate = "";
            var Total = "";
            decimal totalgrwt = 0;
            decimal totalnetwt = 0;
            decimal totalstwt = 0;
            decimal totalsaleamount = 0;
            decimal netamount = 0;
            decimal cgstamount = 0;
            // decimal totalstwt;
            //decimal totalnetwt;
            decimal totaltax = 0;
            decimal sgstamount = 0;
            decimal Making_Percentage;
            decimal Making_per_gram;
            decimal Making_Fixed_Amt;
            //  decimal totalgrwt;
            decimal grossTotalRate;
            decimal makingchrg;
            decimal MRP;
            decimal total;
            var cgst = 1.5;
            var sgst = 1.5;
            var data = "";
            foreach (var item in orderitem)
            {
                if (item.MRP != null && item.MRP != "")
                {
                    var imagePath = productdata.Images.Split(',');
                    data = "https://jewellerywebapplications.blob.core.windows.net/images/" + imagePath[0];
                    var productsdata = _productrepository.All().Where(x => x.Id == item.ProductId).FirstOrDefault();
                    itemcode += "<p>" + item.HSNCode + "</p>";
                    productListHtml += "<p>" + item.CategoryName + " " + item.ProductName + "</p>";
                    Quantity += "<p>" + item.Quantity + "</p>";
                    puritydata1 += "<p>" + item.Purity + "</p>";
                    grosswt += "<p>" + item.grosswt + "</p>";
                    StoneWeight += "<p>" + item.StoneWeight + "</p>";
                    NetWt += "<p>" + item.NetWt + "</p>";
                    makingcharg += "<p>" + item.makingchrg + "</p>";
                    Rate += "<p>" + item.Rate + "</p>";
                    Total += "<p>" + Convert.ToDecimal(item.MRP) * Convert.ToDecimal(item.Quantity) + "</p>";
                    totalnetwt = totalnetwt + Convert.ToDecimal(item.NetWt);
                    if (item.StoneWeight != null && item.StoneWeight != "")
                    {
                        totalstwt = totalstwt + Convert.ToDecimal(item.StoneWeight);
                    }

                    else
                    {
                        totalstwt = totalstwt + 0;
                    }
                    totalgrwt = totalgrwt + Convert.ToDecimal(item.grosswt);
                    totalsaleamount = totalsaleamount + (Convert.ToDecimal(item.MRP) * Convert.ToDecimal(item.Quantity));
                    cgstamount = (Convert.ToDecimal(cgst) * totalsaleamount) / 100;
                    sgstamount = (Convert.ToDecimal(sgst) * totalsaleamount) / 100;
                    totaltax = cgstamount + sgstamount;
                    netamount = totalsaleamount + totaltax;
                }
                else
                {
                    var imagePath = productdata.Images.Split(',');
                    data = "https://jewellerywebapplications.blob.core.windows.net/images/" + imagePath[0];
                    var productsdata = _productrepository.All().Where(x => x.Id == item.ProductId).FirstOrDefault();
                    var purities = _purityRepository.All().Where(x => x.Id == productsdata.PurityId).FirstOrDefault();
                    var netGoldRate = (Convert.ToDecimal(productsdata.NetWt) * Convert.ToDecimal(item.Rate)) / 10;
                    var makingCharges1 = Convert.ToDecimal(item.NetWt) * Convert.ToDecimal(productsdata.Making_per_gram);
                    var makingCharges2 = (netGoldRate * Convert.ToDecimal(productsdata.Making_Percentage)) / 100;
                    var makingCharges3 = Convert.ToDecimal(productsdata.Making_Fixed_Amt);
                    var makingCharges4 = (Convert.ToDecimal(purities.TodaysRate) * Convert.ToDecimal(productsdata.Making_Fixed_Wastage)) / 10;
                    makingchrg = makingCharges1 + makingCharges2 + makingCharges3 + makingCharges4;

                    Total = (netGoldRate + makingCharges1 + makingCharges2 + makingCharges3 + makingCharges4 + Convert.ToDecimal(productsdata.StoneAmount)).ToString();

                    totalsaleamount = totalsaleamount + Convert.ToDecimal(Total);
                    cgstamount = (Convert.ToDecimal(cgst) * totalsaleamount) / 100;
                    sgstamount = (Convert.ToDecimal(sgst) * totalsaleamount) / 100;
                    totaltax = cgstamount + sgstamount;
                    netamount = totalsaleamount + totaltax;

                }
            }
         
           

            if (orderdata.OrderStatus == "Paid")
            {
                var viewPath = System.IO.Path.Combine(_environment.WebRootPath + "/Templates", "OrderConfirmation.html");
                var template = System.IO.File.ReadAllText(viewPath);

                template = template.Replace("XXXCallUrlXXX", "<p style=\"color:#ffffff\">For queries contact us</p>");
                template = template.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template = template.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddState + " " + user.currAddPinCode);
                template = template.Replace("XXXXemailXXX", user.Email);
                template = template.Replace("XXXXpincodeXXX", user.currAddPinCode);
                template = template.Replace("XXXXquantityXXX", orderdata.Qty.ToString());
                template = template.Replace("XXXXsizeXXX", productdata.Size);
                template = template.Replace("XXXXimagesXXX", data);
                template = template.Replace("XXXXorderXXX", orderdata.InvoiceNo);
                template = template.Replace("XXXXorderdateXXX", orderdata.CreatedOn.ToString("dd/MM/yyyy"));
                template = template.Replace("XXXXmobileXXX", user.Mobile);
                template = template.Replace("XXXXproductnameXXX", productdata.Product_Name);
                template = template.Replace("XXXXitemcodeXXX", productdata.ItemCode);
                //  template = template.Replace("XXXXorderidXXX", orderdata.Id.ToString());
                template = template.Replace("XXXnetamtXXX", netamount.ToString());
                template = template.Replace("XXXStatusXXX", orderdata.OrderStatus);
                template = template.Replace("XXXpriceXXX", Total.ToString());
                template = template.Replace("XXXqtyXXX", orderdata.Qty.ToString());
                await _emailSender.SendEmailAsync(user.Email, "Order Confirmation", $"" + template + "");
            }
            else if (orderdata.OrderStatus == "Delivered")
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=jewellerywebapplications;AccountKey=ZaTHTvbGMADLIQLKM4ujTLLDwoCMDRGIIfeVBdjPN+taeuEmhqXV51R1IoCdFgOwdMRQRaZ7Axgr+AStBPFF2g==;EndpointSuffix=core.windows.net";
                string containerName = "template";
                string blobName = "Invoice.html";


                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                Response<BlobDownloadInfo> downloadResponse = await blobClient.DownloadAsync();
                var template1 = await new StreamReader(downloadResponse.Value.Content).ReadToEndAsync();
                var viewPath = System.IO.Path.Combine(_environment.WebRootPath + "/Templates", "OrderDelivered.html");
                //  var viewPath1 = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
                var template = System.IO.File.ReadAllText(viewPath);
                //var viewPath1 = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
                //  var viewPath1 = "https://jewellerywebapplications.blob.core.windows.net/template/Invoice.html";
                //   var template1 = System.IO.File.ReadAllText(viewPath1);
                //    var template1 = System.IO.File.ReadAllText(viewPath1);
                //  template = template.Replace("XXXABCXXX", Message);
                template = template.Replace("XXXCallUrlXXX", "<p style=\"color:#ffffff\">For queries contact us</p>");
                template = template.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template = template.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddPinCode);
                template = template.Replace("XXXXemailXXX", user.Email);
                template = template.Replace("XXXXpincodeXXX", user.currAddPinCode);
                template = template.Replace("XXXXquantityXXX", productdata.Quantity.ToString());
                template = template.Replace("XXXXsizeXXX", productdata.Size);
                template = template.Replace("XXXXimagesXXX", data);
                template = template.Replace("XXXXorderXXX", orderdata.InvoiceNo.ToString());
                template = template.Replace("XXXXorderdateXXX", orderdata.CreatedOn.ToString("dd/MM/yyyy"));
                template = template.Replace("XXXXmobileXXX", user.Mobile);
                template = template.Replace("XXXXproductnameXXX", productdata.Product_Name);
                template = template.Replace("XXXXitemcodeXXX", productdata.ItemCode);
                template = template.Replace("XXXXorderidXXX", orderdata.Id.ToString());
                template = template.Replace("XXXStatusXXX", orderdata.OrderStatus);
                template = template.Replace("XXXordervalueXXX", orderdata.ReceivedAmt.ToString());
                template = template.Replace("XXXpriceXXX", Total.ToString());
                template = template.Replace("XXXqtyXXX", orderdata.Qty.ToString());
                template = template.Replace("XXXnetamtXXX", netamount.ToString());
                template1 = template1.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template1 = template1.Replace("XXXXproductnameXXX", productListHtml);
                template1 = template1.Replace("XXXXhsncodeXXX", itemcode);
                template1 = template1.Replace("XXXXqtyXXX", Quantity.ToString());
                template1 = template1.Replace("XXXXpurityXXX", puritydata1.ToString());
                template1 = template1.Replace("XXXXgrwtXXX", grosswt.ToString());
                template1 = template1.Replace("XXXXstonewtXXX", StoneWeight.ToString());
                template1 = template1.Replace("XXXXnetwtXXX", NetWt.ToString());
                template1 = template1.Replace("XXXXmkpercXXX", makingcharg.ToString());
                //     template1 = template1.Replace("XXXXsizeXXX", productsdata.Size);
                template1 = template1.Replace("XXXXrateXXX", Rate);

                template1 = template1.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template1 = template1.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddState);
                template1 = template1.Replace("XXXXemailXXX", user.Email);
                template1 = template1.Replace("XXXXpincodeXXX", user.currAddPinCode);
                //   template1 = template1.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template1 = template1.Replace("XXXXorderXXX", orderdata.InvoiceNo);
                template1 = template1.Replace("XXXXorderdateXXX", user.CreatedOn.ToString("dd/MM/yyyy"));
                template1 = template1.Replace("XXXXmobileXXX", user.Mobile);

                template1 = template1.Replace("XXXXtotalamountXXX", totalsaleamount.ToString());
                //template1 = template1.Replace("XXXXvalueXXX", user.OrderValue);
                //template1 = template1.Replace("XXXStatusXXX", user.OrderStatus);
                template1 = template1.Replace("XXXnetamtXXX", netamount.ToString());
                template1 = template1.Replace("XXXcgstXXX", cgstamount.ToString());
                template1 = template1.Replace("XXXsgstXXX", sgstamount.ToString());
                template1 = template1.Replace("XXXtotaltaxXXX", totaltax.ToString());
                template1 = template1.Replace("XXXXpincodeXXX", user.currAddPinCode);
                template1 = template1.Replace("XXXXtotalXXX", Total.ToString());
                //        template1 = template1.Replace("XXXXinvoiceXXX", orderdata.Id.ToString());
                template1 = template1.Replace("XXXXtotalnetwtXXX", totalnetwt.ToString());
                template1 = template1.Replace("XXXXtotalgrwtXXX", totalgrwt.ToString());
                template1 = template1.Replace("XXXXtotalstwtXXX", totalstwt.ToString());
                template1 = template1.Replace("XXXXpaymentmodeXXX", orderdata.PaymentMode.ToString());
                //template1 = template1.Replace("XXXXinvoiceXXX", orderdata.Id.ToString());
                //try
                //{
                byte[] pdfBytes;
                var htmlContent = template1;
                using (MemoryStream stream = new MemoryStream())
                {
                    if (string.IsNullOrEmpty(htmlContent))
                    {
                        throw new Exception("HTML content is null or empty.");
                    }

                    ConverterProperties converterProperties = new ConverterProperties();
                    HtmlConverter.ConvertToPdf(htmlContent, stream, converterProperties);

                    pdfBytes = stream.ToArray();
                }
                var sender = new MailAddress("info@mkgharejewellers.com", "Ghare Jewellers");
                var subject = "Order Delivered";
                var client = new SmtpClient(_emailSettings.MailServer)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password),
                    Port = _emailSettings.MailPort,
                    EnableSsl = _emailSettings.SSL
                };
                var mailMessage = new MailMessage
                {
                    From = sender,
                };

                var attachment = new Attachment(new MemoryStream(pdfBytes), "Invoice.pdf", "application/pdf");
                mailMessage.Attachments.Add(attachment);
                mailMessage.To.Add(user.Email);
                mailMessage.Subject = subject;
                mailMessage.Body = template;
                mailMessage.IsBodyHtml = true;

                await client.SendMailAsync(mailMessage);
                // Email sent successfully
                //}
                //catch (Exception ex)
                //{

                //    return ex.Message;
                //}

            }
            else if (orderdata.OrderStatus == "Payment Failed")
            {
                var viewPath = System.IO.Path.Combine(_environment.WebRootPath + "/Templates", "PaymentFailed.html");
                //   var viewPath1 = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
                var template = System.IO.File.ReadAllText(viewPath);


                //    var template1 = System.IO.File.ReadAllText(viewPath1);
                //  template = template.Replace("XXXABCXXX", Message);

                template = template.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template = template.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddState + " " + user.currAddPinCode);
                template = template.Replace("XXXXemailXXX", user.Email);
                template = template.Replace("XXXXpincodeXXX", user.currAddPinCode);
                template = template.Replace("XXXXqtyXXX", orderdata.Qty.ToString());
                template = template.Replace("XXXXsizeXXX", productdata.Size);
                template = template.Replace("XXXXimagesXXX", data);
                template = template.Replace("XXXXorderXXX", orderdata.InvoiceNo.ToString());
                template = template.Replace("XXXXorderdateXXX", user.CreatedOn.ToString("dd/MM/yyyy"));
                template = template.Replace("XXXXmobileXXX", user.Mobile);
                template = template.Replace("XXXXproductnameXXX", productdata.Product_Name);
                template = template.Replace("XXXXitemcodeXXX", productdata.ItemCode);
                template = template.Replace("XXXXorderidXXX", orderdata.Id.ToString());
                template = template.Replace("XXXStatusXXX", orderdata.OrderStatus);
                template = template.Replace("XXXordervalueXXX", orderdata.ReceivedAmt.ToString());
                template = template.Replace("XXXpriceXXX", Total.ToString());

                await _emailSender.SendEmailAsync(user.Email, "Payment Failure - Order #" + orderdata.Id + "", $"" + template + "");
            }
            else if (orderdata.OrderStatus == "Shipped")
            {
                var viewPath = System.IO.Path.Combine(_environment.WebRootPath + "/Templates", "orderOnTheWay.html");
                //   var viewPath1 = Path.Combine(_environment.WebRootPath + "/Templates", "Invoice.html");
                var template = System.IO.File.ReadAllText(viewPath);


                //    var template1 = System.IO.File.ReadAllText(viewPath1);
                //  template = template.Replace("XXXABCXXX", Message);

                template = template.Replace("XXXXNameXXX", user.FirstName + " " + user.LastName);
                template = template.Replace("XXXXcuraddressXXX", user.currAddStreet + " " + user.currAddTown + " " + user.currAddState);
                template = template.Replace("XXXXemailXXX", user.Email);
                template = template.Replace("XXXXpincodeXXX", user.currAddPinCode);
                template = template.Replace("XXXXquantityXXX", orderdata.Qty.ToString());
                template = template.Replace("XXXXsizeXXX", productdata.Size);
                template = template.Replace("XXXXimagesXXX", data);
                template = template.Replace("XXXXorderXXX", orderdata.InvoiceNo.ToString());
                template = template.Replace("XXXXorderdateXXX", orderdata.CreatedOn.ToString("dd/MM/yyyy"));
                template = template.Replace("XXXXmobileXXX", user.Mobile);
                template = template.Replace("XXXXproductnameXXX", productdata.Product_Name);
                template = template.Replace("XXXXitemcodeXXX", productdata.ItemCode);
                template = template.Replace("XXXXorderidXXX", orderdata.Id.ToString());
                template = template.Replace("XXXStatusXXX", orderdata.OrderStatus);
                template = template.Replace("XXXordervalueXXX", orderdata.ReceivedAmt.ToString());
                template = template.Replace("XXXXtotalamountXXX", totalsaleamount.ToString());
                template = template.Replace("XXXXqtyXXX", orderdata.Qty.ToString());
                template = template.Replace("XXXpriceXXX", Total.ToString());
                template = template.Replace("XXXnetamtXXX", netamount.ToString());
                await _emailSender.SendEmailAsync(user.Email, "Your Order Has Been Shipped!", $"" + template + "");
            }
        }



    }

}


