namespace UIStoreMVC.Models.Checkout
{
    // FORM POST İÇİN KULLANILAN MODEL
    public class CheckoutFormModel
    {
        public int? SelectedAddressId { get; set; }

        // 💳 Basit ödeme alanları
        public string? CardHolderName { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpiryMonth { get; set; }
        public string? ExpiryYear { get; set; }
        public string? Cvv { get; set; }
    }
}
