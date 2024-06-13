namespace Ecom.Service.OrderAPI.Model.DTO
{
    public class CartDetailDTO
    {

        public int CartDetailId { get; set; }
        public int CartHeaderId { get; set; }
        public CartHeaderDTO? CartHeader { get; set; }
        public int ProductId { get; set; }
        public ProductDTO? ProductDTO { get; set; }
        public int Count { get; set; }
    }
}
