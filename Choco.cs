using System.ComponentModel.DataAnnotations;

namespace Aaditya.Models
{
    public class Choco
    {
        [Key]
        public int ChocolateID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Brand { get; set; }

        [StringLength(100)]
        public string Flavour { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal SellerPrice { get; set; }

        [DataType(DataType.Currency)]
        public decimal? SellingPrice { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Margin { get; set; }
    }
}
