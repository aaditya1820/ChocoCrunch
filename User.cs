using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aaditya.Models
{
    [Table("User", Schema = "dbo")]
    public class User
    {
        [Key]
        public int uID { get; set; }

        [Required]
        public string uName { get; set; }

        [Required, EmailAddress]
        public string uEmail { get; set; }

        [Required]
        public string uContact { get; set; }

        public string uAddress { get; set; }

        public string uPincode { get; set; }
    }
}