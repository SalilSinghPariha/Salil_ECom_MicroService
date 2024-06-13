namespace Ecom.Service.EmailAPI.Model
{
    public class CartDTO
    {
        public CartHeaderDTO? cartHeaderDTO { get; set; }

        //it will have one cart header with multiple DTO
        public IEnumerable<CartDetailDTO>? cartDetailDTOs { get; set; }  
    }
}
