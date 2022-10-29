using System.Collections.Generic;

namespace Investing.Common.Services
{
    public class SplitService
    {
        public string[] SplitLine(string line)
        {
            var result = new List<string>();
            
            var array = line.Split(',');

            string trimStr = null;
            
            for (var i = 0; i < array.Length; i++)
            {
                var str = array[i];
                
                if (str.StartsWith("\""))
                {
                    str = str.Remove(0, 1);
                    trimStr += str;
                }

                if (str.EndsWith("\""))
                {
                    trimStr += str.Remove(str.Length - 1, 1);
                    result.Add(trimStr);
                    trimStr = null;
                    continue;
                }

                if (trimStr == null)
                {
                    result.Add(str);
                }
            }

            return result.ToArray();
        }
    }
}