using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHSOS.Models
{
    public class SustainabilityMetrics
    {
        [Key]
        public int MetricID { get; set; }

        // 🔑 Foreign Key
        public int DepartmentID { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Departments Departments { get; set; }

        public DateTime CalculationDate { get; set; }

        // Sustainability Score (0-100)
        public decimal SustainabilityScore { get; set; }

        // Individual Component Scores
        public decimal EnergyEfficiencyScore { get; set; }
        public decimal WaterEfficiencyScore { get; set; }
        public decimal WasteManagementScore { get; set; }

        // Carbon Footprint
        public decimal TotalCarbonEmissionKg { get; set; }
        public decimal CarbonReductionPercentage { get; set; }

        // Cost Metrics
        public decimal TotalCost { get; set; }
        public decimal PotentialSavings { get; set; }
        public decimal CostSavingsPercentage { get; set; }

        // Efficiency Metrics
        public decimal EnergyPerBed { get; set; } // kWh per bed
        public decimal WaterPerBed { get; set; } // Liters per bed
        public decimal WastePerBed { get; set; } // Kg per bed

        // Compliance
        public int ComplianceViolations { get; set; }

        public DateTime RecordedAt { get; set; }

        [StringLength(1000)]
        public string Recommendations { get; set; }
    }
}
