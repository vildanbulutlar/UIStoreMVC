using Application.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using UIStoreMVC.Areas.Admin.Models;
using Domain.Enums;
using Application.DTOs.OrderDTOs;   // 🔹 OrderItemDto için

namespace UIStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IServiceUnit _services;

        public HomeController(IServiceUnit services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 📌 SERVİSLERDEN VERİLERİ ÇEK
            var products = await _services.ProductService.GetAllAsync();
            var categories = await _services.CategoryService.GetAllAsync();
            var orders = await _services.OrderService.GetAllAsync();

            // 📌 MÜŞTERİ SAYISI
            int totalCustomers = 0;
            try
            {
                var users = await _services.CustomerService.GetAllUsersAsync();
                totalCustomers = users.Count;
            }
            catch
            {
                totalCustomers = 0;
            }

            // 📌 SON 7 GÜN SİPARİŞ GRAFİĞİ
            var today = DateTime.Today;
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-i))
                .OrderBy(d => d)
                .ToList();

            var last7DaysOrders = last7Days
                .Select(d => new DailyOrderChartItem
                {
                    DayLabel = d.ToString("dd.MM"),
                    OrderCount = orders.Count(o => o.OrderDate.Date == d.Date)
                })
                .ToList();

            // 📌 DURUM DAĞILIMI
            var statusDistribution = orders
                .GroupBy(o => o.Status)
                .Select(g => new OrderStatusChartItem
                {
                    StatusName = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            // 📌 SON 5 SİPARİŞ
            var recentOrders = orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new RecentOrderItem
                {
                    OrderId = o.Id,
                    CustomerName = o.CustomerFullName ?? "Bilinmiyor",
                    CreatedAt = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status.ToString()
                })
                .ToList();

            // 📌 EN ÇOK SATAN ÜRÜNLER (Top 5)
            // OrderDto içindeki Items listesine bakıyoruz.
            var allItems = orders
                .Where(o => o.Items != null)
                .SelectMany(o => o.Items!);  // OrderItemDto

            var groupedItems = allItems
                .GroupBy(i => new { i.ProductId, i.ProductName })
                .Select(g => new
                {
                    g.Key.ProductId,
                    Name = g.Key.ProductName,
                    Quantity = g.Sum(x => x.Quantity),
                    TotalAmount = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToList();

            int totalSoldQty = groupedItems.Sum(x => x.Quantity);

            var topProducts = groupedItems
                .Select(x => new TopProductItem
                {
                    Name = x.Name,
                    Quantity = x.Quantity,
                    TotalAmount = x.TotalAmount,
                    PercentOfTotal = totalSoldQty == 0
                        ? 0
                        : (int)Math.Round((decimal)x.Quantity * 100m / totalSoldQty)
                })
                .ToList();

            // 📌 BEKLEYEN İŞLER & DÜŞÜK STOK
            int pendingCount = orders.Count(o => o.Status == OrderStatus.Hazirlaniyor);
            int cancelledCount = orders.Count(o => o.Status == OrderStatus.IptalEdildi);

            // Örneğin stok < 10 ise "az stok" diyelim
            int lowStockCount = products.Count(p => p.Stock < 10);

            var model = new AdminDashboardViewModel
            {
                // Üst kartlar
                TotalProduct = products.Count(),
                TotalCategory = categories.Count(),
                TotalOrder = orders.Count(),
                TotalUser = totalCustomers,

                TotalProductChangeRate = 0,
                TotalCategoryChangeRate = 0,
                TotalOrderChangeRate = 0,
                TotalUserChangeRate = 0,

                // Grafikler
                Last7DaysOrders = last7DaysOrders,
                OrderStatusDistribution = statusDistribution,

                // En çok satanlar
                TopProducts = topProducts,

                // Son siparişler
                RecentOrders = recentOrders,

                // Bekleyen işler
                PendingOrderCount = pendingCount,
                LowStockProductCount = lowStockCount,
                NewCustomerCountToday = 0,
                RefundRequestCount = cancelledCount,

                // Aktivite (şimdilik boş)
                LatestActivities = new List<ActivityItem>()
            };

            return View(model);
        }
    }
}
