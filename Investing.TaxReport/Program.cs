using Autofac;
using Investing.Common;

namespace Investing.TaxReport
{
    class Program
    {
        static void Main(string[] args)
        {
            var containerBuilder = AutofacContainer.GetContainer();
            containerBuilder.RegisterType<Application>();
            var container = containerBuilder.Build();
            container.Resolve<Application>().Run();
        }
    }
}