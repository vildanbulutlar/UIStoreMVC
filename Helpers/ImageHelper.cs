namespace UIStoreMVC.Helpers
{
    public static class ImageHelper
    {
        // Tek kurallı resolver (senin klasör yapına uygun)
        public static string Resolve(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return "/images/no-image.jpg";

            imageUrl = imageUrl.Trim();

            // Dış URL ise direkt dön
            if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
                return imageUrl;

            // Zaten / ile başlıyorsa (örn: /images/products/a.jpg)
            if (imageUrl.StartsWith("/"))
                return imageUrl;

            // Eğer sadece dosya adı geldiyse: products içine koy
            return $"/images/products/{imageUrl}";
        }
    }
}
