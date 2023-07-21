using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;

namespace JewelleryWebApplication.Repository
{
    public class DeviceRepository:BaseRepository<Device>,IDeviceRepository
    {
        public  DeviceRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }


    }
}
