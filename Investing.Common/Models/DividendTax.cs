using System;

namespace Investing.Common.Models
{
    public class DividendTax
    {
        public DateTime Date { get; set; }

        public string Currency { get; set; }

        public string Symbol { get; set; }
        
        public decimal Sum { get; set; }

        public decimal SumAbs => Math.Abs(Sum);
    }
}