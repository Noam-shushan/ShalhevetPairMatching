using PairMatching.DomainModel.BLModels;
using PairMatching.Loggin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.ExcelTool;
using PairMatching.Tools;
using System.Reflection;
using SharpCompress.Common;
using static OfficeOpenXml.ExcelErrorValue;
using PairMatching.Tools;
using System.Collections.ObjectModel;

namespace PairMatching.DomainModel.Services
{
    public class ExcelExportingService
    {
        readonly Logger _logger;

        public ExcelExportingService(Logger logger)
        {
            _logger = logger;
        }

        public async Task Export<T>(ExcelFileInfo<T> excelFileInfo)
        {
            try
            {
                var spreadSheetInfoBuilder =
                    new SpredsheetInfoBuilder<T>();

                var spreadSheetInfo = spreadSheetInfoBuilder
                    .AddItems(excelFileInfo.Values, "Items")
                    .AddProperties(excelFileInfo.Properties)
                .Build();

                var excel = new ExcelGenerator(excelFileInfo.FilePath, excelFileInfo.FileName);
                await excel.Generate(spreadSheetInfo);
                _logger.LogInformation("Excel file was generated");
            }
            catch (Exception ex)
            {
                _logger.LogError("Can not genrate excel file, Sorry;(" ,ex);
                throw ex;
            }
        }

        public IEnumerable<PropWithText> GetPropertiesOfType<T>()
        {
            return from prop in typeof(T).GetProperties()
                   where prop.GetCustomAttributes(false)
                        .Any(attr => attr.GetType() == typeof(ExportPropertyAttribute))
                   select new PropWithText
                   {
                       Property = prop,
                       Text = prop.GetCustomAttributes(false)
                        .OfType<ExportPropertyAttribute>()
                        .FirstOrDefault()
                        .Text
                   };
        }
    }
}
