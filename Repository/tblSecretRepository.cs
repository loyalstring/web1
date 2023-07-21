using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;

namespace JewelleryWebApplication.Repository
{
    public class tblSecretRepository : BaseRepository<tblSecret>, ItblSecretRepository
    {
        public tblSecretRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }
    
    }
}
