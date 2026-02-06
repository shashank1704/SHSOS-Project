using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHSOS.Models
{
    [Table("WaterConsumption", Schema = "snot")]
    public class WaterConsumption
    {
        [Key]
        public int ConsumptionID { get; set; }

        // 🔑 Foreign Key
        public int DepartmentID { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Departments? Departments { get; set; }

        [NotMapped]
        public string? DepartmentName { get; set; }

        public DateTime ConsumptionDate { get; set; }
        public TimeSpan ReadingTime { get; set; }

        public decimal ReadingEnd { get; set; }
        public decimal UnitsConsumedLiters { get; set; }

        public decimal UnitCost { get; set; }
        public bool LeakageDetected { get; set; }

        public string? WeatherCategory { get; set; }
        public string? WeatherCondition { get; set; }

        public string? Remarks { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}

