using System.ComponentModel.DataAnnotations;
using Ecom_Web.Utility;

namespace Ecom_Web.Models
{
    public class ProductDTO
    {     
        public int ProductId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }

        public string Description { get; set; }

        public string CategoryName { get; set; }

        public string? ImageUrl { get; set; }

        public string? ImageLocalUrl { get; set; }

        // we will add count here since dto is not used for dbset

        [Range(1,100)]
        public int count { get; set; } = 1;

        [MaxFileSize(1)]
        [AllowedExtensions(new string[] { ".jpg",".png"})]
        public IFormFile? Image { get; set; }
    }
}
