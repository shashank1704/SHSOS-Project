using System;
using System.Collections.Generic;
using System.Linq;
using SHSOS.Data;
using SHSOS.Models;

namespace SHSOS.Services
{
    public class PredictionService
    {
        private readonly SHSOSDbContext _context;

        public PredictionService(SHSOSDbContext context)
        {
            _context = context;
        }

        // ===== ENERGY PREDICTIONS =====

        public decimal PredictEnergyConsumption(int departmentId, int daysAhead = 7)
        {
            var historicalData = _context.EnergyConsumption
                .Where(e => e.DepartmentID == departmentId)
                .OrderByDescending(e => e.ConsumptionDate)
                .Take(30)
                .Select(e => e.UnitsConsumedkWh)
                .ToList();

            if (historicalData.Count < 7)
                return 0; // Not enough data

            // Simple moving average
            var average = historicalData.Average();
            
            // Calculate trend (linear regression simplified)
            var trend = CalculateTrend(historicalData);

            return average + (trend * daysAhead);
        }

        public List<(DateTime Date, decimal PredictedConsumption)> PredictEnergyTrend(int days = 30, int forecastDays = 7)
        {
            var predictions = new List<(DateTime, decimal)>();
            var historicalData = _context.EnergyConsumption
                .Where(e => e.ConsumptionDate >= DateTime.Now.AddDays(-days))
                .GroupBy(e => e.ConsumptionDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(e => e.UnitsConsumedkWh) })
                .OrderBy(x => x.Date)
                .ToList();

            if (historicalData.Count < 7)
                return predictions;

            var values = historicalData.Select(x => x.Total).ToList();
            var trend = CalculateTrend(values);
            var average = values.Average();

            var lastDate = historicalData.Last().Date;

            for (int i = 1; i <= forecastDays; i++)
            {
                var predictedValue = average + (trend * i);
                predictions.Add((lastDate.AddDays(i), predictedValue));
            }

            return predictions;
        }

        // ===== COST PREDICTIONS =====

        public decimal PredictEnergyCost(int departmentId, int daysAhead = 30)
        {
            var historicalData = _context.EnergyConsumption
                .Where(e => e.DepartmentID == departmentId)
                .OrderByDescending(e => e.ConsumptionDate)
                .Take(30)
                .ToList();

            if (historicalData.Count < 7)
                return 0;

            var avgDailyCost = historicalData.Average(e => e.TotalCost);
            var costTrend = CalculateTrend(historicalData.Select(e => e.TotalCost).ToList());

            return (avgDailyCost * daysAhead) + (costTrend * daysAhead * daysAhead / 2);
        }

        public decimal PredictTotalMonthlyCost()
        {
            var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var daysElapsed = (DateTime.Now - currentMonthStart).Days + 1;

            // Energy cost
            var energyCostSoFar = _context.EnergyConsumption
                .Where(e => e.ConsumptionDate >= currentMonthStart)
                .Sum(e => (decimal?)e.TotalCost) ?? 0;

            // Waste cost
            var wasteCostSoFar = _context.WasteManagement
                .Where(w => w.CollectionDate >= currentMonthStart)
                .Sum(w => (decimal?)(w.DisposalCost + w.DisinfectionCost)) ?? 0;

            var totalCostSoFar = energyCostSoFar + wasteCostSoFar;

            if (daysElapsed == 0)
                return 0;

            var avgDailyCost = totalCostSoFar / daysElapsed;
            return totalCostSoFar + (avgDailyCost * (daysInMonth - daysElapsed));
        }

        // ===== ANOMALY DETECTION =====

        public List<(DateTime Date, string Type, decimal Value, string Reason)> DetectAnomalies(int days = 30)
        {
            var anomalies = new List<(DateTime, string, decimal, string)>();

            // Energy anomalies
            var energyData = _context.EnergyConsumption
                .Where(e => e.ConsumptionDate >= DateTime.Now.AddDays(-days))
                .ToList();

            if (energyData.Any())
            {
                var avgEnergy = energyData.Average(e => e.UnitsConsumedkWh);
                var threshold = avgEnergy * 1.5m; // 50% above average

                foreach (var record in energyData.Where(e => e.UnitsConsumedkWh > threshold))
                {
                    anomalies.Add((
                        record.ConsumptionDate,
                        "Energy",
                        record.UnitsConsumedkWh,
                        $"Consumption {((record.UnitsConsumedkWh / avgEnergy - 1) * 100):F1}% above average"
                    ));
                }
            }

            // Water leakage anomalies
            var leakages = _context.WaterConsumption
                .Where(w => w.LeakageDetected && w.ConsumptionDate >= DateTime.Now.AddDays(-days))
                .ToList();

            foreach (var leak in leakages)
            {
                anomalies.Add((
                    leak.ConsumptionDate,
                    "Water",
                    leak.UnitsConsumedLiters,
                    "Leakage detected"
                ));
            }

            // Waste compliance anomalies
            var nonCompliant = _context.WasteManagement
                .Where(w => w.ComplianceStatus != "Compliant" && w.CollectionDate >= DateTime.Now.AddDays(-days))
                .ToList();

            foreach (var waste in nonCompliant)
            {
                anomalies.Add((
                    waste.CollectionDate,
                    "Waste",
                    waste.WasteWeight,
                    $"Non-compliant: {waste.ComplianceStatus}"
                ));
            }

            return anomalies.OrderByDescending(a => a.Item1).ToList();
        }

        // ===== HELPER METHODS =====

        private decimal CalculateTrend(List<decimal> values)
        {
            if (values.Count < 2)
                return 0;

            var n = values.Count;
            var sumX = 0m;
            var sumY = 0m;
            var sumXY = 0m;
            var sumX2 = 0m;

            for (int i = 0; i < n; i++)
            {
                var x = i + 1;
                var y = values[i];
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            // Slope of linear regression
            var slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            return slope;
        }
    }
}
