using AutoMapper;
using Ecom.MessageBus;
using Ecom.Service.OrderAPI.Data;
using Ecom.Service.OrderAPI.Model;
using Ecom.Service.OrderAPI.Model.DTO;
using Ecom.Service.OrderAPI.Service;
using Ecom.Service.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Ecom.Service.OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private ResponseDto  _response;
        private IMapper _mapper;
        private readonly ApplicationDbContext _db;
        private IProductServicecs _productService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public OrderController(ApplicationDbContext db,
            IProductServicecs productService, IMapper mapper, IConfiguration configuration
            , IMessageBus messageBus)
        {
            _db = db;
            _messageBus = messageBus;
            this._response = new ResponseDto();
            _productService = productService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDTO cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.cartHeaderDTO);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.cartDetailDTOs);
                orderHeaderDto.OrderTotal = Math.Round(orderHeaderDto.OrderTotal, 2);
                OrderHeader orderHeader = _mapper.Map<OrderHeader>(orderHeaderDto);
                //OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                OrderHeader orderHeader1 = _db.OrderHeaders.Add(orderHeader).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderHeader1.OrderHeaderId;
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };

                // to fetch coupan 
                var DiscountObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDTO.OrderHeaderDto.CoupanCode
                    }
                    
                    };

                // creating sesion line item based on order details
                foreach (var item in stripeRequestDTO.OrderHeaderDto.OrderDetails)
                {
                    var SessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount=(long)(item.Price*100),
                            Currency="usd",
                            ProductData= new SessionLineItemPriceDataProductDataOptions
                            {
                                Name= item.ProductDTO.Name,
                            }
                        },

                        Quantity=item.Count
                    };
                    // add session line item for all the product we have inside order detials
                    options.LineItems.Add(SessionLineItem);
                }
                //add coupan
                if (stripeRequestDTO.OrderHeaderDto.Discount > 0)
                {
                    options.Discounts = DiscountObj;
                }

                 //service
                var service = new Stripe.Checkout.SessionService();
                // create service with option and store it in stripe session which will give option
               Session session= service.Create(options);
                //get session url and id and other property and assign it to striperequestDTO
                stripeRequestDTO.StripeSeesionUrl = session.Url; //this will give where to redirect checkout session

                // we can store session id in DB so that later we can investigate later based on this
                OrderHeader orderHeader= _db.OrderHeaders.First(u=>
                u.OrderHeaderId==stripeRequestDTO.OrderHeaderDto.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                await _db.SaveChangesAsync();
                _response.Result = stripeRequestDTO;
                _response.IsSuccess = true;

            }
            catch(Exception ex) 
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                // get the order so that we can get the session
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

                // create session and verify the ID
                var service = new SessionService();

                Session session = service.Get(orderHeader.StripeSessionId);

                // payment intent service and get the payment status based on paymentintentID
                var paymentIntentService= new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                // check the payment status
                if (paymentIntent.Status == "succeeded")
                {
                    // order payment is completed
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status= SD.Status_Approved;
                    await _db.SaveChangesAsync();

                }

                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

    }
}
