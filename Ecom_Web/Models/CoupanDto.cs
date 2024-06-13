using System.ComponentModel.DataAnnotations;

namespace Ecom_Web.Models
{
    public class CoupanDto
    {
        [Key]
        public int CoupanId { get; set; }
        [Required]
        public string CoupanCode { get; set; }
        [Required]
        public double DiscountAmount { get; set; }

        public int MinAmount { get; set; }
    }
}
