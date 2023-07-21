
using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
using JewelleryWebApplication.Repository;
namespace JewelleryWebApplication.Repository
{
    public class CustomerDetailsRepository : BaseRepository<tblCustomerDetails>, ICustomerDetailsRepository
    {
        public CustomerDetailsRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }


    }
}
