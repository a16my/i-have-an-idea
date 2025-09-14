using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BirFikrimVar.Models;

namespace BirFikrimVar.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
        }

        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Sharing relation
            modelBuilder.Entity<Post>()
                .HasOne(p => p.OriginalPost)
                .WithMany()
                .HasForeignKey(p => p.OriginalPostId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}