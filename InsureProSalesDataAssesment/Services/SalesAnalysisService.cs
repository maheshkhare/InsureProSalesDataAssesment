using InsureProSalesDataAssesment.Models;

namespace InsureProSalesDataAssesment.Services
{
    public class SalesAnalysisService
    {
        private List<SalesModel> _salesData;

        // Constructor loads sales data automatically
        public SalesAnalysisService()
        {
            _salesData = ParseSalesData();
        }

        private static List<SalesModel> ParseSalesData()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFile", "SalesData.txt");
            var salesRecords = new List<SalesModel>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Sales data file not found.");
                return salesRecords;
            }

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines.Skip(1)) 
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        salesRecords.Add(new SalesModel(line));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Invalid line skipped: {line} | Error: {ex.Message}");
                    }
                }
            }
            return salesRecords;
        }

        private string GetMonthKey(DateTime date) => date.ToString("yyyy-MM");

        private void UpdateDictionary<T>(Dictionary<string, T> dictionary, string key, T value, Func<T, T, T> updateFunc)
        {
            if (!dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary[key] = updateFunc(dictionary[key], value);
        }

        public decimal GetTotalSales()
        {
            decimal total = 0;
            foreach (var sale in _salesData)
                total += sale.TotalPrice;
            return total;
        }

        public Dictionary<string, decimal> GetMonthWiseSales()
        {
            var result = new Dictionary<string, decimal>();

            foreach (var sale in _salesData)
                UpdateDictionary(result, GetMonthKey(sale.Date), sale.TotalPrice, (old, add) => old + add);

            return result;
        }

        public Dictionary<string, string> GetMostPopularItems()
        {
            var salesCount = new Dictionary<string, Dictionary<string, int>>();

            foreach (var sale in _salesData)
            {
                string month = GetMonthKey(sale.Date);
                if (!salesCount.ContainsKey(month))
                    salesCount[month] = new Dictionary<string, int>();

                UpdateDictionary(salesCount[month], sale.SKU, 1, (old, add) => old + add);
            }

            var result = new Dictionary<string, string>();
            foreach (var month in salesCount)
            {
                string topItem = "";
                int maxCount = 0;
                foreach (var item in month.Value)
                {
                    if (item.Value > maxCount)
                    {
                        maxCount = item.Value;
                        topItem = item.Key;
                    }
                }
                result[month.Key] = topItem;
            }
            return result;
        }

        public Dictionary<string, string> GetTopRevenueItems()
        {
            var revenuePerMonth = new Dictionary<string, Dictionary<string, decimal>>();

            foreach (var sale in _salesData)
            {
                string month = GetMonthKey(sale.Date);
                if (!revenuePerMonth.ContainsKey(month))
                    revenuePerMonth[month] = new Dictionary<string, decimal>();

                UpdateDictionary(revenuePerMonth[month], sale.SKU, sale.TotalPrice, (old, add) => old + add);
            }

            var result = new Dictionary<string, string>();
            foreach (var month in revenuePerMonth)
            {
                string topItem = "";
                decimal maxRevenue = 0;
                foreach (var item in month.Value)
                {
                    if (item.Value > maxRevenue)
                    {
                        maxRevenue = item.Value;
                        topItem = item.Key;
                    }
                }
                result[month.Key] = topItem;
            }
            return result;
        }

        public Dictionary<string, (int Min, int Max, double Avg)> GetMinMaxAvgOrdersForPopularItem()
        {
            var mostPopularItems = GetMostPopularItems();
            var ordersPerMonth = new Dictionary<string, List<int>>();

            foreach (var sale in _salesData)
            {
                string month = GetMonthKey(sale.Date);
                if (mostPopularItems.ContainsKey(month) && sale.SKU == mostPopularItems[month])
                {
                    if (!ordersPerMonth.ContainsKey(month))
                        ordersPerMonth[month] = new List<int>();

                    ordersPerMonth[month].Add(sale.Quantity);
                }
            }

            var result = new Dictionary<string, (int Min, int Max, double Avg)>();
            foreach (var month in ordersPerMonth)
            {
                var orders = month.Value;
                int min = orders[0], max = orders[0];
                double sum = 0;
                foreach (var count in orders)
                {
                    if (count < min) min = count;
                    if (count > max) max = count;
                    sum += count;
                }
                double avg = sum / orders.Count;
                result[month.Key] = (min, max, avg);
            }

            return result;
        }

        // Print the sales report
        public void PrintReport(
            decimal totalSales,
            Dictionary<string, decimal> monthWiseSales,
            Dictionary<string, string> popularItems,
            Dictionary<string, string> topRevenueItems,
            Dictionary<string, (int Min, int Max, double Avg)> minMaxAvgOrders)
        {
            Console.WriteLine($"\nTotal Sales: {totalSales:C}\n");

            Console.WriteLine("Below is Month wise Total Sale,Most Popular Items,Most Popular Item and MinMaxAvg : ");
            foreach (var month in monthWiseSales.Keys)
            {
                Console.WriteLine($"Month: {month}");
                Console.WriteLine($"Sales Total: {monthWiseSales[month]:C}");
                Console.WriteLine($"Most Popular Item: {popularItems[month]}");
                Console.WriteLine($"Top Revenue Item: {topRevenueItems[month]}");

                if (minMaxAvgOrders.ContainsKey(month))
                {
                    var stats = minMaxAvgOrders[month];
                    Console.WriteLine($"Min Orders: {stats.Min}");
                    Console.WriteLine($"Max Orders: {stats.Max}");
                    Console.WriteLine($"Avg Orders: {stats.Avg:F2}");
                }

                Console.WriteLine("-----------------------------");
            }
        }
    }
}
