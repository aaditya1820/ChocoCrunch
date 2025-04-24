using System.ComponentModel.DataAnnotations;

namespace Aaditya.Models // Make sure this matches your folder & usage
{
    public class Seller
    {
        [Key]
        public int SellerID { get; set; }

        [Required]
        public string SellerName { get; set; } = string.Empty;

        [Required]
        public string SellerEmail { get; set; } = string.Empty;

        [Required]
        public string SellerBrand { get; set; } = string.Empty;

        public string? SellerAddress { get; set; }

        [Required]
        public string SellerContact { get; set; } = string.Empty;
    }
}
