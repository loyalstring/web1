
using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
namespace JewelleryWebApplication.Repository
{
    public class PurityRepository : BaseRepository<tblPurity>, IPurityRepository
    {
        public PurityRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }


    }
}
