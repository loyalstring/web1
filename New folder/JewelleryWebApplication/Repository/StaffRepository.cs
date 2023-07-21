

using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;


namespace JewelleryWebApplication.Repository
{
    public class StaffRepository : BaseRepository<tblStaff>, IStaffRepository
    {
        public StaffRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }

       
    }
}
