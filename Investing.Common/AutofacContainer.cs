using Autofac;
using Investing.Common.Services;
using Investing.Common.Stores;

namespace Investing.Common
{
    public static class AutofacContainer
    {
        public static ContainerBuilder GetContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<BrokerReportParser>();
            builder.RegisterType<ExcelSheetService>().SingleInstance();
            builder.RegisterType<ExcelTradeSheetService>();
            builder.RegisterType<ExcelDividendSheetService>();
            builder.RegisterType<StockStore>().SingleInstance();
            builder.RegisterType<TradeStore>().SingleInstance();
            builder.RegisterType<DividendStore>().SingleInstance();
            builder.RegisterType<DividendTaxStore>().SingleInstance();
            builder.RegisterType<CorporateActionStore>().SingleInstance();
            builder.RegisterType<SplitService>().SingleInstance();
            builder.RegisterType<TitleProvider>();
            builder.RegisterType<ExcelRangeService>();
            builder.RegisterType<TradeService>();
            return builder;
        }
    }
}