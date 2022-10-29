using System.Collections.Generic;
using Investing.Common.Models;

namespace Investing.Common.Stores
{
    public class CorporateActionStore
    {
        private readonly List<StockSplit> _splits = new List<StockSplit>();
        private readonly List<SpinOff> _spinOffs = new List<SpinOff>();
        
        public void AddSplit(StockSplit split)
        {
            _splits.Add(split);
        }    
        
        public void AddSpinOff(SpinOff spinOff)
        {
            _spinOffs.Add(spinOff);
        }

        public IEnumerable<StockSplit> GetSplits() => _splits;
        
        public IEnumerable<SpinOff> GetSpinOffs() => _spinOffs;
    }
}