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
        
        //builder.Entity<UserEntity>(entity =>
        //{
        //    entity.HasKey(x => x.Id).HasName("user_pkey");
        //    entity.ToTable("Users");
        //    entity.Property(x => x.Id).HasColumnName("user_id");
        //    entity.Property(x => x.Name)
        //          .HasMaxLength(255)
        //          .HasColumnName("Name");
        //});

        //builder.Entity<MessageEntity>(entity =>
        //{
        //    entity.HasKey(x => x.Id).HasName("message_pkey");
        //    entity.ToTable("Messages");
        //    entity.Property(x => x.Id).HasColumnName("message_id");
        //    entity.Property(x => x.Text).HasColumnName("text");
        //    entity.Property(x => x.SenderId).HasColumnName("from_user_id");
        //    entity.Property(x => x.RepicientId).HasColumnName("to_user_id");

        //    entity.HasOne(x => x.FromUser)
        //          .WithMany(p => p.FromMessages)
        //          .HasForeignKey(c => c.FromUserId)
        //          .HasConstraintName("messages_from_user_id_fkey");

        //    entity.HasOne(x => x.ToUser)
        //          .WithMany(p => p.ToMessages)
        //          .HasForeignKey(c => c.ToUserId)
        //          .HasConstraintName("messages_to_user_id_fkey");
        //});
        //base.OnModelCreating(builder);
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
