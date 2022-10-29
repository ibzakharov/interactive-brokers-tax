using System;

namespace Investing.Data.Models
{
    public class ExchangeRate
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public decimal Value { get; set; }
        
        public Currency Currency { get; set; }
    }
}