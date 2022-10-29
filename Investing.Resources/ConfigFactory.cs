using System.Collections.Generic;

namespace Investing.Resources
{
    public static class ConfigFactory
    {
        public static Config TestConfig
        {
            get
            {
                var resource = new ResourceProvider();
                return new Config
                {
                    UserName = "Test",
                    BrokerFiles = new List<BrokerFile>()
                    {
                        new BrokerFile(2019, resource.GetInterBrokerReport("Test.2019")),
                        new BrokerFile(2020, resource.GetInterBrokerReport("Test.2020")),
                        new BrokerFile(2021, resource.GetInterBrokerReport("Test.2021")),
                        new BrokerFile(2022, resource.GetInterBrokerReport("Test.2022")),
                    }
                };
            }
        }       
        
        public static Config RuslanConfig
        {
            get
            {
                var resource = new ResourceProvider();
                return new Config
                {
                    CurrentYear = 2021,
                    UserName = "Ruslan",
                    BrokerFiles = new List<BrokerFile>()
                    {
                        new BrokerFile(2020, resource.GetInterBrokerReport("Ruslan.2020")),
                        new BrokerFile(2021, resource.GetInterBrokerReport("Ruslan.2021")),
                    }
                };
            }
        }
    }
}