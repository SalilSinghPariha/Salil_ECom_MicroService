using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Ecom_Web.Controllers
{
    public class CoupanController : Controller
    {
        private readonly ICoupanService _coupanService;
        public CoupanController(ICoupanService coupanService)
        {

           _coupanService = coupanService;
        }
        public async Task<IActionResult> CoupanIndex()
        {
            List<CoupanDto> coupanDtos = new();
            ResponseDto? responseDto = await _coupanService.GetAllCoupanAsync();
            if (responseDto != null && responseDto.IsSuccess)
            {
                coupanDtos = JsonConvert.DeserializeObject<List<CoupanDto>>(Convert.ToString(responseDto.Result));
            }
            else 
            {
                TempData["error"] = responseDto?.Message;
            }

            return View(coupanDtos);
        }

        public async Task<IActionResult> CreateCoupan()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupan(CoupanDto coupanDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? responseDto = await _coupanService.CreateCoupanAsync(coupanDto);
                if (responseDto != null && responseDto.IsSuccess)
                {
                    TempData["Success"] = responseDto?.Message;
                    return RedirectToAction(nameof(CoupanIndex));
                }
                else
                {
                    TempData["error"] = responseDto?.Message;
                }
            }
            return View(coupanDto);
        }

        public async Task<IActionResult> CoupanDelete(int CoupanId)
        {
          
            ResponseDto? responseDto = await _coupanService.GetCoupanByIdAsync(CoupanId);
            if (responseDto != null && responseDto.IsSuccess)
            {
                CoupanDto? model = JsonConvert.DeserializeObject<CoupanDto>(Convert.ToString(responseDto.Result));
                
                return View(model);
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(CoupanIndex);
        }

        [HttpPost]
        public async Task<IActionResult> CoupanDelete(CoupanDto coupanDto)
        {

            ResponseDto? responseDto = await _coupanService.DeleteCoupanAsync(coupanDto.CoupanId);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["Success"] = responseDto?.Message;
                return RedirectToAction(nameof(CoupanIndex));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(coupanDto);
        }
    }
}
