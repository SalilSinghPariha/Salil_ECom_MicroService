﻿using System.ComponentModel.DataAnnotations;

namespace Ecom.Service.ProductAPI.Model.DTO
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

        public IFormFile? Image { get; set; }
    }
}
