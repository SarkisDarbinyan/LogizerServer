// ApplicationDbContext.cs
using LogizerServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LogizerServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Level> Levels { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<LevelRating> LevelRatings { get; set; }
        public DbSet<LevelLike> LevelLikes { get; set; }

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

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // UserFavorite configuration
            modelBuilder.Entity<UserFavorite>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.LevelId }).IsUnique();

                entity.HasOne(uf => uf.User)
                      .WithMany(u => u.FavoriteLevels)
                      .HasForeignKey(uf => uf.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(uf => uf.Level)
                      .WithMany()
                      .HasForeignKey(uf => uf.LevelId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // LevelRating configuration
            modelBuilder.Entity<LevelRating>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.LevelId }).IsUnique();

                entity.HasOne(lr => lr.User)
                      .WithMany(u => u.LevelRatings)
                      .HasForeignKey(lr => lr.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(lr => lr.Level)
                      .WithMany(l => l.Ratings)
                      .HasForeignKey(lr => lr.LevelId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // LevelLike configuration
            modelBuilder.Entity<LevelLike>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.LevelId }).IsUnique();

                entity.HasOne(ll => ll.User)
                      .WithMany(u => u.LevelLikes)
                      .HasForeignKey(ll => ll.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ll => ll.Level)
                      .WithMany(l => l.Likes)
                      .HasForeignKey(ll => ll.LevelId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}