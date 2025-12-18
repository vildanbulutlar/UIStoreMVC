using Application.DTOs.AdressDTOs;
using Application.DTOs.UserDTOs;
using Application.DTOs.OrderDTOs;
using System.Collections.Generic;

namespace UIStoreMVC.Models.Checkout
{
    // EKRANI ÇİZMEK İÇİN KULLANILAN MODEL
    public class CheckoutViewModel
    {
        public UserDto User { get; set; } = null!;
        public List<AddressDto> Addresses { get; set; } = new();

        public int? SelectedAddressId { get; set; }

        public ShoppingCartDto Cart { get; set; } = null!;

        // 💳 Ekranda göstermek için (zorunlu değil, sadece binding kolay olsun)
        public string? CardHolderName { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpiryMonth { get; set; }
        public string? ExpiryYear { get; set; }
        public string? Cvv { get; set; }
    }
}
