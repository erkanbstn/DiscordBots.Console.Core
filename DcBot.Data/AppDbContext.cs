using DcBot.Core.Core;
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