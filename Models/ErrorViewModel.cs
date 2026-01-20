using System.ComponentModel.DataAnnotations.Schema;

namespace SHSOS.Models
{
    [Table("ErrorViewModel", Schema = "snot")]

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
