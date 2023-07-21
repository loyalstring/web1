

using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Repository;

[assembly: HostingStartup(typeof(JewelleryWebApplication.Services.JewelleryHostingStartup))]
namespace JewelleryWebApplication.Services
{
    public class JewelleryHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddTransient<IOrdersRepository, OrdersRepository>();
                services.AddTransient<IProductRepository, ProductRepository>();
                services.AddTransient<IProductTypeRepository, ProductTypeRepository>();
                services.AddTransient<IPurityRepository, PurityRepository>();
                services.AddTransient<IStaffRepository, StaffRepository>();
                services.AddTransient<IMaterialCategoryRepository, MaterialCategoryRepository>();
                services.AddTransient<IRateRepository, RateRepository>();
                services.AddTransient<ICustomerDetailsRepository, CustomerDetailsRepository>();
                services.AddTransient<IDeviceRepository, DeviceRepository>();
                services.AddTransient<IBoxMasterRepository, BoxMasterRepository>();
                services.AddTransient<IPartyMasterRepository, PartyMasterRepository>();
                services.AddTransient<ICollectionRepository, CollectionRepository>();
                services.AddTransient<ItblProductDetailsRepository, tblProductDetailsRepository>();
                services.AddTransient<IOrdersItemDetailsRepository, OrdersItemDetailsRepository>();
                services.AddTransient<ItblSecretRepository, tblSecretRepository>();


            });
        }
    }
}
