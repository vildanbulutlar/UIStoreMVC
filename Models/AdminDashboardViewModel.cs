using System;
using System.Collections.Generic;

namespace UIStoreMVC.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        // Üst istatistik kartları
        public int TotalProduct { get; set; }
        public int TotalCategory { get; set; }
        public int TotalOrder { get; set; }
        public int TotalUser { get; set; }

        public decimal TotalProductChangeRate { get; set; }  // %12 gibi
        public decimal TotalCategoryChangeRate { get; set; }
        public decimal TotalOrderChangeRate { get; set; }
        public decimal TotalUserChangeRate { get; set; }

        // Grafikler
        public List<DailyOrderChartItem> Last7DaysOrders { get; set; } = new();
        public List<OrderStatusChartItem> OrderStatusDistribution { get; set; } = new();

        // En çok satanlar
        public List<TopProductItem> TopProducts { get; set; } = new();

        // Son siparişler
        public List<RecentOrderItem> RecentOrders { get; set; } = new();

        // Bekleyen işler widget
        public int PendingOrderCount { get; set; }
        public int LowStockProductCount { get; set; }
        public int NewCustomerCountToday { get; set; }
        public int RefundRequestCount { get; set; }

        // Aktivite timeline
        public List<ActivityItem> LatestActivities { get; set; } = new();
    }

    public class DailyOrderChartItem
    {
        public string DayLabel { get; set; } = string.Empty; // "04.12" gibi
        public int OrderCount { get; set; }
    }

    public class OrderStatusChartItem
    {
        public string StatusName { get; set; } = string.Empty; // "TeslimEdildi"
        public int Count { get; set; }
    }

    public class TopProductItem
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int PercentOfTotal { get; set; } // progress bar için
    }

    public class RecentOrderItem
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty; // "TeslimEdildi"
    }

    public class ActivityItem
    {
        public DateTime Time { get; set; }
        public string Description { get; set; } = string.Empty; // "Yeni kullanıcı kaydoldu" gibi
    }
}
