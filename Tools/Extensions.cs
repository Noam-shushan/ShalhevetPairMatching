﻿using System;
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
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> AddProperty(this object obj, string name, object value)
        {
            var dictionary = obj.ToDictionary();
            dictionary.Add(name, value);
            return dictionary;
        }

        // helper
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor property in properties)
            {
                result.Add(property.Name, property.GetValue(obj));
            }
            return result;
        }
    }

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

        public static bool SearchText(this string text, string searchWord)
        {
            if (string.IsNullOrEmpty(searchWord))
                return true;

            return text.ToLower().Contains(searchWord.ToLower(), StringComparison.InvariantCultureIgnoreCase);
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

        public static T GetValueFromId<T>(string id)
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
                                return ea.DescriptionId == id;
                            })
                            .SingleOrDefault();
            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
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

        public static Dictionary<string, int> GetIndexes<T>(this IEnumerable<T> items, Func<T, string> selctor)
        {
            return items.Select(selctor)
                .GroupBy((id) => id)
                .Select((p) => p.Key)
                .Select((id, i) => new { Id = id, Index = i })
                .ToDictionary(p => p.Id, p => p.Index);
        }

        public static string GetTextFromExportProperty(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<ExportPropertyAttribute>(false);
            return attributes.Any() ?
                 attributes
                .Cast<ExportPropertyAttribute>()
                .Select(x => x.Text)
                .FirstOrDefault()
                : property.Name;
        }
    }


}
