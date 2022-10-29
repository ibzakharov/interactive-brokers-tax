using System;

namespace Investing.Common.Models
{
    public class SpinOff
    {
        public DateTime DateTime { get; set; }

        public int From { get; set; }
        
        public string FromSymbol { get; set; }

        public int To { get; set; }
        
        public string ToSymbol { get; set; }
    }
}