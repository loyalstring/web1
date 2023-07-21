using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;

namespace JewelleryWebApplication.Repository
{
    public class RateRepository : BaseRepository<tblRates>, IRateRepository
    {
        public RateRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }


    }
  
}
