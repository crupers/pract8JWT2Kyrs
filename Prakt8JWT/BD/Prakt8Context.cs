using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Prakt8JWT.DbModels;
using Task = Prakt8JWT.DbModels.Task;

namespace Prakt8JWT.BD;

public partial class Prakt8Context : DbContext
{
    public Prakt8Context(DbContextOptions<Prakt8Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Task_1");

            entity.ToTable("Task");

            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .IsFixedLength();
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
            entity.Property(e => e.OwnerLogin)
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.Title)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.OwnerLoginNavigation).WithMany(p => p.Tasks)
                .HasPrincipalKey(p => p.Login)
                .HasForeignKey(d => d.OwnerLogin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_User_1");

            entity.ToTable("User");

            entity.HasIndex(e => e.Login, "IX_User").IsUnique();

            entity.Property(e => e.Login)
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Role)
                .HasMaxLength(5)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
