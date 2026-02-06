using Microsoft.EntityFrameworkCore;
using SHSOS.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System;
using System.Linq;

namespace SHSOS.Data
{
    public class SHSOSDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SHSOSDbContext(DbContextOptions<SHSOSDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // ===== DbSets (Tables) =====
        public DbSet<hospitals> hospitals { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<WasteManagement> WasteManagement { get; set; }
        public DbSet<EnergyConsumption> EnergyConsumption { get; set; }
        public DbSet<WaterConsumption> WaterConsumption { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<ResourceThreshold> ResourceThreshold { get; set; }
        public DbSet<SustainabilityMetrics> SustainabilityMetrics { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public override int SaveChanges()
        {
            OnBeforeSaveChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaveChanges()
        {
            var auditEntries = new List<AuditLog>();
            var entries = ChangeTracker.Entries();

            var user = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            foreach (var entry in entries)
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    PerformedBy = user,
                    Timestamp = DateTime.UtcNow
                };

                var changes = new Dictionary<string, object>();

                if (entry.State == EntityState.Added)
                {
                    foreach (var property in entry.CurrentValues.Properties)
                    {
                        changes[property.Name] = entry.CurrentValues[property];
                    }
                }
                else if (entry.State == EntityState.Deleted)
                {
                    foreach (var property in entry.OriginalValues.Properties)
                    {
                        changes[property.Name] = entry.OriginalValues[property];
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    foreach (var property in entry.OriginalValues.Properties)
                    {
                        var originalValue = entry.OriginalValues[property];
                        var currentValue = entry.CurrentValues[property];

                        if (!Equals(originalValue, currentValue))
                        {
                            changes[property.Name] = new { Old = originalValue, New = currentValue };
                        }
                    }
                }

                auditEntry.Changes = System.Text.Json.JsonSerializer.Serialize(changes);
                auditEntries.Add(auditEntry);
            }

            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("snot");

            // ===== Hospital → Departments (1:M) =====
            modelBuilder.Entity<Departments>()
                .HasOne(d => d.hospitals)
                .WithMany(h => h.Departments)
                .HasForeignKey(d => d.HospitalID);

            // ===== Departments → WasteManagement (1:M) =====
            modelBuilder.Entity<WasteManagement>()
                .HasOne(w => w.Departments)
                .WithMany(d => d.WasteManagement)
                .HasForeignKey(w => w.DepartmentID);

            // ===== Departments → EnergyConsumption (1:M) =====
            modelBuilder.Entity<EnergyConsumption>()
                .HasOne(e => e.Departments)
                .WithMany(d => d.EnergyConsumption)
                .HasForeignKey(e => e.DepartmentID);

            // ===== Departments → WaterConsumption (1:M) =====
            modelBuilder.Entity<WaterConsumption>()
                .HasOne(w => w.Departments)
                .WithMany(d => d.WaterConsumption)
                .HasForeignKey(w => w.DepartmentID);

            // ===== Departments → Alert (1:M) =====
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Departments)
                .WithMany()
                .HasForeignKey(a => a.DepartmentID);

            // ===== Departments → ResourceThreshold (1:M) =====
            modelBuilder.Entity<ResourceThreshold>()
                .HasOne(rt => rt.Departments)
                .WithMany()
                .HasForeignKey(rt => rt.DepartmentID);

            // ===== Departments → SustainabilityMetrics (1:M) =====
            modelBuilder.Entity<SustainabilityMetrics>()
                .HasOne(sm => sm.Departments)
                .WithMany()
                .HasForeignKey(sm => sm.DepartmentID);
        }
    }
}

