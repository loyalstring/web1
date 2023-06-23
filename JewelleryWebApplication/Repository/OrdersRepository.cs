
using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
using JewelleryWebApplication.Repository;
namespace JewelleryWebApplication.Repository
{
    public class OrdersRepository : BaseRepository<tblOrder>, IOrdersRepository
    {
        public OrdersRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }


    }
}
