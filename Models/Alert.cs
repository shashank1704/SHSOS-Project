using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHSOS.Models
{
    
[Table("Alert", Schema = "snot")]
    public class Alert
    {
        [Key]
        public int AlertID { get; set; }

        // 🔑 Foreign Key
        public int DepartmentID { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Departments? Departments { get; set; }

        [Required]
        [StringLength(50)]
        public string? AlertType { get; set; } // Energy, Water, Waste

        [Required]
        [StringLength(20)]
        public string? Severity { get; set; } // Low, Medium, High, Critical

        [Required]
        [StringLength(500)]
        public string? Message { get; set; }

        public decimal ThresholdValue { get; set; }
        public decimal ActualValue { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }

        [StringLength(1000)]
        public string? ResolutionNotes { get; set; }
    }
}
