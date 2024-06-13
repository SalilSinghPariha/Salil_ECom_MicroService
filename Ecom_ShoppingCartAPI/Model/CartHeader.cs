using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecom_ShoppingCartAPI.Model
{
    public class CartHeader
    {
        [Key]
        public int CartHeaderId { get; set; }

        public string? UserId { get; set; }

        public string? CoupanCode { get; set;}

        //just for display only not for DB so we can use Not Mapped property 
        [NotMapped]
        public double Discount { get; set;}
        [NotMapped]
        public double CartTotal { get; set; }

    }
}
