using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Ecom_Web.Controllers
{
    public class HomeController : Controller
    {
		private readonly IProductService _ProductService;
        private readonly ICartService _cartService;
        public HomeController(IProductService productService, ICartService cartService)
		{

			_ProductService = productService;
            _cartService = cartService;
		}
		public async Task<IActionResult> Index()
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

        [Authorize]
        public async Task<IActionResult> Detail(int productID)
        {
            ProductDTO ProductDtos = new();
            ResponseDto? responseDto = await _ProductService.GetProductByIdAsync(productID);
            if (responseDto != null && responseDto.IsSuccess)
            {
                ProductDtos = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(responseDto.Result));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }

            return View(ProductDtos);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Detail")]
        public async Task<IActionResult> Detail(ProductDTO productDTO)
        {
            CartDTO cartDto = new CartDTO()
            {
                cartHeaderDTO = new CartHeaderDTO
                {
                    UserId = User.Claims.Where(u =>
                    u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value

                }

            };

            CartDetailDTO cartDetailDTO = new CartDetailDTO()
            {
                count = productDTO.count,
                ProductId=productDTO.ProductId
            };

            List<CartDetailDTO> cartDetailDTOes = new()
            {
                cartDetailDTO
            };

            cartDto.cartDetailDTOs = cartDetailDTOes;

            ResponseDto? responseDto = await _cartService.UpsertCartAsync(cartDto);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["Success"] = "Item has been added to Cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }

            return View(productDTO);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}