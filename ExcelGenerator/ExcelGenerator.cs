using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.ExcelTool
{
    public class ExcelGenerator
    {
        readonly string _directoryPath;
        readonly string _fileName;


        public ExcelGenerator(string dirPath, string fileName)
        {
            _directoryPath = dirPath;
            _fileName = fileName;
            if (!Directory.Exists(dirPath))
            {
                _directoryPath = @"Excel Files/";
            }
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task Generate<T>(SpredsheetInfo<T> spredsheetInfo)
        {
            await Generate(spredsheetInfo.OutputItems, spredsheetInfo.WorksheetName);
        }

        public async Task Generate<T>(IEnumerable<T> values, string worksheetName = "Main")
        {
            try
            {
                var file = GetFileInfo();

                using var package = new ExcelPackage(file);

                var ws = package.Workbook.Worksheets.Add(worksheetName);

                var range = ws.Cells["A1"].LoadFromCollection(values, true);
                range.AutoFitColumns();

                await package.SaveAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Generate(IEnumerable<IDictionary<string, object>> values, string worksheetName = "Main")
        {
            try
            {
                var file = GetFileInfo();
                
                using var package = new ExcelPackage(file);

                var ws = package.Workbook.Worksheets.Add(worksheetName);

                var range = ws.Cells["A1"].LoadFromDictionaries(values, true);
                range.AutoFitColumns();

                await package.SaveAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }

        }

        private FileInfo GetFileInfo()
        {
            var file = new FileInfo(@$"{_directoryPath}\{_fileName}.xlsx");

            if (file.Exists)
            {

                try
                {
                    file.Delete();
                }
                catch (IOException)
                {
                    throw new Exception($"File '{_fileName}' is open, and can not be modify\nPlease close the file before");
                }
            }

            return file;
        }
    }
}
