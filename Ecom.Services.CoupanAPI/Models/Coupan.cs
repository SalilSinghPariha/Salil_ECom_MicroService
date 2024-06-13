using System.ComponentModel.DataAnnotations;

namespace Ecom.Services.CoupanAPI.Models
{
    public class Coupan
    {
        [Key]
        public int CoupanId { get; set; }
        [Required]
        public string CoupanCode { get; set; }
        [Range(1, 1000)]
        public double DiscountAmount { get; set; }
        [Range(1, 1000)]
        public int MinAmount { get; set; }
    }

}
