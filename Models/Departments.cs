using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHSOS.Models
{
    [Table("Departments", Schema = "snot")]

    public class Departments
    {
        [Key]
        public int DepartmentID { get; set; }

        // 🔑 Foreign Key
        public int HospitalID { get; set; }

        [ForeignKey(nameof(HospitalID))]
        public hospitals hospitals { get; set; }

        public string DepartmentName { get; set; }
        public int FloorNumber { get; set; }
        public bool Inactive { get; set; }

        // 1 Department → Many Records
        public ICollection<WasteManagement> WasteManagement { get; set; }
        public ICollection<EnergyConsumption> EnergyConsumption { get; set; }
        public ICollection<WaterConsumption> WaterConsumption { get; set; }
    }
}

