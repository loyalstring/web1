using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;

namespace JewelleryWebApplication.Repository
{
    public class tblProductDetailsRepository : BaseRepository<tblProductsDetails>,ItblProductDetailsRepository
    {
        public  tblProductDetailsRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }


    }
}
