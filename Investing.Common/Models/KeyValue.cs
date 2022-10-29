namespace Investing.Common.Models
{
    public class KeyValue<T1,T2>
    {
        public KeyValue(T1 key, T2 value)
        {
            Key = key;
            Value = value;
        }

        public T1 Key { get; set; }
        
        public T2 Value { get; set; }
    }
}