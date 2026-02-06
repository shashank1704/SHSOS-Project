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

        // ===== CLEANUP ALERTS =====
        
        public void PurgeInvalidAlerts()
        {
            // This method removes alerts that were manually seeded or no longer match threshold conditions
            var activeAlerts = _context.Alerts.Where(a => !a.IsResolved).ToList();
            var thresholds = _context.ResourceThreshold.ToList();
            
            var alertsToRemove = new List<Alert>();

            foreach (var alert in activeAlerts)
            {
                // Never purge leakage alerts as they are event-based, not threshold-sum-based
                if (alert.AlertType == "Water" && alert.Message?.Contains("leakage", StringComparison.OrdinalIgnoreCase) == true)
                    continue;

                var threshold = thresholds.FirstOrDefault(t => t.DepartmentID == alert.DepartmentID && t.ResourceType == alert.AlertType);
                
                // If no threshold exists for this type/dept, or if the actual value is below the warning threshold, mark for removal
                if (threshold == null || alert.ActualValue < threshold.WarningThreshold)
                {
                    alertsToRemove.Add(alert);
                }
            }

            if (alertsToRemove.Any())
            {
                _context.Alerts.RemoveRange(alertsToRemove);
                _context.SaveChanges();
            }
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
            {
                // If the new severity is higher, or if the current value is significantly higher than before, update the alert
                if (GetSeverityRank(severity) > GetSeverityRank(existingAlert.Severity) || actualValue > existingAlert.ActualValue)
                {
                    existingAlert.Severity = severity;
                    existingAlert.Message = message;
                    existingAlert.ActualValue = actualValue;
                    existingAlert.ThresholdValue = thresholdValue;
                    existingAlert.CreatedAt = DateTime.Now; // Update timestamp to show latest activity
                    _context.SaveChanges();
                }
                return;
            }

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

        private int GetSeverityRank(string severity)
        {
            return severity switch
            {
                "Critical" => 4,
                "High" => 3,
                "Medium" => 2,
                "Low" => 1,
                _ => 0
            };
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
