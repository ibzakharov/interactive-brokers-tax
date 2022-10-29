using System;
using System.Runtime.Serialization;

namespace Investing.Common
{
    /// <summary>
    /// Ошибка при объединении сделок Open и Close
    /// </summary>
    [Serializable]
    public class TradeUnionException : Exception
    {
        public TradeUnionException()
        {
        }

        public TradeUnionException(string message) : base(message)
        {
        }

        public TradeUnionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TradeUnionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}