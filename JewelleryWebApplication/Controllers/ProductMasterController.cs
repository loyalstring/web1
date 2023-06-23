using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.IO;
using JewelleryWebApplication.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics.Eventing.Reader;
using iText.Commons.Actions.Data;
using Azure.Storage.Blobs;
using JewelleryWebApplication.Options;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs.Models;
using System.Text.RegularExpressions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Abp.Extensions;

namespace JewelleryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductMasterController : ControllerBase
    {
        private readonly ICollectionRepository _collectionRepository;
        public readonly IPartyMasterRepository _partyMasterRepository;
        public static IWebHostEnvironment _environment;
        private readonly IPurityRepository _purityRepository;
        private readonly IMaterialCategoryRepository _materialCategoryRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IProductRepository _productrepository;
        private readonly IBoxMasterRepository _boxmasterRepository;
        private readonly AzureOptions _azureOptions;
        public ProductMasterController(ICollectionRepository collectionRepository, IPartyMasterRepository partyMasterRepository, IBoxMasterRepository boxMasterRepository, IOptions<AzureOptions> azureoptions, IWebHostEnvironment webHostEnvironment, IPurityRepository purityRepository, IProductRepository productRepository, IMaterialCategoryRepository materialCategoryRepository, IProductTypeRepository productTypeRepository, IStaffRepository staffRepository)
        {
            _collectionRepository = collectionRepository;
            _partyMasterRepository = partyMasterRepository;
            _boxmasterRepository = boxMasterRepository;
            _azureOptions = azureoptions.Value;
            _environment = webHostEnvironment;
            _purityRepository = purityRepository;
            _materialCategoryRepository = materialCategoryRepository;
            _productTypeRepository = productTypeRepository;
            _staffRepository = staffRepository;
            _productrepository = productRepository;
        }
        [HttpGet("GetfetchAllProduct")]
        public async Task<IActionResult> GetfetchAllProduct()
        {
            var Productdata = _productrepository.All().Include(x => x.tblMaterialCategory).Include(x => x.Party_Details).Include(x => x.tblBox).Include(x=>x.tblPurity).ToList();
            if (Productdata != null)
            {
                return Ok(new { data = Productdata });
            }
            return Ok(new { data = "No Data" });
        }
        [HttpPost("fetchProductById")]
        public async Task<IActionResult> fetchProductById(tblProduct model)
        {
            var Productdata = _productrepository.All().Include(x=>x.tblPurity).Where(x => x.Id == model.Id).FirstOrDefault();
            if (Productdata != null)
            {
                return Ok(new { data = Productdata });
            }
            return Ok(new { data = "No Data" });
        }
        [HttpGet("fetchAllCategory")]
        public async Task<ActionResult> fetchAllCategory()
        {
            var categorydata = _materialCategoryRepository.All().ToList();
            if (categorydata != null)
            {
                return Ok(new { status = "Success", data = categorydata });
            }
            return Ok(new { data = "No Data" });
        }
        [HttpGet("fetchAllProductType")]
        public async Task<ActionResult> fetchAllProductType()
        {
            var producttypedata = _productTypeRepository.All().ToList();
            if (producttypedata != null)
            {
                return Ok(new { status = "Success", data = producttypedata });
            }
            return Ok(new { data = "No Data" });
        }
        [HttpGet("fetchAllPurity")]
        public async Task<ActionResult> fetchAllPurity()
        {
            var producttypedata = _purityRepository.All().ToList();
            if (producttypedata != null)
            {
                return Ok(new { status = "Success", data = producttypedata });
            }
            return Ok(new { data = "No Data" });
        }
        [HttpPost("InsertPurity")]
        public async Task<IActionResult> InsertPurity(tblPurity model)
        {
            
            if (ModelState.IsValid)
            {
                tblPurity purity = new tblPurity();
                purity.Category = model.Category;
              
                purity.Purity = model.Purity;
                purity.CreatedOn = DateTime.UtcNow;
              
              
                purity.Label = model.Label;
                purity.TodaysRate= model.TodaysRate;
                purity.OnlineStatus ="Active";
                await _purityRepository.InsertAsync(purity);

                return Ok(new { Status = "Success", data = purity });
            }
            return BadRequest();
        }
        [HttpPost("UpdatePurity")]
        public async Task<IActionResult> UpdatePurity(tblPurity model)
        {
            var puritydata = _purityRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (puritydata != null)

            {
                puritydata.Id = model.Id;
               
                    puritydata.Category = model.Category;
             
                  
                    puritydata.Purity = model.Purity;
               
                   
                    puritydata.Label = model.Label;
              
                    puritydata.TodaysRate= model.TodaysRate;
                puritydata.OnlineStatus = "Active";

                await _purityRepository.UpdateAsync(puritydata, puritydata.Id);

                return Ok(new { Status = "Success", data = puritydata });
            }
            return BadRequest();
        }
        [HttpPost("InsertProductType")]
        public async Task<IActionResult> InsertProductType(tblProductType model)
        {

            if (ModelState.IsValid)
            {
                tblProductType productType = new tblProductType();
                productType.Category_id = model.Category_id;
                productType.ProductTitle = model.ProductTitle;
                var isexist = _productTypeRepository.All().Where(x => x.Label == model.Label).FirstOrDefault();
                if (isexist != null)
                {
                    return Ok(new { Status = "Success", messege = "Product label already exist" });
                }
                productType.Label = model.Label;
                productType.Description = model.Description;
                productType.Slug = model.Slug;
                productType.HSNCode = model.HSNCode;
                productType.OnlineStatus = "Active";
                await _productTypeRepository.InsertAsync(productType);

                return Ok(new { Status = "Success", data = productType });
            }
            return BadRequest();
        }
        [HttpPost("UpdateProductType")]
        public async Task<IActionResult> UpdateProductType(tblProductType model)
        {
            var producttypedata = _productTypeRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (producttypedata != null)
            {
                producttypedata.Id = model.Id;
                    producttypedata.Category_id = model.Category_id;
              
                    producttypedata.ProductTitle = model.ProductTitle;
             
                    producttypedata.Label = model.Label;
               
                    producttypedata.Description = model.Description;
               
                    producttypedata.Slug = model.Slug;
               
                    producttypedata.HSNCode = model.HSNCode;
                 producttypedata.OnlineStatus = "Active";
                await _productTypeRepository.UpdateAsync(producttypedata, producttypedata.Id);
                return Ok(new { Status = "Success", data = producttypedata });
            }

            return BadRequest();
        }
        [HttpPost("InsertCategory")]
        public async Task<IActionResult> InsertCategory(CategoryViewModel model)
        {

            if (ModelState.IsValid)
            {
                tblMaterialCategory category = new tblMaterialCategory();
                category.Name = model.Name;
                category.ParentsCategory = model.ParentsCategory;
                category.Material = model.Material;
                category.ShortCode = model.ShortCode;
                category.Description = model.Description;
                category.HSNCode = model.HSNCode;
                category.Slug = model.Slug;
                category.Label = model.Label;
                category.Entryby_Staff_id = model.Entryby_Staff_id;

                category.OnlineStatus = "Active";
                await _materialCategoryRepository.InsertAsync(category);
                return Ok(new { Status = "Success", data = category });
            }
            return BadRequest();
        }
        [HttpPost("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(tblMaterialCategory model)
        {
            var categorydata = _materialCategoryRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (categorydata != null)
            {
                    categorydata.Id = model.Id;
                    categorydata.Name = model.Name;
              
                    categorydata.ParentsCategory = model.ParentsCategory;
               
                    categorydata.Material = model.Material;
               
                    categorydata.ShortCode = model.ShortCode;
               
                    categorydata.Description = model.Description;
              
                    categorydata.HSNCode = model.HSNCode;
              
                    categorydata.Slug = model.Slug;
             
                    categorydata.Label = model.Label;
                categorydata.OnlineStatus = "Active";

                await _materialCategoryRepository.UpdateAsync(categorydata, categorydata.Id);
                return Ok(new { Status = "Success", data = categorydata });
            }
            return BadRequest();
        }
        //public class FIleUploadAPI
        //{
        //    public IFormFile files { get; set; }
        //}

        [HttpPost("InsertProduct")]
        public async Task<IActionResult> InsertProduct()
        {
            int CollectionId = Convert.ToInt32(HttpContext.Request.Form["CollectionId"]);
            string productlabel = _collectionRepository.All().Where(x => x.Id == CollectionId).Select(X => X.Label).FirstOrDefault();

            int count = _productrepository.All().Where(x => x.ItemCode.Contains(productlabel)).Count();
            string data = "";

            if (ModelState.IsValid)
            {
                List<tblProduct> li = new List<tblProduct>();
                tblProduct products = new tblProduct();


                if (HttpContext.Request.Form.Files.Count <= 5)
                {
                    //for (int i = 0; i < HttpContext.Request.Form.Files.Count; i++)
                    //{
                    //    var file = HttpContext.Request.Form.Files[i];
                    //    if (file != null && file.Length > 0)
                    //    {
                    //        var filename = Guid.NewGuid().ToString() + "_" + file.FileName;
                    //        var imgpath = Path.Combine(_environment.WebRootPath, "images", filename);

                    //        //
                    //        //  file.copyto(filename);
                    //        using (FileStream fs = System.IO.File.Create(imgpath))
                    //        {
                    //            // compressedzip.compressimage(fs, imgpath, filename);
                    //            file.CopyTo(fs);


                    //        }
                    //        data = data + imgpath + ",";
                    //    };
                    //}

                    for (int i = 0; i < HttpContext.Request.Form.Files.Count; i++)
                    {
                        var file = HttpContext.Request.Form.Files[i];
                        if (file != null && file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                            //  var imgpath = Path.Combine(_environment.WebRootPath, "productimages", fileName);
                            // var imgpath = "https://jewellerywebapplications.blob.core.windows.net/images/" + fileName;
                            var imgpath = Path.GetExtension(file.FileName);
                            using MemoryStream fileUploadStream = new MemoryStream();
                            file.CopyTo(fileUploadStream);
                            fileUploadStream.Position = 0;
                            BlobContainerClient blobContainerClient = new BlobContainerClient(
                                _azureOptions.ConnectionString, _azureOptions.Container);
                            var uniquename = Guid.NewGuid().ToString() + imgpath;
                            BlobClient blobClient = blobContainerClient.GetBlobClient(uniquename);
                            blobClient.Upload(fileUploadStream, new BlobUploadOptions()
                            {
                                HttpHeaders = new BlobHttpHeaders
                                {
                                    ContentType = "image/bitmap"
                                }
                            }, cancellationToken: default
                             );
                            //  file.CopyTo(fileName);
                            using (FileStream fs = System.IO.File.Create(uniquename))
                            {
                                // Compressedzip.Compressimage(fs, imgpath, fileName);
                                file.CopyTo(fs);


                            }
                            data = data + uniquename + ",";
                        };
                    }
                    data = data.TrimEnd(',');
                    products.Quantity = Convert.ToInt32(HttpContext.Request.Form["Quantity"]);

                    int x = count + 1;

                    for (int i = 1; i <= products.Quantity; i++)
                    {
                        string lable = productlabel + x;
                        x++;
                        tblProduct product = new tblProduct();
                        product.Images = data;
                        product.Entryby_Staff_id = Convert.ToInt32(HttpContext.Request.Form["Entryby_Staff_id"]);
                        product.Product_No = HttpContext.Request.Form["Product_No"];
                        product.Product_Name = HttpContext.Request.Form["Product_Name"];
                        product.Category_id = Convert.ToInt32(HttpContext.Request.Form["Category_id"]);
                        product.Category_Name = HttpContext.Request.Form["Category_Name"];
                        product.Pieces = Convert.ToInt32(HttpContext.Request.Form["Pieces"]);
                        product.HUIDCode = HttpContext.Request.Form["HUIDCode"];
                        product.NetWt = Convert.ToDecimal(HttpContext.Request.Form["NetWt"]);
                        product.product_type = HttpContext.Request.Form["product_type"];
                        product.Size = HttpContext.Request.Form["Size"];
                        product.grosswt = Convert.ToDecimal(HttpContext.Request.Form["grosswt"]);
                        product.purity = HttpContext.Request.Form["purity"];
                        product.collection = HttpContext.Request.Form["collection"];
                        product.occasion = HttpContext.Request.Form["occasion"];
                        product.gender = HttpContext.Request.Form["gender"];
                        product.description = HttpContext.Request.Form["description"];
                        product.PurityId = Convert.ToInt32(HttpContext.Request.Form["PurityId"]);
                        product.ProductTypeId = Convert.ToInt32(HttpContext.Request.Form["ProductTypeId"]);
                        product.PartyTypeId = Convert.ToInt32(HttpContext.Request.Form["PartyTypeId"]);
                        product.BoxId = Convert.ToInt32(HttpContext.Request.Form["BoxId"]);
                        product.Making_Fixed_Amt = HttpContext.Request.Form["Making_Fixed_Amt"];
                        product.Making_Fixed_Wastage = HttpContext.Request.Form["Making_Fixed_Wastage"];
                        product.Making_Percentage = HttpContext.Request.Form["Making_Percentage"];
                        product.Making_per_gram = HttpContext.Request.Form["Making_per_gram"];
                        product.StoneWeight = HttpContext.Request.Form["StoneWeight"];
                        product.StoneAmount = HttpContext.Request.Form["StoneAmount"];
                        product.Featured = HttpContext.Request.Form["Featured"];
                        product.Product_Code = HttpContext.Request.Form["Product_Code"];
                        product.MRP = Convert.ToInt32(HttpContext.Request.Form["MRP"]);
                        product.ItemCode = lable;
                        product.ItemType = HttpContext.Request.Form["ItemType"];
                        product.ImageList1 = HttpContext.Request.Form["ImageList1"];
                        product.ImageList2 = HttpContext.Request.Form["ImageList2"];
                        product.ImageList3 = HttpContext.Request.Form["ImageList3"];
                        product.ImageList4 = HttpContext.Request.Form["ImageList4"];
                        product.ImageList5 = HttpContext.Request.Form["ImageList5"];
                        product.CollectionId= Convert.ToInt32(HttpContext.Request.Form["CollectionId"]);
                        product.OnlineStatus = "Active";

                        li.Add(product);
                    }

                    await _productrepository.BulkInsertAsync(li);
                    return Ok(new { status = "Success", data = li });
                }
                else
                {
                    return Ok(new { message = "select maximum 5 Files" });
                }
            }
            return BadRequest();
        }
        public async Task<string> FetchImageFromAzureStorage(string connectionString, string containerName, string imageName)
        {
            // Create a BlobServiceClient using the connection string
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Get a reference to the container
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Get a reference to the blob
            BlobClient blobClient = containerClient.GetBlobClient(imageName);

            // Download the image to a local file
            string localFilePath = Path.GetTempFileName();
            await blobClient.DownloadToAsync(localFilePath);

            // Return the local file path
            return localFilePath;
        }


        [HttpPut("UpdateImage/{Id}")]
        public async Task<IActionResult> UpdateImage(int Id)
        {
            var productdata = _productrepository.All().Where(x => x.Id == Id).FirstOrDefault();
            string data = "";
            if (ModelState.IsValid)
            {


                if (HttpContext.Request.Form.Files.Count <= 5)
                {
                    for (int i = 0; i < HttpContext.Request.Form.Files.Count; i++)
                    {
                        var file = HttpContext.Request.Form.Files[i];
                        if (file != null && file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                            //  var imgpath = Path.Combine(_environment.WebRootPath, "productimages", fileName);
                            // var imgpath = "https://jewellerywebapplications.blob.core.windows.net/images/" + fileName;
                            var imgpath = Path.GetExtension(file.FileName);
                            using MemoryStream fileUploadStream = new MemoryStream();
                            file.CopyTo(fileUploadStream);
                            fileUploadStream.Position = 0;
                            BlobContainerClient blobContainerClient = new BlobContainerClient(
                                _azureOptions.ConnectionString, _azureOptions.Container);
                            var uniquename = Guid.NewGuid().ToString() + imgpath;
                            BlobClient blobClient = blobContainerClient.GetBlobClient(uniquename);
                            blobClient.Upload(fileUploadStream, new BlobUploadOptions()
                            {
                                HttpHeaders = new BlobHttpHeaders
                                {
                                    ContentType = "image/bitmap"
                                }
                            }, cancellationToken: default
                             );
                            //  file.CopyTo(fileName);
                            using (FileStream fs = System.IO.File.Create(uniquename))
                            {
                                // Compressedzip.Compressimage(fs, imgpath, fileName);
                                file.CopyTo(fs);


                            }
                            data = data + uniquename + ",";
                        };
                    }

                    data = data.TrimEnd(',');
                    if (data != null)
                    {
                        productdata.Images = data;
                        await _productrepository.UpdateAsync(productdata, productdata.Id);
                        return Ok(new { status = "Success", data = productdata });
                    }

                }
                else
                {
                    return Ok(new { message = "select maximum 5 Files" });
                }
            }
            return BadRequest();
        }

        [HttpPost("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(tblProduct model)
        {
            var data = _productrepository.All().ToList();
            var product = _productrepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (product != null)
            {
                if (model.Entryby_Staff_id != 0)
                {
                    product.Entryby_Staff_id = model.Entryby_Staff_id;
                }
                else if (model.Product_No != null)
                {
                    product.Product_No = model.Product_No;
                }
                else if (model.Product_Name != null)
                {
                    product.Product_Name = model.Product_Name;

                }
                else if (model.Category_id != 0)
                {
                    product.Category_id = model.Category_id;
                }
                else if (model.Category_Name != null)
                {
                    product.Category_Name = model.Category_Name;
                }
                else if (model.Pieces != 0)
                {
                    product.Pieces = model.Pieces;
                }
                else if (model.HUIDCode != null)
                {
                    product.HUIDCode = model.HUIDCode;
                }
                else if (model.NetWt != 0)
                {
                    product.NetWt = model.NetWt;
                }
                else if (model.Size != null)
                {
                    product.Size = model.Size;
                }
                else if (model.grosswt != 0)
                {
                    product.grosswt = model.grosswt;
                }
                else if (model.purity != null)
                {
                    product.purity = model.purity;
                }
                else if (model.collection != null)
                {
                    product.collection = model.collection;
                }
                else if (model.occasion != null)
                {
                    product.occasion = model.occasion;
                }
                else if (model.gender != null)
                {
                    product.gender = model.gender;
                }
                else if (model.description != null)
                {
                    product.description = model.description;
                }
                else if (model.ProductTypeId != 0)
                {
                    product.ProductTypeId = model.ProductTypeId;
                }
                else if (model.PartyTypeId != 0)
                {
                    product.PartyTypeId = model.PartyTypeId;
                }
                else if (model.BoxId != 0)
                {
                    product.BoxId = model.BoxId;
                }
                else if (model.Making_per_gram != null)
                {
                    product.Making_per_gram = model.Making_per_gram;
                }
                else if (model.Making_Fixed_Wastage != null)
                {
                    product.Making_Fixed_Wastage = model.Making_Fixed_Wastage;
                }
                else if (model.Making_Percentage != null)
                {
                    product.Making_Percentage = model.Making_Percentage;
                }
                else if (model.Making_Fixed_Amt != null)
                {
                    product.Making_Fixed_Amt = model.Making_Fixed_Amt;
                }
                else if (model.StoneAmount != null)
                {
                    product.StoneAmount = model.StoneAmount;
                }
                else if (model.StoneWeight != null)
                {
                    product.StoneWeight = model.StoneWeight;
                }
                else if (model.Featured != null)
                {
                    product.Featured = model.Featured;
                }
                else if (model.Product_Code != null)
                {
                    product.Product_Code = model.Product_Code;
                }
                else if (model.MRP==0 || model.MRP!=0)
                {
                    product.MRP = model.MRP;
                }
                else if (model.ItemType != null)
                {
                    product.ItemType = model.ItemType;
                }
                else if (model.ImageList1 != null)
                {
                    product.ImageList1 = model.ImageList1;
                }
                else if (model.ImageList2 != null)
                {
                    product.ImageList2 = model.ImageList2;
                }
                else if (model.ImageList3 != null)
                {
                    product.ImageList3 = model.ImageList3;
                }
                else if (model.ImageList4 != null)
                {
                    product.ImageList4 = model.ImageList4;
                }
                else if (model.ImageList5 != null)
                {
                    product.ImageList5 = model.ImageList5;
                }
                else if (model.ItemCode != null)
                {
                    product.ItemCode = model.ItemCode;
                }
                else if (model.OnlineStatus != null)
                {
                    product.OnlineStatus ="Active";
                }

                await _productrepository.UpdateAsync(product, product.Id);
                return Ok(new { status = "Success", data = product });
            }


            return BadRequest();

        }

        [HttpPost("BulkUpdateProduct")]
        public async Task<IActionResult> BulkUpdateProduct(List<tblProduct> list)
        {

            if (ModelState.IsValid)
            {

                foreach (var model in list)
                {
                    tblProduct product = new tblProduct();
                    var products = _productrepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
                    if (products != null)
                    {
                        product.Id = model.Id;
                        product.Entryby_Staff_id = model.Entryby_Staff_id;

                        product.Product_No = model.Product_No;

                        product.Product_Name = model.Product_Name;

                        product.Category_id = model.Category_id;

                        product.ProductTypeId = model.ProductTypeId;

                        product.CollectionId = model.CollectionId;

                        product.BoxId = model.BoxId;
                        product.PurityId = model.PurityId;
                        product.PartyTypeId = model.PartyTypeId;

                        product.Category_Name = model.Category_Name;

                        product.Pieces = model.Pieces;

                        product.HUIDCode = model.HUIDCode;

                        product.NetWt = model.NetWt;

                        product.Size = model.Size;

                        product.grosswt = model.grosswt;

                        product.purity = model.purity;

                        product.collection = model.collection;

                        product.occasion = model.occasion;

                        product.gender = model.gender;

                        product.description = model.description;

                        product.ProductTypeId = model.ProductTypeId;

                        product.PartyTypeId = model.PartyTypeId;

                        product.BoxId = model.BoxId;

                        product.Making_Fixed_Amt = model.Making_Fixed_Amt;

                        product.Making_Fixed_Wastage = model.Making_Fixed_Wastage;
                        product.Making_Percentage = model.Making_Percentage;
                        product.Making_per_gram = model.Making_per_gram;

                        product.StoneAmount = model.StoneAmount;

                        product.StoneWeight = model.StoneWeight;

                        product.Featured = model.Featured;

                        product.Product_Code = model.Product_Code;

                        product.MRP = model.MRP;

                        product.ItemType = model.ItemType;

                        product.ImageList1 = model.ImageList1;

                        product.ImageList2 = model.ImageList2;

                        product.ImageList3 = model.ImageList3;

                        product.ImageList4 = model.ImageList4;

                        product.ImageList5 = model.ImageList5;

                        product.ItemCode = model.ItemCode;

                        product.Images = model.Images;
                        product.OnlineStatus = "Active";

                        await _productrepository.UpdateAsync(product, product.Id);
                    }

                }
                //  await _productrepository.BulkUpdateAsync(list);
                return Ok(new { status = "Success", data = list });
            }
            return BadRequest();
        }

        [HttpDelete("deleteproductimage/{id}")]
        public async Task<IActionResult> deleteproductdetails(int id)
        {
            var productdata = _productrepository.All().Where(x => x.Id == id).FirstOrDefault();
            if (productdata != null)
            {
                if (productdata.Images != "")
                {
                    var result = productdata.Images.Split(',');
                    foreach (string str in result)
                    {
                        System.IO.File.Delete(str);
                    }
                }
                await _productrepository.DeleteAsync(productdata);
                return Ok(new { message = "deleted successfully" });
                //  file.delete(product.product_image);
            };

            return BadRequest();
        }

        [HttpGet("GetAllBoxMaster")]
        public async Task<IActionResult> GetAllBoxMaster()
        {
            var boxdata = await _boxmasterRepository.All().ToListAsync();
            if (boxdata != null)
            {
                return Ok(new { status = "Success", data = boxdata });
            }
            return Ok(new { status = "Success", message = "No Data" });
        }

        [HttpPost("AddBoxMaster")]
        public async Task<IActionResult> AddBoxMaster(tblBox model)
        {
            if (model != null)
            {
                tblBox box = new tblBox();
                box.MetalName = model.MetalName;
                box.BoxName = model.BoxName;
                box.OnlineStatus = "Active";
                box.EmptyWeight = model.EmptyWeight;
                box.ProductName = model.ProductName;
                await _boxmasterRepository.InsertAsync(box);
                return Ok(new { status = "Success", Data = box });
            }
            return BadRequest();

        }
        [HttpPost("UpdateBoxMaster")]
        public async Task<IActionResult> UpdateBoxMaster(tblBox model)
        {
            var boxdata = _boxmasterRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (boxdata != null)
            {
                boxdata.Id = model.Id;
                    boxdata.MetalName = model.MetalName;
                
                    boxdata.EmptyWeight = model.EmptyWeight;
              
                    boxdata.BoxName = model.BoxName;
               
                    boxdata.ProductName = model.ProductName;
                boxdata.OnlineStatus = "Active";
                
                await _boxmasterRepository.UpdateAsync(boxdata, boxdata.Id);
                return Ok(new { status = "Success", Data = boxdata });
            }
            return BadRequest();

        }
        [HttpGet("GetAllPartyMaster")]
        public async Task<IActionResult> GetAllPartyMaster()
        {
            var partydata = _partyMasterRepository.All().ToListAsync();
            if (partydata != null)
            {
                return Ok(new { status = "Success", data = partydata });
            }
            return Ok(new { status = "Success", message = "No Data" });
        }

        [HttpPost("AddPartyMaster")]
        public async Task<IActionResult> AddPartyMaster(Party_Details model)
        {
            if (model != null)
            {
                Party_Details party = new Party_Details();
                party.supplier_code = model.supplier_code;
                party.supplier_name = model.supplier_name;
                party.party_pan_no = model.party_pan_no;
                party.party_adhar_no = model.party_adhar_no;
                party.contact_no = model.contact_no;
                party.email_id = model.email_id;
                party.address = model.address;
                party.state = model.state;
                party.city = model.city;
                party.firm_name = model.firm_name;
                party.firm_details = model.firm_details;
                party.gst_no = model.gst_no;
                party.supplierType = model.supplierType;
                party.central_gst_no = model.central_gst_no;
                party.OnlineStatus = "Active";
                await _partyMasterRepository.InsertAsync(party);
                return Ok(new { status = "Success", Data = party });
            }
            return BadRequest();

        }

        [HttpPost("UpdatePartyMaster")]
        public async Task<IActionResult> UpdatePartyMaster(Party_Details model)
        {
            var party = _partyMasterRepository.All().Where(x => x.Id == model.Id).FirstOrDefault();
            if (party != null)
            {
                party.Id = model.Id;

                    party.central_gst_no = model.central_gst_no;
               
                    party.gst_no = model.gst_no;
               
                    party.firm_details = model.firm_details;
             
                    party.firm_name = model.firm_name;
              
                    party.city = model.city;
              
                    party.state = model.state;
                
                    party.address = model.address;
             
                    party.email_id = model.email_id;
              
                    party.contact_no = model.contact_no;
               
                    party.supplier_code = model.supplier_code;
              
                    party.party_pan_no = model.party_pan_no;

                    party.supplier_name = model.supplier_name;
              
                    party.party_adhar_no = model.party_adhar_no;
               
                    party.supplierType = model.supplierType;
                party.OnlineStatus = "Active";

                await _partyMasterRepository.UpdateAsync(party, party.Id);
                return Ok(new { status = "Success", Data = party });
            }
            return BadRequest();

        }


        [HttpPost("AddCollection")]
        public async Task<IActionResult> AddCollection(tblCollection model)
        {
            if (ModelState.IsValid)
            {
                tblCollection tblCollection = new tblCollection();
                tblCollection.Category_id = model.Category_id;
                tblCollection.ProductType = model.ProductType;
                tblCollection.Collection_Name = model.Collection_Name;
                tblCollection.Slug = model.Slug;
                tblCollection.Label = model.Label;
                tblCollection.OnlineStatus = "Active";
                await _collectionRepository.InsertAsync(tblCollection);
                return Ok(new { status = "Success", Data = tblCollection });
            }
            return BadRequest();

        }
        [HttpGet("GetAllCollection")]
        public async Task<IActionResult> GetAllCollection()
        {
            var collectiondata = _collectionRepository.All().ToList();
            if (collectiondata != null)
            {
                return Ok(new { status = "Success", Data = collectiondata });
            }
            return Ok(new { status = "Success", Data = "No Data" });

        }

        [HttpPost("UpdateCollection")]
        public async Task<IActionResult> UpdateCollection(tblCollection model)
        {
            var collectiondata = _collectionRepository.All().Where(x=>x.Id==model.Id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                collectiondata.Id = model.Id;
                    collectiondata.Category_id = model.Category_id;
              
                    collectiondata.ProductType = model.ProductType;
             
                    collectiondata.Collection_Name = model.Collection_Name;
                
                    collectiondata.Slug = model.Slug;
              
                    collectiondata.Label = model.Label;
                collectiondata.OnlineStatus = model.OnlineStatus;
                
                await _collectionRepository.UpdateAsync(collectiondata, collectiondata.Id);
                return Ok(new { status = "Success", Data = collectiondata });
            }
            return BadRequest();

        }
    }
}

