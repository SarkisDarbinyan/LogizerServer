using Microsoft.EntityFrameworkCore;
using LogizerServer.Models;

namespace LogizerServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Level> Levels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Level configuration
            modelBuilder.Entity<Level>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.LevelData).IsRequired();

                entity.HasIndex(e => e.Name).IsUnique();
            });
        }
    }
}