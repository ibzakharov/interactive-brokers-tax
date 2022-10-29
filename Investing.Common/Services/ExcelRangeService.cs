using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Investing.Common.Services
{
    public class ExcelRangeService
    {
        public void CreateTitleHeader(ExcelWorksheet sheet, int row, int start, int end, string title, Color color)
        {
            var range = sheet.Cells[row, start, row, end];
            range.Merge = true;
            range.Value = title;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(color);
            range.Style.Font.Bold = true;
            SetBorder(range);
        }

        public void SetBorder(ExcelRange range)
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }
    }
}