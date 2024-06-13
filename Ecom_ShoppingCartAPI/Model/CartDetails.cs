using Ecom_ShoppingCartAPI.Model.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecom_ShoppingCartAPI.Model
{
    public class CartDetails
    {
        [Key]
        public int CartDetailID { get; set; }

        public int CartHeaderId  { get; set; }
        [ForeignKey("CartHeaderId")]
        public CartHeader CartHeader { get; set; }

        public int ProductId { get; set; }
        [NotMapped]
        public ProductDTO ProductSTO { get; set; }

        public int count { get; set; }  
    }
}
