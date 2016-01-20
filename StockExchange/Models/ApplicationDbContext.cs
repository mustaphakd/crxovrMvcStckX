using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace StockExchange.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<Stock> Stocks { get; set; }

       // public new virtual DbSet<ApplicationUser> Users { get; set; }

    }
}
