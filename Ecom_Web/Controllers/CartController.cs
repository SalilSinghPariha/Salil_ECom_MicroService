using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using Ecom_Web.Utility;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Ecom_Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;

        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View( await LoadCartDtoBasedOnLoggedInUser());
        }

        public async Task<IActionResult> OrderConfirmation(int OrderId)
        {
            var response = await _orderService.ValidateStripeSession(OrderId);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>
                    (Convert.ToString(response.Result));
                if (orderHeaderDto.Status == SD.Status_Approved)
                {
                    return View(OrderId);
                }
                
            }
            // redirect to some other page based on error
            return View(OrderId);
        }


        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            // get latest cart 
           CartDTO cart = await LoadCartDtoBasedOnLoggedInUser();

            //populate email and phone, number 

            cart.cartHeaderDTO.Email = cartDTO.cartHeaderDTO.Email;
            cart.cartHeaderDTO.Phone= cartDTO.cartHeaderDTO.Phone;
            cart.cartHeaderDTO.Name = cartDTO.cartHeaderDTO.Name;

            //create order through order service via order API
            var respone = await _orderService.CreateOrder(cart);

            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>
                (Convert.ToString(respone.Result));

            
            if (respone != null && respone.IsSuccess)
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                StripeRequestDTO stripeRequestDTO = new()
                {
                    ApprovedUrl = domain + "cart/OrderConfirmation?orderId=" + orderHeaderDto.OrderHeaderId,
                    CancelUrl=domain +"cart/checkout",
                    OrderHeaderDto = orderHeaderDto
                };
                
                //make stripe call
                var stripeRespone= await _orderService.CreateStripeSession(stripeRequestDTO);
                StripeRequestDTO stripeRequest = JsonConvert.DeserializeObject<StripeRequestDTO>
                    (Convert.ToString(stripeRespone.Result));

                // add session url in response header
                Response.Headers.Add("Location",stripeRequest.StripeSeesionUrl);
                return new StatusCodeResult(303);
            }

            return View();

        }
        private async Task<CartDTO> LoadCartDtoBasedOnLoggedInUser()
        {
            //Id of logged in user
            var userId = User.Claims.Where(u=>u.Type==
            JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            //call cart service
            ResponseDto? response = await _cartService.GetCartbyUserIdAsync(userId);

            
            //if reposne is sucess
            if (response!=null&&  response.IsSuccess)
            {
                //deserialized and return cartdto
               CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                return cartDTO;
                
            }
            // if not success then return empty cartdto
            return new CartDTO();

        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId= User.Claims.Where(U=>U.Type==JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            ResponseDto? response = await _cartService.RemoveCartAsync(cartDetailsId);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Cart Updated Succesfully";
                return RedirectToAction(nameof(CartIndex));
            }
            TempData["Error"]= response.Message.ToString();

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupan(CartDTO cartDTO)
        {
            ResponseDto? response = await _cartService.ApplyCoupan(cartDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Coupan Applied Succesfully";
                return RedirectToAction(nameof(CartIndex));
            }
            TempData["Error"] = response.Message.ToString();

            return View();

        }


        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cartDTO)
        {
            CartDTO cart =  await LoadCartDtoBasedOnLoggedInUser();
            cart.cartHeaderDTO.Email = User.Claims.Where(u=>u.Type== 
            JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
          
            ResponseDto? response = await _cartService.EmailCart(cart);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Mesage sent to queue for publish and emial will send shortly";
                return RedirectToAction(nameof(CartIndex));
            }
            TempData["Error"] = response.Message.ToString();

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupan(CartDTO cartDTO)
        {
            cartDTO.cartHeaderDTO.CoupanCode = "";
            ResponseDto? response = await _cartService.ApplyCoupan(cartDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Coupan Removed Succesfully";
                return RedirectToAction(nameof(CartIndex));
            }
            TempData["Error"] = response.Message.ToString();

            return View();

        }
    }
}
