using System;
using System.Collections.Generic;
using System.Reflection;

namespace PairMatching.ExcelTool
{
    public class SpredsheetInfoBuilder<T>
    {
        SpredsheetInfo<T> _result;

        public SpredsheetInfoBuilder(string fileName, string filePath = @"Excels\")
        {
            _result = new()
            {
                FileName = fileName,
                OutputItems = new(),
                Worksheets = new()
            };
        }

        public SpredsheetInfoBuilder<T> AddItems(IEnumerable<T> items, string worksheetName)
        {
            _result.InputItems = items;
            _result.WorksheetName = worksheetName;
            
            _result.Worksheets[worksheetName] = items;
            return this;
        }

        public SpredsheetInfoBuilder<T> AddProperties(params PropertyInfo[] properties)
        {
            _result.Properties = properties;
            return this;
        }

        public SpredsheetInfo<T> Build()
        {
            if (_result.InputItems == null)
            {
                throw new Exception();
            }
            if (_result.Properties == null)
            {
                throw new Exception();
            }

            foreach (var item in _result.InputItems)
            {
                Dictionary<string, object> finalItem = new();
                foreach (var prop in _result.Properties)
                {
                    finalItem[prop.Name] = prop.GetValue(item, null);
                }
                _result.OutputItems.Add(finalItem);
            }

            return _result;
        }
    }
}
