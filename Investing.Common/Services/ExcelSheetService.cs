using System.IO;
using OfficeOpenXml;

namespace Investing.Common.Services
{
    public class ExcelSheetService
    {
        private ExcelPackage _package;

        private ExcelWorksheet _dividendsSheet;

        private ExcelWorksheet _tradesSheet;
        
        public string FileName { get; set; }

        private ExcelPackage GetPackage()
        {
            if (_package == null)
            {
                var directory = Directory.GetCurrentDirectory();
                var fileTemplate = $"{directory}\\Templates\\Nalog.xlsx"; 
                
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                _package = new ExcelPackage(new FileInfo(FileName), new FileInfo(fileTemplate));
            }

            return _package;
        }

        public ExcelWorksheet GetTradesSheet => _tradesSheet ??= CreateSheet("Сделки");

        public ExcelWorksheet GetDividendsSheet => _dividendsSheet ??= CreateSheet("Дивиденды");
        
        private ExcelWorksheet CreateSheet(string sheetName)
        {
            try
            {
                GetPackage().Workbook.Worksheets.Delete(sheetName);
            }
            catch
            {
            }

            return GetPackage().Workbook.Worksheets.Add(sheetName);
        }

        public void Save()
        {
            _package.Save();
            _package.Dispose();
        }
    }
}