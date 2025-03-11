using InsureProSalesDataAssesment.Services;

class Program
{
    static void Main(string[] args)
    {
        // 1. Analyze Sales Data 
        var salesService = new SalesAnalysisService();
        decimal totalSales = salesService.GetTotalSales();
        var monthWiseSales = salesService.GetMonthWiseSales();
        var popularItems = salesService.GetMostPopularItems();
        var topRevenueItems = salesService.GetTopRevenueItems();
        var minMaxAvgOrders = salesService.GetMinMaxAvgOrdersForPopularItem();
       
        Console.Write("Data loaded successfully!\n");

        // 2. generate Reports
        salesService.PrintReport(totalSales, monthWiseSales, popularItems, topRevenueItems, minMaxAvgOrders);
    }
}
