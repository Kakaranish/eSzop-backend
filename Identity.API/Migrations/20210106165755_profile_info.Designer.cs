﻿// <auto-generated />
using System;
using Identity.API.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Identity.API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210106165755_profile_info")]
    partial class profile_info
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("Identity.API.Domain.ProfileInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("ProfileInfos");
                });

            modelBuilder.Entity("Identity.API.Domain.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RevokedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("Identity.API.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HashedPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("0b7e7262-e06a-436d-9049-6b7655d8e91d"),
                            CreatedAt = new DateTime(2021, 1, 6, 16, 57, 55, 124, DateTimeKind.Utc).AddTicks(8285),
                            Email = "admin@mail.com",
                            HashedPassword = "90aFcZq9I2zPmRXnQzWrItNtyzwmd7QPCRMQLmsfX1o=|LVdpW3tdc4GMZETuVTr4ug==|10000",
                            Role = "admin",
                            UpdatedAt = new DateTime(2021, 1, 6, 16, 57, 55, 124, DateTimeKind.Utc).AddTicks(8638)
                        });
                });

            modelBuilder.Entity("Identity.API.Domain.ProfileInfo", b =>
                {
                    b.HasOne("Identity.API.Domain.User", null)
                        .WithOne("ProfileInfo")
                        .HasForeignKey("Identity.API.Domain.ProfileInfo", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.API.Domain.User", b =>
                {
                    b.Navigation("ProfileInfo");
                });
#pragma warning restore 612, 618
        }
    }
}