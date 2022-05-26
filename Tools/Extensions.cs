using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
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



        public static string ToDescriptionString<T>(this T enumVal, string engOrHeb) where T : Enum 
        {
            EnumDescriptionAttribute[] attributes = (EnumDescriptionAttribute[])enumVal
               .GetType()
               .GetField(enumVal.ToString())
               .GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            if(attributes.Length > 0)
            {
                return engOrHeb == "eng" ? attributes[0].DisplyEngDescription : attributes[0].HebDescription;
            }
            return string.Empty;
        }

        public static string GetDescriptionFromEnumValue(Enum value, string engOrHeb)
        {
            EnumDescriptionAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(EnumDescriptionAttribute), false)
                .SingleOrDefault() as EnumDescriptionAttribute;
            if(attribute != null)
            {
                return engOrHeb == "eng" ? attribute.DisplyEngDescription : attribute.HebDescription;
            }
            return string.Empty;
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
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
    }


}
