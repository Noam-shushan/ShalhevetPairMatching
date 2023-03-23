using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PairMatching.Tools
{
    public static class HelperFunction
    {
        /// <summary>
        /// return the differences between the local time in Israel 
        /// to the utc offset of somewhere in the word
        /// </summary>
        /// <param name="offset">utc offset somewhere in the word</param>
        /// <returns></returns>
        public static TimeSpan GetDifferenceUtc(TimeSpan offset)
        {
            var timeInIsrael = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time")
                .BaseUtcOffset;
            return offset - timeInIsrael;
        }

        public static dynamic GetCurrentId<T>(this T record)
        {
            return record.GetType()
                .GetProperty("Id")
                .GetValue(record, null);
        }

        public static string ReadJson(string fileName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            
            var jsonString = File.ReadAllText(path);
            return jsonString;
        }

        public static string CleanString(string input)
        {
            // Replace any non-letter characters with whitespace
            string cleaned = Regex.Replace(input, @"[^a-zA-Z]+", " ");

            // Replace consecutive whitespace characters with a single space
            cleaned = Regex.Replace(cleaned, @"\s+", " ");

            // Trim any whitespace from the beginning or end of the output string
            return cleaned.Trim();
        }
    }
}
