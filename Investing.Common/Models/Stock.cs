namespace Investing.Common.Models
{
    public class Stock
    {
        public string Symbol { get; set; }

        public string Name { get; set; }

        public string Identity { get; set; }

        public string Exchange { get; set; }
        
        public string Currency 
        {
            get
            {
                return Exchange switch
                {
                    "ASX" => "AUD",
                    "IBIS" => "EUR",
                    "WSE" => "PLN",
                    "SGX" => "SGD",
                    "LSE" => "GBP",
                    "SBF" => "EUR",
                    "TSE" => "CAD",
                    _ => "USD"
                };
            }
        }
    }
}