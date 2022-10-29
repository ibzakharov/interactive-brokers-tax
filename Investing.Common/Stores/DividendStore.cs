using System.Collections.Generic;
using System.Linq;
using Investing.Common.Models;

namespace Investing.Common.Stores
{
    public class DividendStore
    {
        private List<Dividend> _dividends = new List<Dividend>();

        public void Add(Dividend dividend)
        {
            _dividends.Add(dividend);
        }

        public List<Dividend> ByYear(int year)
        {
            return _dividends.Where(d => d.Date.Year == year).ToList();
        }
    }
}