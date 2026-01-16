using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHSOS.Models
{
    public class EnergyConsumption
    {
        [Key]
        public int EnergyConsumptionID { get; set; }

        // 🔑 Foreign Key
        public int DepartmentID { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Departments Departments { get; set; }

        public DateTime ConsumptionDate { get; set; }
        public TimeSpan ReadingTime { get; set; }

        public decimal MeterReadingStart { get; set; }
        public decimal UnitsConsumedkWh { get; set; }

        public decimal UnitCost { get; set; }
        public string UsageCategory { get; set; }
        public bool PeakHourFlag { get; set; }

        public decimal TotalCost { get; set; }
        public decimal CarbonEmissionsKg { get; set; }

        public DateTime RecordedAt { get; set; }
    }
}

