using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Investing.Resources
{
    public class ResourceProvider
    {
        public string GetInterBrokerReport(string resourceName)
        {
            var fullResourceName = $"InteractiveBrokers.{resourceName}.csv";
            return GetResource(fullResourceName);
        }

        private static async Task<string> GetResourceAsync(string resourceName)
        {
            var assembly = Assembly.GetEntryAssembly();
            var assemblyName = assembly.GetName().Name;
            var fullResourceName = $"{assemblyName}.Resources.{resourceName}";
            var resourceStream = assembly.GetManifestResourceStream(fullResourceName);
            using var reader = new StreamReader(resourceStream, Encoding.UTF8);
            var resource = await reader.ReadToEndAsync();
            return resource;
        }    
        
        private static string GetResource(string resourceName)
        {
            var assembly = Assembly.GetAssembly(typeof(ResourceProvider));
            var assemblyName = assembly.GetName().Name;
            var fullResourceName = $"{assemblyName}.{resourceName}";
            var resourceStream = assembly.GetManifestResourceStream(fullResourceName);
            using var reader = new StreamReader(resourceStream, Encoding.UTF8);
            var resource = reader.ReadToEnd();
            return resource;
        }
    }
}