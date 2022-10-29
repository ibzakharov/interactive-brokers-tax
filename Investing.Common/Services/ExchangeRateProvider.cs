using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Xml;
using Investing.Data;
using Investing.Data.Models;

namespace Investing.Common.Services
{
    public static class ExchangeRateProvider
    {
        public static ExchangeRate Get(string currencyId, DateTime date)
        {
            using (var context = new ApplicationContext())
            {
                ExchangeRate rate = context.ExchangeRates.SingleOrDefault(i =>
                    i.Currency.Id == currencyId && i.DateTime == date);

                if (rate == null)
                {
                    var value = GetValueFromCentralBank(currencyId, date);
                    
                    var currency = context.Currencies.SingleOrDefault(c => c.Id == currencyId);
                    if (currency == null)
                    {
                        var newCurrency = new Currency() {Id = currencyId};
                        context.Currencies.Add(newCurrency);
                        context.SaveChanges();
                    }

                    currency = context.Currencies.SingleOrDefault(c => c.Id == currencyId);
                    rate = new ExchangeRate {Id = Guid.NewGuid(), Currency = currency, DateTime = date, Value = value};
                    context.ExchangeRates.Add(rate);
                    context.SaveChanges();
                }

                return rate;
            }
        }

        private static decimal GetValueFromCentralBank(string currencyId, DateTime date)
        {
            var client = new HttpClient();
            var dt = $"{date:dd}/{date:MM}/{date:yyyy}";
            var url = $"http://www.cbr.ru/scripts/XML_daily.asp?date_req={dt}";
            
            var responseMessage = client.GetAsync(url).Result;
            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Отсутствует курс валюты {currencyId} на указанную дату {date:d}");
            }
            var res = responseMessage.Content.ReadAsByteArrayAsync().Result;
            var xmlText = System.Text.Encoding.UTF8.GetString(res);
            
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlText);
            var valNodes = xmlDocument.SelectNodes("ValCurs/Valute");
            foreach (XmlElement valNode in valNodes)
            {
                var charCode = valNode["CharCode"].InnerText;
                if (charCode == currencyId)
                {
                    var value = valNode["Value"].InnerText;
                    var nominal = valNode["Nominal"].InnerText;
                    var nom = Decimal.Parse(nominal, CultureInfo.GetCultureInfo("ru-RU"));
                    var val = Decimal.Parse(value, CultureInfo.GetCultureInfo("ru-RU"));
                    val = val / nom;
                    return val;
                }
            }

            throw new Exception($"Отсутствует курс валюты {currencyId} на указанную дату {date:d}");
        }
    }
}