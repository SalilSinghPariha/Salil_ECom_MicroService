namespace Ecom.Service.ProductAPI.Model.DTO
{
    public class ResponseDto
    {
        public object? Result { get; set; }

        public bool IsSuccess { get; set; }=true;

        public String Message { get; set; } = "";
    }
}
