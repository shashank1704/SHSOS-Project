using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHSOS.Models
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int AuditLogID { get; set; }

        [Required]
        public string EntityName { get; set; }

        [Required]
        public string Action { get; set; } // Insert, Update, Delete

        [Required]
        public string Changes { get; set; } // JSON or text representation of changes

        [Required]
        public string PerformedBy { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
