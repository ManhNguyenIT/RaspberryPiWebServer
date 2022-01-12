﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RaspberryPiWebServer.Data;

#nullable disable

namespace RaspberryPiWebServer.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("RaspberryPiWebServer.Models.History", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsOk")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Template")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ModelId");

                    b.HasIndex(new[] { "CreateDate" }, "UQ__History__737584F6098D37A7");

                    b.ToTable("History", (string)null);
                });

            modelBuilder.Entity("RaspberryPiWebServer.Models.Model", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Name" }, "UQ__Model__737584F6098D37A7")
                        .IsUnique();

                    b.ToTable("Model", (string)null);
                });

            modelBuilder.Entity("RaspberryPiWebServer.Models.History", b =>
                {
                    b.HasOne("RaspberryPiWebServer.Models.Model", "Model")
                        .WithMany("Histories")
                        .HasForeignKey("ModelId")
                        .IsRequired()
                        .HasConstraintName("FK_History_Model");

                    b.Navigation("Model");
                });

            modelBuilder.Entity("RaspberryPiWebServer.Models.Model", b =>
                {
                    b.Navigation("Histories");
                });
#pragma warning restore 612, 618
        }
    }
}
