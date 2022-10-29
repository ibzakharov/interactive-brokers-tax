using System;

namespace Investing.Common.Models
{
    /// <summary>
    /// Сделка
    /// </summary>
    public class Trade : ICloneable<Trade>
    {
        /// <summary>
        /// Класс актива
        /// </summary>
        public string ActiveType { get; set; }

        /// <summary>
        /// Валюта
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Символ
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Дата/Время
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        public decimal Quantity { get; set; }

        public decimal QuantityAbs => Math.Abs(Quantity);

        /// <summary>
        /// Цена транзакции
        /// </summary>
        public decimal TransactionPrice { get; set; }

        // /// <summary>
        // /// Цена закрытия
        // /// </summary>
        // public decimal ClosePrice { get; set; }

        /// <summary>
        /// Выручка
        /// </summary>
        public decimal Gain { get; set; }

        /// <summary>
        /// Комиссия/плата
        /// </summary>
        public decimal Comission { get; set; }

        public decimal ComissionAbs => Math.Abs(Comission);

        // /// <summary>
        // /// Базис
        // /// </summary>
        // public decimal Basis { get; set; }

        /// <summary>
        /// Realized P / L 
        /// </summary>
        public decimal RealizedPL { get; set; }

        // /// <summary>
        // /// Market revaluation P / L
        // /// </summary>
        // public decimal MarketRevaluationPL { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        public Operation Operation => Quantity > 0 ? Operation.Open : Operation.Close;

        public bool IsSplitted { get; set; }
        
        public Trade Clone()
        {
            return new Trade
            {
                ActiveType = ActiveType,
                Symbol = Symbol,
                Currency = Currency,
                DateTime = DateTime,
                Quantity = Quantity,
                Comission = Comission,
                Gain = Gain,
                TransactionPrice = TransactionPrice,
                Code = Code,
                IsSplitted = IsSplitted
            };
        }

        public Trade Ref { get; set; }
    }
}