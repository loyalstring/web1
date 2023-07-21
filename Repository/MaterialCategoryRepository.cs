
using JewelleryWebApplication.Base.Repository;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Interface;
using JewelleryWebApplication.Models;
namespace JewelleryWebApplication.Repository
{
    public class MaterialCategoryRepository : BaseRepository<tblMaterialCategory>, IMaterialCategoryRepository
    {
        public MaterialCategoryRepository(JewelleryWebApplicationContext context) : base(context)
        {

        }


    }
}
