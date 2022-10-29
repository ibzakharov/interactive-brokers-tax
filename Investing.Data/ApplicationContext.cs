using Investing.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Investing.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(
                @"Data Source=C:\Users\Work\MyProjects\interactive-brokers-tax\Investing.Data\tax_return.db");
        }
    }
}