using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PairMatching.Tools
{
    public static class Extensions
    {
        /// <summary>
        /// Splice text with new line in every 'n' characters 
        /// </summary>
        /// <param name="text">The text to splice</param>
        /// <param name="n">Number of characters to put new line</param>
        /// <returns>This string with new lines in every 'n' characters</returns>
        public static string SpliceText(this string text, int n = 8)
        {
            text = string.Join(Environment.NewLine, text.Split()
                .Select((word, index) => new { word, index })
                .GroupBy(x => x.index / n)
                .Select(grp => string.Join(" ", grp.Select(x => x.word))));
            return text;
        }

        public static TimeSpan ToTimeSpan(this string timeStr)
        {
            string timeFormat = Regex.Replace(timeStr, "[^0-9.:-]", "");
            if(TimeSpan.TryParse(timeFormat, out TimeSpan res))
                return res;
            
            throw new Exception($"can not parse {timeStr} to time");
        }

        public static async Task<IEnumerable<T>> WhenAll<T>(List<Task> tasks1, params Task<T>[] tasks)
        {
            var allTasks = Task.WhenAll(tasks);

            try
            {
                return await allTasks;
            }
            catch
            {

            }

            throw allTasks.Exception ?? throw new Exception("Bad");
        }

        public static string GetDescriptionFromEnumValue(this Enum value, string engOrHeb = "heb")
        {
            if (value == null)
                return string.Empty;
            var attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(EnumDescriptionAttribute), false)
                .SingleOrDefault() as EnumDescriptionAttribute;
            if(attribute != null)
            {
                return engOrHeb == "eng" ? attribute.DisplyEngDescription : attribute.HebDescription;
            }
            return string.Empty;
        }

        public static string GetDescriptionIdFromEnum(this Enum value)
        {
            if (value == null)
                return string.Empty;
            var attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(EnumDescriptionAttribute), false)
                .SingleOrDefault() as EnumDescriptionAttribute;
            if (attribute != null)
            {
                return attribute.DescriptionId;
            }
            return string.Empty;
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields
                            .SelectMany(f => f.GetCustomAttributes(
                                typeof(EnumDescriptionAttribute), false), (
                                    f, a) => new { Field = f, Att = a })
                            .Where(a =>
                            {
                                var ea = (EnumDescriptionAttribute)a.Att;
                                return ea.EngDescriptions.Contains(description) 
                                || 
                                ea.HebDescription == description;
                            })
                            .SingleOrDefault();
            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }

        public static object GetValueFromDescription(string description, Type type)
        {
            //object to = Activator.CreateInstance(type);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields
                            .SelectMany(f => f.GetCustomAttributes(
                                typeof(EnumDescriptionAttribute), false), (
                                    f, a) => new { Field = f, Att = a })
                            .Where(a =>
                            {
                                var ea = (EnumDescriptionAttribute)a.Att;
                                return ea.EngDescriptions.Contains(description)
                                ||
                                ea.HebDescription == description;
                            })
                            .SingleOrDefault();
            var result = field == null ? default : field.Field.GetRawConstantValue();
            return Enum.Parse(type, result.ToString());
        }
    }


}
