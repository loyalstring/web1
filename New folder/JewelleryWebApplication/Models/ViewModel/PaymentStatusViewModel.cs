namespace JewelleryWebApplication.Models.ViewModel
{
    public class PaymentStatusViewModel
    {
    
        public string merchantTransactionId { get; set; }
      
        public int amount { get; set; }
        public string callbackUrl { get; set; }
        public string mobileNumber { get; set; }
    }
}
