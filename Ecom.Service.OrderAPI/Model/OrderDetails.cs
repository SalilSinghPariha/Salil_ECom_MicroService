using Ecom.Service.OrderAPI.Model.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecom.Service.OrderAPI.Model
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader? OrderHeader { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDTO? ProductDTO { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}
