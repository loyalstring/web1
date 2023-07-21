
using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
using JewelleryWebApplication.Repository;
namespace  JewelleryWebApplication.Repository
{
    public class ProductRepository : BaseRepository<tblProduct>, IProductRepository
    {
        public ProductRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }


    }
}
