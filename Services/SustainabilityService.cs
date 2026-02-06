using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SHSOS.Data;
using SHSOS.Models;

namespace SHSOS.Services
{
    public class SustainabilityService
    {
        private readonly SHSOSDbContext _context;

        public SustainabilityService(SHSOSDbContext context)
        {
            _context = context;
        }

        // ===== CALCULATE SUSTAINABILITY SCORE =====

        public SustainabilityMetrics CalculateSustainabilityScore(int departmentId, DateTime? date = null)
        {
            var targetDate = date ?? DateTime.Today;
            var monthStart = new DateTime(targetDate.Year, targetDate.Month, 1);

            // Get current month's data
            var energyData = _context.EnergyConsumption
                .Where(e => e.DepartmentID == departmentId && e.ConsumptionDate >= monthStart)
                .ToList();

            var waterData = _context.WaterConsumption
                .Where(w => w.DepartmentID == departmentId && w.ConsumptionDate >= monthStart)
                .ToList();

            var wasteData = _context.WasteManagement
                .Where(w => w.DepartmentID == departmentId && w.CollectionDate >= monthStart)
                .ToList();

            // Calculate component scores
            var energyScore = CalculateEnergyEfficiencyScore(energyData);
            var waterScore = CalculateWaterEfficiencyScore(waterData);
            var wasteScore = CalculateWasteManagementScore(wasteData);

            // Overall sustainability score (weighted average)
            var sustainabilityScore = (energyScore * 0.4m) + (waterScore * 0.3m) + (wasteScore * 0.3m);

            // Calculate carbon emissions
            var totalCarbon = energyData.Sum(e => e.CarbonEmissionsKg);

            // Calculate costs
            var energyCost = energyData.Sum(e => e.TotalCost);
            var wasteCost = wasteData.Sum(w => w.DisposalCost + w.DisinfectionCost);
            var totalCost = energyCost + wasteCost;

            // Calculate potential savings (based on efficiency opportunities)
            var potentialSavings = CalculatePotentialSavings(energyData, waterData, wasteData);

            // Compliance violations
            var violations = wasteData.Count(w => w.ComplianceStatus != "Compliant");

            var metrics = new SustainabilityMetrics
            {
                DepartmentID = departmentId,
                CalculationDate = targetDate,
                SustainabilityScore = Math.Round(sustainabilityScore, 2),
                EnergyEfficiencyScore = Math.Round(energyScore, 2),
                WaterEfficiencyScore = Math.Round(waterScore, 2),
                WasteManagementScore = Math.Round(wasteScore, 2),
                TotalCarbonEmissionKg = totalCarbon,
                CarbonReductionPercentage = CalculateCarbonReduction(departmentId, totalCarbon),
                TotalCost = totalCost,
                PotentialSavings = potentialSavings,
                CostSavingsPercentage = totalCost > 0 ? Math.Round((potentialSavings / totalCost) * 100, 2) : 0,
                ComplianceViolations = violations,
                RecordedAt = DateTime.Now,
                Recommendations = GenerateRecommendations(energyScore, waterScore, wasteScore, violations)
            };

            return metrics;
        }

        private decimal CalculateEnergyEfficiencyScore(List<EnergyConsumption> data)
        {
            if (!data.Any())
                return 50; // Neutral score

            var totalConsumption = data.Sum(e => e.UnitsConsumedkWh);
            var peakConsumption = data.Where(e => e.PeakHourFlag).Sum(e => e.UnitsConsumedkWh);
            var peakPercentage = totalConsumption > 0 ? (peakConsumption / totalConsumption) * 100 : 0;

            // Lower peak usage = better score
            var score = 100 - (peakPercentage * 0.5m);

            // Bonus for low overall consumption
            var avgDaily = totalConsumption / Math.Max(data.Select(e => e.ConsumptionDate.Date).Distinct().Count(), 1);
            if (avgDaily < 300) score += 10; // Low consumption bonus

            return Math.Min(100, Math.Max(0, score));
        }

