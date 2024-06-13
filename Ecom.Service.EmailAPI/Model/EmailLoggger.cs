namespace Ecom.Service.EmailAPI.Model
{
    public class EmailLoggger
    {
        public int id { get; set; }
        public string Email { get; set;}
        public string message { get; set;}
        public DateTime? EmailSent { get; set;}
    }
}
