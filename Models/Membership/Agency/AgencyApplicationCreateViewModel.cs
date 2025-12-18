using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UIStoreMVC.Models.Membership.Agency
{
    public class AgencyApplicationCreateViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ContactPerson { get; set; }

        public IFormFile? TaxDocument { get; set; }
        public IFormFile? SignatureCircular { get; set; }
        public IFormFile? TradeRegistry { get; set; }
    }
}
