namespace Ecom_Web.Utility
{
    public class SD
    {
        public static string CoupanApiBase { get; set; }
        public static string AuthApiBase { get; set; }
        public static string ProductApiBase { get; set; }
        public static string CartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "jwtToken";
        public enum ApiType
        {

            PUT ,
            DELETE,
            GET,
            POST

        }

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadyForPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Refunded = "Refunded";
        public const string Status_Cancelled = "Cancelled";

        public enum ContentType
        {
            Json,
            MultipartFormData,
        }
    }
}
