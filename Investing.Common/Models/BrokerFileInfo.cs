using System;

namespace Investing.Common.Models
{
    public class BrokerFileInfo
    {
        public int Year { get; set; }
        
        public decimal Commission { get; set; }

        public decimal CommissionAbs => Math.Abs(Commission);
    }
}