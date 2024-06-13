using AutoMapper;
using Ecom.MessageBus;
using Ecom_ShoppingCartAPI.Data;
using Ecom_ShoppingCartAPI.Model;
using Ecom_ShoppingCartAPI.Model.DTO;
using Ecom_ShoppingCartAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Ecom_ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ResponseDto _responseDto;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICoupanService _coupanService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;


        public CartController(ApplicationDbContext context, IMapper mapper, 
            IProductService productService, ICoupanService coupanService,
            IMessageBus messageBus,IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _responseDto = new ResponseDto();
            _productService = productService;
            _coupanService = coupanService;
            _messageBus = messageBus;
            _configuration = configuration;
        }
        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDTO cartDTO)
        {
            try 
            {
                // check and get card header based on user id from db
                //AsNoTracking() : if we won't add this and we are trying to update existing cart with cart detials
                //and header then it will give same key already applied in call and api call will get failed so now
                //if add this here then it will allow to update esxiting cat item 

                var cardHeaderFromDb = await _context.cartHeaders.AsNoTracking().
                    FirstOrDefaultAsync(u => u.UserId == cartDTO.cartHeaderDTO.UserId);

                if (cardHeaderFromDb == null)
                {
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.cartHeaderDTO);

                     _context.cartHeaders.Add(cartHeader);
                    await _context.SaveChangesAsync();
                    cartDTO.cartDetailDTOs.First().CartHeaderId = cartHeader.CartHeaderId;
                 _context.cartDetails.Add(_mapper.Map<CartDetails>(cartDTO.cartDetailDTOs.First()));
                    await _context.SaveChangesAsync();
                }

                else
                {
                    // get the cart details
                    //check if detials of product is same

                    var cartDetailsFromDb = await _context.cartDetails.AsNoTracking().
                        FirstOrDefaultAsync(u =>
                    u.ProductId == cartDTO.cartDetailDTOs.First().ProductId &&
                    u.CartHeaderId == cardHeaderFromDb.CartHeaderId);

                    if (cartDetailsFromDb == null)
                    {
                        //create Cart details
                        cartDTO.cartDetailDTOs.First().CartHeaderId = cardHeaderFromDb.CartHeaderId;
                        _context.cartDetails.Add(_mapper.Map<CartDetails>(cartDTO.cartDetailDTOs.First()));
                        await _context.SaveChangesAsync();

                    }

                    else
                    {
                        //update count to cart details since it's for same product and thatr logic
                        cartDTO.cartDetailDTOs.First().count += cartDetailsFromDb.count;
                        cartDTO.cartDetailDTOs.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDTO.cartDetailDTOs.First().CartDetailID = cartDetailsFromDb.CartDetailID;

                        // since it's update call so update call instead of Add
                        _context.cartDetails.Update(_mapper.Map<CartDetails>(cartDTO.cartDetailDTOs.First()));
                        await _context.SaveChangesAsync();

                    }
                }

              _responseDto.Result = cartDTO;
               
            }
            catch (Exception ex)
            { 
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
            
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                // Get Cart Details
                var cartDetails = _context.cartDetails.First(u=>u.CartDetailID==cartDetailsId);

                // get count
                int totalCartItems = _context.cartDetails.Where(u =>
                u.CartHeaderId == cartDetails.CartHeaderId).Count();

                // remove cart details
                _context.cartDetails.Remove(cartDetails);

                // if total cart item is 1 then remove it from cart header

                if(totalCartItems == 1)
                {
                    //get cart header
                    var cartHeaderRemove = _context.cartHeaders.FirstOrDefault(u =>
                    u.CartHeaderId == cartDetails.CartHeaderId);

                    // remove header as well if count is 1 which measn last in cart

                    _context.cartHeaders.Remove(cartHeaderRemove);

                }

               await _context.SaveChangesAsync();
                _responseDto.Result = true;


            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;

        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try 
            {
                CartDTO cart = new()
                {
                    cartHeaderDTO = _mapper.Map<CartHeaderDTO>(_context.cartHeaders.First(u => 
                    u.UserId == userId))
                };

                cart.cartDetailDTOs = _mapper.Map<IEnumerable<CartDetailDTO>>(_context.cartDetails.Where(u=>
                u.CartHeaderId==cart.cartHeaderDTO.CartHeaderId));

                IEnumerable<ProductDTO> products = await _productService.GetProducts();
                foreach (var item in cart.cartDetailDTOs)
                {
                    item.ProductDTO= products.First(u=>u.ProductId==item.ProductId);
                    cart.cartHeaderDTO.CartTotal += (item.count * item.ProductDTO.Price);
                }
                //apply coupan if any
                if (!string.IsNullOrEmpty(cart.cartHeaderDTO.CoupanCode))
                {
                    //retrieve coupan from coupanAPI
                    CoupanDto coupan = await _coupanService.GetCoupan(cart.cartHeaderDTO.CoupanCode);
                    //now will check if coupan is not null and then check if cartotal is greater than min amount
                    //then only apply coupan and return back discount and revised total

                    if (coupan != null && cart.cartHeaderDTO.CartTotal > coupan.MinAmount)
                    {
                        cart.cartHeaderDTO.Discount = coupan.DiscountAmount;
                        cart.cartHeaderDTO.CartTotal-= coupan.DiscountAmount;
                    }
                }

                _responseDto.Result = cart;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess= false;
                _responseDto.Message= ex.Message.ToString();

            }

            return _responseDto;
        }

        [HttpPost("ApplyCoupan")]
        public async Task<ResponseDto> ApplyCoupan([FromBody] CartDTO cartDTO)
        {
            try 
            {
                //cart from db 
                var cartFromDb=  await  _context.cartHeaders.FirstAsync(u=>u.UserId==cartDTO.cartHeaderDTO.UserId);
                //then assign coupan code which we applied
                cartFromDb.CoupanCode=cartDTO.cartHeaderDTO.CoupanCode;
                //save this to cart
                _context.cartHeaders.Update(cartFromDb);    
                await _context.SaveChangesAsync();
                _responseDto.IsSuccess = true;
                
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess=false;
                _responseDto.Message= ex.Message.ToString();    
            }

            return _responseDto;
        }

        [HttpPost("RemoveCoupan")]
        public async Task<ResponseDto> RemoveCoupan([FromBody] CartDTO cartDTO)
        {
            try
            {
                //cart from db 
                var cartFromDb = await _context.cartHeaders.FirstAsync(u => u.UserId == cartDTO.cartHeaderDTO.UserId);
                //then assign coupan code as empty since we are removing coupan
                cartFromDb.CoupanCode = "";
                //save this to cart
                _context.cartHeaders.Update(cartFromDb);
                await _context.SaveChangesAsync();
                _responseDto.IsSuccess = true;

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }

            return _responseDto;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<ResponseDto> EmailCartRequest([FromBody] CartDTO cartDTO)
        {
            try
            {
                await _messageBus.PublishMessage(cartDTO, 
                    _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCart"));
                _responseDto.IsSuccess = true;

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }

            return _responseDto;
        }
    }
}
