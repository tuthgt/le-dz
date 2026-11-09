using Microsoft.EntityFrameworkCore;
using MyApp.Data.Models;

namespace MyApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<Quest> Quests { get; set; } = null!;
    public DbSet<Playthrough> Playthroughs { get; set; } = null!;

    public static string DbPath
    {
        get
        {
            var folder = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

            Directory.CreateDirectory(folder);

            return Path.Combine(folder, "questpoint.db");
        }
    }

    protected override void OnConfiguring(
        DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>().ToTable("Teams");
        modelBuilder.Entity<Quest>().ToTable("Quests");
        modelBuilder.Entity<Playthrough>().ToTable("Playthroughs");

        modelBuilder.Entity<Playthrough>()
            .HasOne(p => p.Team)
            .WithMany(t => t.Playthroughs)
            .HasForeignKey(p => p.TeamId);

        modelBuilder.Entity<Playthrough>()
            .HasOne(p => p.Quest)
            .WithMany(q => q.Playthroughs)
            .HasForeignKey(p => p.QuestId);
    }
}