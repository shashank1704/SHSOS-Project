using System;
using System.Collections.Generic;
using System.Linq;
using SHSOS.Data;
using SHSOS.Models;

namespace SHSOS.Services
{
    public class AlertService
    {
        private readonly SHSOSDbContext _context;

        public AlertService(SHSOSDbContext context)
        {
            _context = context;
        }

        // ===== CHECK AND CREATE ALERTS =====

        public void CheckEnergyThresholds()
        {
            var activeThresholds = _context.ResourceThreshold
                .Where(t => t.ResourceType == "Energy" && t.IsActive)
                .ToList();

            foreach (var threshold in activeThresholds)
            {
                var todayConsumption = _context.EnergyConsumption
                    .Where(e => e.DepartmentID == threshold.DepartmentID && 
                                e.ConsumptionDate.Date == DateTime.Today)
                    .Sum(e => (decimal?)e.UnitsConsumedkWh) ?? 0;

                if (todayConsumption >= threshold.CriticalThreshold)
                {
                    CreateAlert(threshold.DepartmentID, "Energy", "Critical",
                        $"Energy consumption ({todayConsumption} kWh) exceeded critical threshold ({threshold.CriticalThreshold} kWh)",
                        threshold.CriticalThreshold, todayConsumption);
                }
                else if (todayConsumption >= threshold.WarningThreshold)
                {
                    CreateAlert(threshold.DepartmentID, "Energy", "High",
                        $"Energy consumption ({todayConsumption} kWh) exceeded warning threshold ({threshold.WarningThreshold} kWh)",
                        threshold.WarningThreshold, todayConsumption);
                }
            }
        }

        public void CheckWaterThresholds()
        {
            var activeThresholds = _context.ResourceThreshold
                .Where(t => t.ResourceType == "Water" && t.IsActive)
                .ToList();

            foreach (var threshold in activeThresholds)
            {
                var todayConsumption = _context.WaterConsumption
                    .Where(w => w.DepartmentID == threshold.DepartmentID && 
                                w.ConsumptionDate.Date == DateTime.Today)
                    .Sum(w => (decimal?)w.UnitsConsumedLiters) ?? 0;

                if (todayConsumption >= threshold.CriticalThreshold)
                {
                    CreateAlert(threshold.DepartmentID, "Water", "Critical",
                        $"Water consumption ({todayConsumption} L) exceeded critical threshold ({threshold.CriticalThreshold} L)",
                        threshold.CriticalThreshold, todayConsumption);
                }
                else if (todayConsumption >= threshold.WarningThreshold)
                {
                    CreateAlert(threshold.DepartmentID, "Water", "High",
                        $"Water consumption ({todayConsumption} L) exceeded warning threshold ({threshold.WarningThreshold} L)",
                        threshold.WarningThreshold, todayConsumption);
                }
            }

            // Check for leakages
            var todayLeakages = _context.WaterConsumption
                .Where(w => w.LeakageDetected && w.ConsumptionDate.Date == DateTime.Today)
                .ToList();

            foreach (var leak in todayLeakages)
            {
                CreateAlert(leak.DepartmentID, "Water", "Critical",
                    $"Water leakage detected! Consumption: {leak.UnitsConsumedLiters} L",
                    0, leak.UnitsConsumedLiters);
            }
        }

        public void CheckWasteCompliance()
        {
            var todayWaste = _context.WasteManagement
                .Where(w => w.CollectionDate.Date == DateTime.Today && w.ComplianceStatus != "Compliant")
                .ToList();

            foreach (var waste in todayWaste)
            {
                var severity = waste.WasteCategory == "Hazardous" ? "Critical" : "High";
                CreateAlert(waste.DepartmentID, "Waste", severity,
                    $"Non-compliant waste disposal: {waste.WasteType} - {waste.ComplianceStatus}",
                    0, waste.WasteWeight);
            }
        }

        private void CreateAlert(int departmentId, string alertType, string severity, 
                                 string message, decimal thresholdValue, decimal actualValue)
        {
            // Check if similar alert already exists today
            var existingAlert = _context.Alerts
                .FirstOrDefault(a => a.DepartmentID == departmentId &&
                                    a.AlertType == alertType &&
                                    a.CreatedAt.Date == DateTime.Today &&
                                    !a.IsResolved);

            if (existingAlert != null)
                return; // Don't create duplicate

            var alert = new Alert
            {
                DepartmentID = departmentId,
                AlertType = alertType,
                Severity = severity,
                Message = message,
                ThresholdValue = thresholdValue,
                ActualValue = actualValue,
                CreatedAt = DateTime.Now,
                IsResolved = false
            };

            _context.Alerts.Add(alert);
            _context.SaveChanges();
        }

        // ===== ALERT MANAGEMENT =====

        public List<Alert> GetActiveAlerts()
        {
            return _context.Alerts
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        public List<Alert> GetAlertsByDepartment(int departmentId, bool includeResolved = false)
        {
            var query = _context.Alerts.Where(a => a.DepartmentID == departmentId);

            if (!includeResolved)
                query = query.Where(a => !a.IsResolved);

            return query.OrderByDescending(a => a.CreatedAt).ToList();
        }

        public void ResolveAlert(int alertId, string resolutionNotes)
        {
            var alert = _context.Alerts.Find(alertId);
            if (alert != null)
            {
                alert.IsResolved = true;
                alert.ResolvedAt = DateTime.Now;
                alert.ResolutionNotes = resolutionNotes;
                _context.SaveChanges();
            }
        }

        public Dictionary<string, int> GetAlertSummary()
        {
            var activeAlerts = _context.Alerts.Where(a => !a.IsResolved).ToList();

            return new Dictionary<string, int>
            {
                { "Critical", activeAlerts.Count(a => a.Severity == "Critical") },
                { "High", activeAlerts.Count(a => a.Severity == "High") },
                { "Medium", activeAlerts.Count(a => a.Severity == "Medium") },
                { "Low", activeAlerts.Count(a => a.Severity == "Low") }
            };
        }
    }
}
