
using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
namespace JewelleryWebApplication.Repository
{
    public class ProductTypeRepository : BaseRepository<tblProductType>, IProductTypeRepository
    {
        public ProductTypeRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }


    }
}
