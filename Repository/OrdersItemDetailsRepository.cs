
using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
namespace JewelleryWebApplication.Repository
{
    public class OrdersItemDetailsRepository : BaseRepository<OrderItemDetails>, IOrdersItemDetailsRepository
    {
        public OrdersItemDetailsRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }


    }
}
