
using JewelleryWebApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JewelleryWebApplication.Data
{
        public class JewelleryWebApplicationContext : IdentityDbContext<ApplicationUser>
    {
    
            public JewelleryWebApplicationContext(DbContextOptions<JewelleryWebApplicationContext> options)
                : base(options)
            {
            }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<tblProduct> tblProducts { get; set; }
        public DbSet<tblMaterialCategory> materialCategories { get; set; }
        public DbSet<tblProductType> tblProductType { get; set; }
        public DbSet<tblPurity> tblPurities { get; set; }
        public DbSet<tblCustomerDetails> tblCustomerDetails { get; set; }
        public DbSet<tblRates> tblRates { get; set; }
        public DbSet<tblOrder> orders { get; set; }
        public DbSet<tblStaff> tblStaff { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<tblBox> tblBox { get; set; }
        public DbSet<Party_Details> Party_Details { get; set;}
        public DbSet<tblCollection> tblCollection { get; set; }
        public DbSet<tblProductsDetails> tblProductsDetails { get; set; }
        public DbSet<OrderItemDetails> OrderItemDetails { get; set; }
        public DbSet<tblSecret> tblSecret { get; set; }


    }
}
