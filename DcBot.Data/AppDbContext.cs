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
        public DbSet<Afk> Afks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<RoleTypeRelation> RoleTypeRelations { get; set; }
    }
}