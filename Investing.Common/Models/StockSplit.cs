using System;

namespace Investing.Common.Models
{
    public class StockSplit
    {
        public string Simbol { get; set; }

        public DateTime DateTime { get; set; }

        public int From { get; set; }

        public int To { get; set; }
    }
}