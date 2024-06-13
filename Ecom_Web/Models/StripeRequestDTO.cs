namespace Ecom_Web.Models
{
    public class StripeRequestDTO
    {
        public string? StripeSessionId { get; set; }
        public string? StripeSeesionUrl { get; set; }
        public string CancelUrl { get; set; }
        public string ApprovedUrl { get; set; }
        public OrderHeaderDto OrderHeaderDto { get; set;}
    }

    }
