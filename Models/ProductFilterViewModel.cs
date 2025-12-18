using System.Collections.Generic;
using Application.DTOs.ProductDTOs;
using Application.DTOs.CategoryDTOs;

namespace UIStoreMVC.Models
{
    public class ProductFilterViewModel
    {
        // Ürün listesi
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();

        // (İstersen) dropdown için kategoriler
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        // Filtre alanları
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // DİKKAT: Controller SearchTerm kullanıyor, adı buna göre
        public string? SearchTerm { get; set; }

        public string? SortBy { get; set; }

        // Chip filtre (indirimdekiler, çok satanlar vs.)
        public string? ChipFilter { get; set; }

        // Checkbox'lar
        public bool InStockOnly { get; set; }
        public bool OnlyDiscounted { get; set; }
    }
}
