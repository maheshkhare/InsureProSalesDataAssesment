using System;

namespace InsureProSalesDataAssesment.Models
{
    public class SalesModel
    {
        public DateTime Date { get; set; }
        public string SKU { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public SalesModel(string line)
        {
            var parts = line.Split(',');
            Date = DateTime.Parse(parts[0].Trim());
            SKU = parts[1].Trim();
            UnitPrice = decimal.Parse(parts[2].Trim());
            Quantity = int.Parse(parts[3].Trim());
            TotalPrice = decimal.Parse(parts[4].Trim());
        }
    }
}
