using Microsoft.EntityFrameworkCore;
using voca.Domain.Entities;

namespace voca.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Word> Words => Set<Word>();

    public DbSet<UserWord> UserWords => Set<UserWord>();

    public DbSet<WordSynonym> WordSynonyms => Set<WordSynonym>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserWord>()
            .HasKey(uw => new
            {
                uw.UserId,
                uw.WordId
            });

        modelBuilder.Entity<UserWord>()
            .HasOne(uw => uw.User)
            .WithMany(u => u.UserWords)
            .HasForeignKey(uw => uw.UserId);

        modelBuilder.Entity<UserWord>()
            .HasOne(uw => uw.Word)
            .WithMany(w => w.UserWords)
            .HasForeignKey(uw => uw.WordId);
        
        modelBuilder.Entity<WordSynonym>()
            .HasOne(ws => ws.Word)
            .WithMany(w => w.Synonyms)
            .HasForeignKey(ws => ws.WordId);
        
        modelBuilder.Entity<User>()
            .HasIndex(user => user.GoogleId)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();
            }
}