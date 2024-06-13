using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Ecom_Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _ProductService;
        public ProductController(IProductService productService)
        {

           _ProductService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDTO> ProductDtos = new();
            ResponseDto? responseDto = await _ProductService.GetAllProductAsync();
            if (responseDto != null && responseDto.IsSuccess)
            {
                ProductDtos = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(responseDto.Result));
            }
            else 
            {
                TempData["error"] = responseDto?.Message;
            }

            return View(ProductDtos);
        }

        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDTO ProductDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? responseDto = await _ProductService.CreateProductAsync(ProductDto);
                if (responseDto != null && responseDto.IsSuccess)
                {
                    TempData["Success"] = responseDto?.Message;
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = responseDto?.Message;
                }
            }
            return View(ProductDto);
        }

        public async Task<IActionResult> ProductDelete(int ProductId)
        {
          
            ResponseDto? responseDto = await _ProductService.GetProductByIdAsync(ProductId);
            if (responseDto != null && responseDto.IsSuccess)
            {
                ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(responseDto.Result));
                
                return View(model);
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(ProductIndex);
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDTO ProductDto)
        {

            ResponseDto? responseDto = await _ProductService.DeleteProductAsync(ProductDto.ProductId);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["Success"] = responseDto?.Message;
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(ProductDto);
        }

		public async Task<IActionResult> ProductEdit(int ProductId)
		{

			ResponseDto? responseDto = await _ProductService.GetProductByIdAsync(ProductId);
			if (responseDto != null && responseDto.IsSuccess)
			{
				ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(responseDto.Result));

				return View(model);
			}
			else
			{
				TempData["error"] = responseDto?.Message;
			}
			return View(ProductIndex);
		}
		[HttpPost]
		public async Task<IActionResult> ProductEdit(ProductDTO ProductDto)
		{

			ResponseDto? responseDto = await _ProductService.UpdateProductAsync(ProductDto);
			if (responseDto != null && responseDto.IsSuccess)
			{
				TempData["Success"] = responseDto?.Message;
				return RedirectToAction(nameof(ProductIndex));
			}
			else
			{
				TempData["error"] = responseDto?.Message;
			}
			return View(ProductDto);
		}
	}
}
