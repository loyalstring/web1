using JewelleryWebApplication.Base.Interface;
using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;

namespace JewelleryWebApplication.Repository
{
    public class PartyMasterRepository:BaseRepository<Party_Details>,IPartyMasterRepository
    {
         public PartyMasterRepository(JewelleryWebApplicationContext context):base(context)
        {

        }
        
    }
}
