using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RaspberryPiWebServer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPiWebServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Model> Models { get; set; }
        public virtual DbSet<History> Histories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=ktf.db;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model>(entity =>
            {
                entity.ToTable("Model");

                entity.HasIndex(e => e.Name, "UQ__Model__737584F6098D37A7")
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.ToTable("History");

                entity.Property(e => e.ModelId)
                    .IsRequired();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Template)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IsOk)
                    .IsRequired();

                entity.Property(e => e.CreateDate)
                    .IsRequired();

                entity.HasOne(d => d.Model)
                    .WithMany(p => p.Histories)
                    .HasForeignKey(d => d.ModelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_History_Model");

                entity.HasIndex(e => e.CreateDate, "UQ__History__737584F6098D37A7");
            });

        }
    }
}
