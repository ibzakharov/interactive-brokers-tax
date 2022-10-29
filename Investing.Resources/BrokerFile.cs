namespace Investing.Resources
{
    public class BrokerFile
    {
        public int Year { get; set; }

        public string FileText { get; set; }

        public BrokerFile(int year, string fileText)
        {
            Year = year;
            FileText = fileText;
        }
    }
}