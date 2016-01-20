using System;
using System.Data.Entity.Infrastructure;
namespace StockExchange.Models
{
    public interface IApplicationDbContext : IDisposable
    {
        System.Data.Entity.DbSet<Stock> Stocks { get; set; }
        //System.Data.Entity.DbSet<ApplicationUser> Users { get; set; }

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();
    }
}
