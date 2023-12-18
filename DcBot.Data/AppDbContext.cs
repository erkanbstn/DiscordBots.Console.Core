using DcBot.Core.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DcBot.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<DcServer> DcServers { get; set; }
    }
}