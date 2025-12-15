using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PJ_P_Installation_Management_System.Models;
using System;

namespace PJ_P_Installation_Management_System.Data
{
    public class PJInstallationDbContext : DbContext
    {
        public PJInstallationDbContext(DbContextOptions<PJInstallationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<ProductSupplier> ProductSuppliers { get; set; }

        public DbSet<CustomerPurchase> CustomerPurchases { get; set; }
        public DbSet<CustomerPurchaseItem> CustomerPurchaseItems { get; set; }

        // 🔑 NEW: join table for many-to-many (Schedule ↔ Staff)
        public DbSet<ScheduleStaff> ScheduleStaffs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Purchase -> Supplier
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            // PurchaseItem join table
            modelBuilder.Entity<PurchaseItem>()
                .HasKey(pi => pi.PurchaseItemId);

            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Purchase)
                .WithMany(p => p.PurchaseItems)
                .HasForeignKey(pi => pi.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.PurchaseItems)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(p => p.Description)
                    .HasMaxLength(500);

            });

            // Supplier
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(s => s.CompanyName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(s => s.ContactPerson)
                    .HasMaxLength(50);

                entity.Property(s => s.Phone)
                    .HasMaxLength(20);

                entity.Property(s => s.Email)
                    .HasMaxLength(100);

                entity.Property(s => s.IsActive)
                    .HasDefaultValue(true);
            });

            // ProductSupplier many-to-many
            modelBuilder.Entity<ProductSupplier>()
                .HasKey(ps => new { ps.ProductId, ps.SupplierId });

            modelBuilder.Entity<ProductSupplier>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSuppliers)
                .HasForeignKey(ps => ps.ProductId);

            modelBuilder.Entity<ProductSupplier>()
                .HasOne(ps => ps.Supplier)
                .WithMany(s => s.ProductSuppliers)
                .HasForeignKey(ps => ps.SupplierId);

            // CustomerPurchase -> Schedule
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.CustomerPurchase)
                .WithMany(cp => cp.Schedules)
                .HasForeignKey(s => s.CustomerPurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔑 NEW: ScheduleStaff many-to-many
            modelBuilder.Entity<ScheduleStaff>()
                .HasKey(ss => new { ss.ScheduleId, ss.StaffId });

            modelBuilder.Entity<ScheduleStaff>()
                .HasOne(ss => ss.Schedule)
                .WithMany(s => s.StaffAssignments)
                .HasForeignKey(ss => ss.ScheduleId);

            modelBuilder.Entity<ScheduleStaff>()
                .HasOne(ss => ss.Staff)
                .WithMany(st => st.ScheduleAssignments)
                .HasForeignKey(ss => ss.StaffId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Only used for design-time migration creation
                optionsBuilder.UseSqlServer("YourConnectionString");
            }

            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
