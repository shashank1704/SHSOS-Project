using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SHSOS.Models
{
    public class hospitals
    {
        [Key]
        public int HospitalID { get; set; }

        public string HospitalName { get; set; }
        public string Location { get; set; }

        // 1 Hospital → Many Departments
        public ICollection<Departments> Departments { get; set; }
    }
}
