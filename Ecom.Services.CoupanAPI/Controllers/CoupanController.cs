using AutoMapper;
using Ecom.Services.CoupanAPI.Data;
using Ecom.Services.CoupanAPI.Models;
using Ecom.Services.CoupanAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Services.CoupanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // this is authrize attribute if we add this and we won't implement correctly then it will give internal server error if we access befofe or after loginvi
    [Authorize]
    public class CoupanController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ResponseDto _responseDto;
        private readonly IMapper _mapper;

        
        public CoupanController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try 
            {
                var objList = _context.coupans.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<CoupanDto>>(objList);
            }
            catch(Exception ex)
            { 
                _responseDto.Message= ex.Message;
                _responseDto.IsSuccess = false;

            }
            return _responseDto;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupan obj = _context.coupans.FirstOrDefault(u => u.CoupanId == id);
                _responseDto.Result = _mapper.Map<CoupanDto>(obj);
            }
            catch(Exception ex)
            {
                _responseDto.Result = ex;
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("GetByCoupanCode/{code}")]
        public ResponseDto GetByCoupanCode(string code)
        {
            try
            {
                Coupan obj = _context.coupans.First(u => u.CoupanCode.ToLower() == code.ToLower());
                _responseDto.Result = _mapper.Map<CoupanDto>(obj);
            }
            catch (Exception ex)
            {
                _responseDto.Result = ex;
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpPost]
        [Route("AddCoupan")]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto AddCoupan([FromBody] CoupanDto coupanDto)
        {
            try
            {
                var coupan=_mapper.Map<Coupan>(coupanDto);
                _context.coupans.Add(coupan);
                _context.SaveChanges();

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff=(long)(coupanDto.DiscountAmount*100),
                    Name=coupanDto.CoupanCode,
                    Currency="usd",
                    Id=coupanDto.CoupanCode
                };
                //since stripe also have same coupan so we instead of using we
                //can direclty use like below since same coupan we ar ehaving it here in cpoupan api 
                var service = new Stripe.CouponService();
                service.Create(options);

                _responseDto.Result = _mapper.Map<CoupanDto>(coupan);
                _responseDto.Message = "Coupan created successfully";

            }
            catch (Exception ex)
            {
                _responseDto.Result = ex;
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpPut]
        [Route("UpdateCoupan")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto UpdateCoupan([FromBody] CoupanDto coupanDto)
        {
            try
            {
                var coupan = _mapper.Map<Coupan>(coupanDto);
                _context.coupans.Update(coupan);
                _context.SaveChanges();
                _responseDto.Result = _mapper.Map<CoupanDto>(coupan);

            }
            catch (Exception ex)
            {
                _responseDto.Result = ex;
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto DeleteCoupan(int id)
        {
            try
            {
                var coupan = _context.coupans.First(u=>u.CoupanId==id);
                _context.coupans.Remove(coupan);
                _context.SaveChanges();
                //remove coupan service
                var service = new Stripe.CouponService();
                service.Delete(coupan.CoupanCode);

                _responseDto.Message = "Provided Coupan deleted successfully";

            }
            catch (Exception ex)
            {
                _responseDto.Result = ex;
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

    }
}
