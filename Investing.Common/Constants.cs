using System.Drawing;
using Investing.Common.Models;

namespace Investing.Common
{
    public static class Constants
    {
        public static class ExcelColumnNumber
        {
            public const int First = 1;
            public const int Second = 2;
            public const int Third = 3;
            public const int Fourth = 4;
            public const int Fifth = 5;
            public const int Sixth = 6;
            public const int Seventh = 7;
            public const int Eighth = 8;
            public const int Ninth = 9;
            public const int Tenth = 10;
            public const int Eleventh = 11;
            public const int Twelfth = 12;
            public const int Thirteenth = 13;
            public const int Fourteenth = 14;
        }

        public static class ExcelColumnName
        {
            public const string First = "A";
            public const string Second = "B";
            public const string Third = "C";
            public const string Fourth = "D";
            public const string Fifth = "E";
            public const string Sixth = "F";
            public const string Seventh = "G";
            public const string Eight = "H";
            public const string Ninth = "I";
            public const string Tenth = "J";
            public const string Eleventh = "K";
            public const string Twelfth = "L";
            public const string Thirteenth = "M";
            public const string Fourteenth = "N";
        }

        public static class Colors
        {
            public static CellColor IncomeColor = new CellColor
            {
                Alpha = 0,
                Red = 158,
                Green = 191,
                Blue = 125
            };

            public static CellColor OutcomeColor = new CellColor
            {
                Alpha = 0,
                Red = 230,
                Green = 160,
                Blue = 110
            };

            public static Color TotalColor = Color.Pink;

            public static Color SymbolTitleColor = Color.FromArgb(216, 230, 252);

            public static CellColor PositiveProfitLossColor = new CellColor()
            {
                Red = 228,
                Green = 239,
                Blue = 219
            };

            public static CellColor NegativeProfitLossColor = new CellColor()
            {
                Red = 249,
                Green = 213,
                Blue = 231
            };

            public static Color MainTitleColor = Color.FromArgb(231, 230, 230);

            public static Color SecurityTitleColor = Color.FromArgb(255, 192, 0);
        }
    }
}