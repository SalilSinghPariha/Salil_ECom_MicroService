using AutoMapper;
using Ecom.Service.ProductAPI.Data;
using Ecom.Service.ProductAPI.Model;
using Ecom.Service.ProductAPI.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Service.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // this is authrize attribute if we add this and we won't implement correctly then it will give internal server error if we access befofe or after loginvi
    //[Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ResponseDto _responseDto;
        private readonly IMapper _mapper;

        
        public ProductController(ApplicationDbContext context, IMapper mapper)
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
                var objList = _context.Products.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<ProductDTO>>(objList);
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
                Product obj = _context.Products.FirstOrDefault(u => u.ProductId == id);
                _responseDto.Result = _mapper.Map<ProductDTO>(obj);
            }
            catch(Exception ex)
            {
                _responseDto.Result = ex;
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("GetByProductCode/{code}")]
        public ResponseDto GetByProductCode(string code)
        {
            try
            {
                Product obj = _context.Products.First(u => u.Name.ToLower() == code.ToLower());
                _responseDto.Result = _mapper.Map<ProductDTO>(obj);
            }
            catch (Exception ex)
            {
                _responseDto.Result = ex;
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpPost("AddProduct")]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto AddProduct([FromForm] ProductDTO productDTO)
        {
            try
            {
                var product = _mapper.Map<Product>(productDTO);
                _context.Products.Add(product);
                _context.SaveChanges();
                if (productDTO.Image != null)
                {
                    // get fileName
                    string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);

                    //get filePath
                    string filePath = @"wwwroot\ProductImage\" + fileName;

                    //get filePathDirectory
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        // copy image to root
                        productDTO.Image.CopyTo(fileStream);
                    }
                    // get the base url which will attach with product url

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    product.ImageLocalUrl = filePath;
                }

                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }

                _context.Products.Update(product);
                _context.SaveChanges();
                _responseDto.Result = _mapper.Map<ProductDTO>(product);
                _responseDto.Message = "Product created successfully";

            }
            catch (Exception ex)
            {
                _responseDto.Result = ex;
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpPut]
        [Route("UpdateProduct")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto UpdateProduct([FromForm] ProductDTO productDTO)
        {
            try
            {
                var product = _mapper.Map<Product>(productDTO);

                if (productDTO.Image != null)
                {
                    //remove old file from difrectory
                    // if we are having image then get the directoty and rmeve it then remove it from product table
                    if (!string.IsNullOrEmpty(product.ImageLocalUrl))
                    {
                        var oldDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalUrl);

                        FileInfo fileInfo = new FileInfo(oldDirectoryPath);
                        if (fileInfo.Exists)
                        {
                            fileInfo.Delete();
                        }
                    }
                    //now create new file
                    // get fileName
                    string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);

                    //get filePath
                    string filePath = @"wwwroot\ProductImage\" + fileName;

                    //get filePathDirectory
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        // copy image to root
                        productDTO.Image.CopyTo(fileStream);
                    }
                    // get the base url which will attach with product url

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    product.ImageLocalUrl = filePath;
                }

                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }
                _context.Products.Update(product);
                _context.SaveChanges();
                _responseDto.Result = _mapper.Map<ProductDTO>(product);
				_responseDto.Message = "Provided Product Updated successfully";

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
        public ResponseDto DeleteProduct(int id)
        {
            try
            {
                var product = _context.Products.First(u=>u.ProductId==id);

                // if we are having image then get the directoty and rmeve it then remove it from product table
                if (!string.IsNullOrEmpty(product.ImageLocalUrl))
                {
                    var oldDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalUrl);

                    FileInfo fileInfo = new FileInfo(oldDirectoryPath);
                    if(fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }
                _context.Products.Remove(product);
                _context.SaveChanges();
                _responseDto.Message = "Provided Product deleted successfully";

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
