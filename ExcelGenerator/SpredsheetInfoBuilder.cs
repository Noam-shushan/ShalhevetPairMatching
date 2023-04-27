using System;
using System.Collections.Generic;
using System.Reflection;
using PairMatching.Tools;

namespace PairMatching.ExcelTool
{
    public class SpredsheetInfoBuilder<T>
    {
        SpredsheetInfo<T> _result;

        public SpredsheetInfoBuilder()
        {
            _result = new()
            {
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
                throw new MissingFieldException(nameof(SpredsheetInfoBuilder<T>), nameof(_result.InputItems));
            }
            if (_result.Properties == null)
            {
                throw new MissingFieldException(nameof(SpredsheetInfoBuilder<T>), nameof(_result.Properties));
            }

            foreach (var item in _result.InputItems)
            {
                Dictionary<string, object> finalItem = new();
                foreach (var prop in _result.Properties)
                {
                    var coluoName = prop.GetTextFromExportProperty();
                    if (prop.PropertyType == typeof(DateTime))
                    {
                        var obj = prop.GetValue(item, null) as DateTime? ?? new DateTime?();
                        finalItem[coluoName] = obj?.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        finalItem[coluoName] = prop.GetValue(item, null);
                    }
                }
                _result.OutputItems.Add(finalItem);
            }

            return _result;
        }
    }
}
