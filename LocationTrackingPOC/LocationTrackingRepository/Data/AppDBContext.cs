using LocationTrackingRepository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<UserDb> Users { get; set; }
        public DbSet<UserStatusDb> UserStatus { get; set; }
        public DbSet<RoleDb> Roles { get; set; }
        public DbSet<AddressDb> Addresses { get; set; }
        public DbSet<DriverDb> Drivers { get; set; }
        public DbSet<DriverLocationDb> DriverLocations { get; set; }
        public DbSet<DriverStatusDb> DriverStatus { get; set; }
        public DbSet<CollectionRequestDb> Rides { get; set; }
        public DbSet<CollectionStatusDb> CollectionStatus{get;set;}
        public DbSet<DropLocationDb> DropLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure self-referencing relationships
            modelBuilder.Entity<UserDb>()
                .HasOne(u => u.CreatedByUser)
                .WithMany()
                .HasForeignKey(u => u.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserDb>()
                .HasOne(u => u.UpdatedByUser)
                .WithMany()
                .HasForeignKey(u => u.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserStatusDb>()
                .HasOne(us => us.CreatedByUser)
                .WithMany()
                .HasForeignKey(us => us.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserStatusDb>()
                .HasOne(us => us.UpdatedByUser)
                .WithMany()
                .HasForeignKey(us => us.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleDb>()
                .HasOne(r => r.CreatedByUser)
                .WithMany()
                .HasForeignKey(r => r.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleDb>()
                .HasOne(r => r.UpdatedByUser)
                .WithMany()
                .HasForeignKey(r => r.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DriverStatusDb>()
                .HasOne(ds => ds.CreatedByUser)
                .WithMany()
                .HasForeignKey(ds => ds.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DriverStatusDb>()
                .HasOne(ds => ds.UpdatedByUser)
                .WithMany()
                .HasForeignKey(ds => ds.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CollectionStatusDb>()
                .HasOne(cs => cs.CreatedByUser)
                .WithMany()
                .HasForeignKey(cs => cs.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CollectionStatusDb>()
                .HasOne(cs => cs.UpdatedByUser)
                .WithMany()
                .HasForeignKey(cs => cs.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data will be added in migration
        }
    }
    }
