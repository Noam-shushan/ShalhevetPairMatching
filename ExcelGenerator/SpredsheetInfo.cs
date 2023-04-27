using System.Collections.Generic;
using System.Reflection;

namespace PairMatching.ExcelTool
{
    public class SpredsheetInfo<T>
    {
        public string WorksheetName { get; set; }

        public Dictionary<string, IEnumerable<T>> Worksheets { get; set; }

        public IEnumerable<T> InputItems { get; set; }
        
        /// <summary>
        /// List of items 
        /// </summary>
        public List<Dictionary<string, object>> OutputItems { get; set; }

        public IEnumerable<PropertyInfo> Properties { get; set; }
    }
}
