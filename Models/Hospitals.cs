using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHSOS.Models
{
    [Table("hospitals", Schema = "snot")]

    public class hospitals
    {
        [Key]
        public int HospitalID { get; set; }

        public string? HospitalName { get; set; }
        public string? Location { get; set; }

        // 1 Hospital → Many Departments
        public ICollection<Departments>? Departments { get; set; }
    }
}
