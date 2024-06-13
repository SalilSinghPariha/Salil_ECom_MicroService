namespace Ecom_ShoppingCartAPI.Model.DTO
{
    public class ResponseDto
    {
        public object? Result { get; set; }

        public bool IsSuccess { get; set; }=true;

        public String Message { get; set; } = "";
    }
}
