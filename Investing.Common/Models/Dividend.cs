using System;

namespace Investing.Common.Models
{
    public class Dividend
    {
        public DateTime Date { get; set; }

        public string Currency { get; set; }

        public string Symbol { get; set; }
        
        public decimal Sum { get; set; }
    }
}