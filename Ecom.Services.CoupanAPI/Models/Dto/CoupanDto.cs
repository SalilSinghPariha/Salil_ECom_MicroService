using System.ComponentModel.DataAnnotations;

namespace Ecom.Services.CoupanAPI.Models.Dto
{
    public class CoupanDto
    {
 
        public int CoupanId { get; set; }

        public string CoupanCode { get; set; }

        public double DiscountAmount { get; set; }

        public int MinAmount { get; set; }
    }
}
