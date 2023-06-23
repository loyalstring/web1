using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;

namespace JewelleryWebApplication.Repository
{
    public class CollectionRepository:BaseRepository<tblCollection>,ICollectionRepository
    {
        public CollectionRepository(JewelleryWebApplicationContext context):base(context)
        {

        }
    }
}
