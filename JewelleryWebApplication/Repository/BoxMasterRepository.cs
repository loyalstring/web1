using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;

namespace JewelleryWebApplication.Repository
{
    public class BoxMasterRepository: BaseRepository<tblBox>, IBoxMasterRepository
    {
        public BoxMasterRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }
    
    }
}
