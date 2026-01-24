using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHSOS.Models
{
    [Table("WasteManagement", Schema = "snot")]
    public class WasteManagement
    {
        [Key]
        public int WasteRecordID { get; set; }

        // 🔑 Foreign Key
        public int DepartmentID { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Departments? Departments { get; set; }

        public string? WasteType { get; set; }
        public string? WasteCategory { get; set; }
        public decimal WasteWeight { get; set; }
        public string? SegregationStatus { get; set; }
        public string? DisposalMethod { get; set; }

        public decimal DisposalCost { get; set; }
        public decimal DisinfectionCost { get; set; }
        public string? ComplianceStatus { get; set; }

        public DateTime CollectionDate { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}

