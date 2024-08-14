using Domain;
using Microsoft.EntityFrameworkCore;

namespace InfrastructePersistence.Context;

public class ChatContext : DbContext
{
    private string _connectionString = "Host=localhost;Username=postgres;Password=1;Database=App";
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
       builder.LogTo(Console.WriteLine).UseLazyLoadingProxies().UseNpgsql(_connectionString);
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ConfigurateUsers(builder);
        ConfigurateMessages(builder);
        base.OnModelCreating(builder);
    }

    private static void ConfigurateMessages(ModelBuilder builder)
    {
        builder.Entity<MessageEntity>().HasKey(x => x.Id);
        builder.Entity<MessageEntity>().Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Entity<MessageEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.SenderId);
        builder.Entity<MessageEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.RepicientId);
    }

    private static void ConfigurateUsers(ModelBuilder builder)
    {
        builder.Entity<UserEntity>().HasKey(x => x.Id);
        builder.Entity<UserEntity>().Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Entity<UserEntity>().HasIndex(x => x.Name).IsUnique();       
    }
}
