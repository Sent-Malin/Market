using Azure;
using Market.Data.Models;
using Market_Init;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Market.Data
{
    public class MarketDbContext : DbContext
    {
        public DbSet<GamesEntity> Games { get; set; }
        public DbSet<RoomsEntity> Rooms { get; set; }
        public DbSet<UsersEntity> Users { get; set; }
        public DbSet<OperationsEntity> Operations { get; set; }
        public MarketDbContext() : base()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=Yes;");
        }

        public void PlayersOffline()
        {
            Users.Where(u => u.IsOnline == true).ToList().ForEach(u => { u.IsOnline = false; });
            SaveChanges();
        }
    }
}