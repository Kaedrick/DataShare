using DataShare.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DataShare.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<StoredFile>()
            .HasIndex(file => file.Token)
            .IsUnique();

        modelBuilder.Entity<StoredFile>()
            .HasOne(file => file.User)
            .WithMany(user => user.Files)
            .HasForeignKey(file => file.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
