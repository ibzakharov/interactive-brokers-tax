using System.Collections.Generic;

namespace Investing.Resources
{
    public class Config
    {
        public decimal Commission { get; set; }

        public bool HasCommission => Commission > 0;

        public int CurrentYear { get; set; }

        public string UserName { get; set; }

        public List<BrokerFile> BrokerFiles { get; set; }
    }
}