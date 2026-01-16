using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SHSOS.Data;
using SHSOS.Models;

namespace SHSOS.Services
{
    public class AnalyticsService
    {
        private readonly SHSOSDbContext _context;

        public AnalyticsService(SHSOSDbContext context)
        {
            _context = context;
        }

        // ===== ENERGY ANALYTICS =====

        public decimal GetTotalEnergyConsumption(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.EnergyConsumption.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(e => e.ConsumptionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.ConsumptionDate <= endDate.Value);

            return query.Sum(e => (decimal?)e.UnitsConsumedkWh) ?? 0;
        }

        public decimal GetAverageEnergyConsumption(int departmentId, int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            var total = _context.EnergyConsumption
                .Where(e => e.DepartmentID == departmentId && e.ConsumptionDate >= cutoffDate)
                .Sum(e => (decimal?)e.UnitsConsumedkWh) ?? 0;

            return total / days;
        }

        public Dictionary<string, decimal> GetEnergyByDepartment()
        {
            return _context.EnergyConsumption
                .Include(e => e.Departments)
                .GroupBy(e => e.Departments.DepartmentName)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.UnitsConsumedkWh));
        }

        public List<(DateTime Date, decimal Consumption)> GetEnergyTrend(int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            return _context.EnergyConsumption
                .Where(e => e.ConsumptionDate >= cutoffDate)
                .GroupBy(e => e.ConsumptionDate.Date)
                .Select(g => new { Date = g.Key, Consumption = g.Sum(e => e.UnitsConsumedkWh) })
                .OrderBy(x => x.Date)
                .AsEnumerable()
                .Select(x => (x.Date, x.Consumption))
                .ToList();
        }

        public decimal GetPeakEnergyUsage()
        {
            return _context.EnergyConsumption
                .Where(e => e.PeakHourFlag)
                .Sum(e => (decimal?)e.UnitsConsumedkWh) ?? 0;
        }

        // ===== WATER ANALYTICS =====

        public decimal GetTotalWaterConsumption(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.WaterConsumption.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(w => w.ConsumptionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(w => w.ConsumptionDate <= endDate.Value);

            return query.Sum(w => (decimal?)w.UnitsConsumedLiters) ?? 0;
        }

        public int GetLeakageCount(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.WaterConsumption.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(w => w.ConsumptionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(w => w.ConsumptionDate <= endDate.Value);

            return query.Count(w => w.LeakageDetected);
        }

        public Dictionary<string, decimal> GetWaterByDepartment()
        {
            return _context.WaterConsumption
                .Include(w => w.Departments)
                .GroupBy(w => w.Departments.DepartmentName)
                .ToDictionary(g => g.Key, g => g.Sum(w => w.UnitsConsumedLiters));
        }

        // ===== WASTE ANALYTICS =====

        public decimal GetTotalWasteGenerated(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.WasteManagement.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(w => w.CollectionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(w => w.CollectionDate <= endDate.Value);

            return query.Sum(w => (decimal?)w.WasteWeight) ?? 0;
        }

        public Dictionary<string, decimal> GetWasteByCategory()
        {
            return _context.WasteManagement
                .GroupBy(w => w.WasteCategory)
                .ToDictionary(g => g.Key, g => g.Sum(w => w.WasteWeight));
        }

        public int GetNonCompliantWasteCount()
        {
            return _context.WasteManagement
                .Count(w => w.ComplianceStatus != "Compliant");
        }

        // ===== COST ANALYTICS =====

        public decimal GetTotalEnergyCost(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.EnergyConsumption.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(e => e.ConsumptionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.ConsumptionDate <= endDate.Value);

            return query.Sum(e => (decimal?)e.TotalCost) ?? 0;
        }

        public decimal GetTotalWasteCost(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.WasteManagement.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(w => w.CollectionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(w => w.CollectionDate <= endDate.Value);

            return query.Sum(w => (decimal?)(w.DisposalCost + w.DisinfectionCost)) ?? 0;
        }

        public decimal GetTotalCarbonEmissions(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.EnergyConsumption.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(e => e.ConsumptionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.ConsumptionDate <= endDate.Value);

            return query.Sum(e => (decimal?)e.CarbonEmissionsKg) ?? 0;
        }
    }
}