        private decimal CalculateWaterEfficiencyScore(List<WaterConsumption> data)
        {
            if (!data.Any())
                return 50;

            var leakageCount = data.Count(w => w.LeakageDetected);
            var totalRecords = data.Count;
            var leakagePercentage = ((decimal)leakageCount / totalRecords) * 100;

            // No leakages = perfect score
            var score = 100 - (leakagePercentage * 10);

            return Math.Min(100, Math.Max(0, score));
        }

        private decimal CalculateWasteManagementScore(List<WasteManagement> data)
        {
            if (!data.Any())
                return 50;

            var compliantCount = data.Count(w => w.ComplianceStatus == "Compliant");
            var segregatedCount = data.Count(w => w.SegregationStatus == "Segregated");
            var totalRecords = data.Count;

            var compliancePercentage = ((decimal)compliantCount / totalRecords) * 100;
            var segregationPercentage = ((decimal)segregatedCount / totalRecords) * 100;

            // High compliance and segregation = high score
            var score = (compliancePercentage * 0.6m) + (segregationPercentage * 0.4m);

            return Math.Min(100, Math.Max(0, score));
        }

        private decimal CalculatePotentialSavings(List<EnergyConsumption> energy, 
                                                   List<WaterConsumption> water, 
                                                   List<WasteManagement> waste)
        {
            decimal savings = 0;

            // Energy savings from reducing peak hour usage
            var peakEnergy = energy.Where(e => e.PeakHourFlag).Sum(e => e.TotalCost);
            savings += peakEnergy * 0.2m; // Assume 20% savings possible

            // Water savings from fixing leakages
            var leakageWater = water.Where(w => w.LeakageDetected).ToList();
            foreach (var leak in leakageWater)
            {
                savings += leak.UnitsConsumedLiters * leak.UnitCost * 0.8m; // 80% of leaked water cost
            }

            // Waste savings from better segregation
            var nonSegregated = waste.Where(w => w.SegregationStatus != "Segregated").ToList();
            savings += nonSegregated.Sum(w => w.DisposalCost) * 0.15m; // 15% savings from segregation

            return Math.Round(savings, 2);
        }

        private decimal CalculateCarbonReduction(int departmentId, decimal currentCarbon)
        {
            // Compare with last month
            var lastMonth = DateTime.Now.AddMonths(-1);
            var lastMonthStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);

            var lastMonthCarbon = _context.EnergyConsumption
                .Where(e => e.DepartmentID == departmentId && 
                           e.ConsumptionDate >= lastMonthStart && 
                           e.ConsumptionDate <= lastMonthEnd)
                .Sum(e => (decimal?)e.CarbonEmissionsKg) ?? 0;

            if (lastMonthCarbon == 0)
                return 0;

            var reduction = ((lastMonthCarbon - currentCarbon) / lastMonthCarbon) * 100;
            return Math.Round(reduction, 2);
        }

        private string GenerateRecommendations(decimal energyScore, decimal waterScore, 
                                               decimal wasteScore, int violations)
        {
            var recommendations = new List<string>();

            if (energyScore < 70)
                recommendations.Add("Reduce peak hour consumption by shifting non-critical operations");

            if (waterScore < 70)
                recommendations.Add("Address water leakages immediately to reduce wastage");

            if (wasteScore < 70)
                recommendations.Add("Improve waste segregation practices and staff training");

            if (violations > 0)
                recommendations.Add("Urgent: Resolve compliance violations to avoid penalties");

            if (!recommendations.Any())
                recommendations.Add("Excellent performance! Maintain current sustainability practices");

            return string.Join("; ", recommendations);
        }

        // ===== SAVE METRICS =====

        public void SaveSustainabilityMetrics(SustainabilityMetrics metrics)
        {
            _context.SustainabilityMetrics.Add(metrics);
            _context.SaveChanges();
        }

        public List<SustainabilityMetrics> GetDepartmentHistory(int departmentId, int months = 6)
        {
            var cutoffDate = DateTime.Now.AddMonths(-months);
            return _context.SustainabilityMetrics
                .Where(m => m.DepartmentID == departmentId && m.CalculationDate >= cutoffDate)
                .OrderByDescending(m => m.CalculationDate)
                .ToList();
        }
    }
}
