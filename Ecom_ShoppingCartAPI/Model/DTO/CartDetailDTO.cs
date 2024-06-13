namespace Ecom_ShoppingCartAPI.Model.DTO
{
    public class CartDetailDTO
    {

        public int CartDetailID { get; set; }
        public int CartHeaderId { get; set; }
        public CartHeaderDTO? CartHeader { get; set; }
        public int ProductId { get; set; }
        public ProductDTO? ProductDTO { get; set; }
        public int count { get; set; }
    }
}
