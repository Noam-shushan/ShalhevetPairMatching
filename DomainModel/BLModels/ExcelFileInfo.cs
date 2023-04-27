using System.Collections.Generic;
using System.Reflection;

namespace PairMatching.DomainModel.BLModels
{
    public class ExcelFileInfo<T>
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string SheetName { get; set; }

        public IEnumerable<T> Values { get; set; }

        public PropertyInfo[] Properties { get; set; }
    }

    public record PropWithText
    {
        public PropertyInfo Property { get; set; }

        public string Text { get; set; }
    }
}
