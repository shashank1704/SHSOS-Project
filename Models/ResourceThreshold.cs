using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHSOS.Models
{
    
[Table("ResourceThreshold", Schema = "snot")]
    public class ResourceThreshold
    {
        [Key]
        public int ThresholdID { get; set; }

        // 🔑 Foreign Key
        public int DepartmentID { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Departments Departments { get; set; }

        [Required]
        [StringLength(50)]
        public string ResourceType { get; set; } // Energy, Water, Waste

        [Required]
        [StringLength(50)]
        public string ThresholdName { get; set; } // Daily Limit, Peak Hour, etc.

        public decimal WarningThreshold { get; set; }
        public decimal CriticalThreshold { get; set; }

        [StringLength(20)]
        public string Unit { get; set; } // kWh, Liters, Kg

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
