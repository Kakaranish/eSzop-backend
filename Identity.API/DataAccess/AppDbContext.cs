﻿using Identity.API.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace Identity.API.DataAccess
{
    public class AppDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public DbSet<User> Users { get; private set; }
        public DbSet<RefreshToken> RefreshTokens { get; private set; }
        public DbSet<ProfileInfo> ProfileInfos { get; private set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; private set; }
        public DbSet<AboutSeller> AboutSellers { get; private set; }

        public AppDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>()
                .Property(x => x.HashedPassword)
                .HasConversion(x => x.ToString(), x => new HashedPassword(x));
            modelBuilder.Entity<User>()
                .Property(x => x.Role)
                .HasConversion(x => x.Name, x => Role.Parse(x));

            modelBuilder.Entity<RefreshToken>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ProfileInfo>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<DeliveryAddress>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<AboutSeller>()
                .HasKey(x => x.Id);

            modelBuilder.Seed(_serviceProvider);
        }
    }
}
