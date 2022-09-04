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
        const string dir = @"Excels\";

        public ExcelGenerator()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public async Task Generate<T>(SpredsheetInfo<T> spredsheetInfo)
        {
            await Generate(spredsheetInfo.OutputItems, spredsheetInfo.FileName, spredsheetInfo.WorksheetName);
        }

        public async Task Generate<T>(IEnumerable<T> values, string fileName, string worksheetName = "Main")
        {
            var file = GetFileInfo(fileName);

            try
            {
                using var package = new ExcelPackage(file);

                var ws = package.Workbook.Worksheets.Add(worksheetName);


                var range = ws.Cells["A1"].LoadFromCollection(values, true);
                range.AutoFitColumns();

                await package.SaveAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task Generate(IEnumerable<IDictionary<string, object>> values, string fileName, string worksheetName = "Main")
        {
            var file = GetFileInfo(fileName);

            try
            {
                using var package = new ExcelPackage(file);

                var ws = package.Workbook.Worksheets.Add(worksheetName);

                var range = ws.Cells["A1"].LoadFromDictionaries(values, true);
                range.AutoFitColumns();

                await package.SaveAsync();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private static FileInfo GetFileInfo(string fileName)
        {
            var file = new FileInfo($"{dir}{fileName}.xlsx");

            if (file.Exists)
            {

                try
                {
                    file.Delete();
                }
                catch (IOException)
                {

                    throw new Exception($"File '{fileName}' is open, and can not be modify\nPlease close the file before");
                }
            }

            return file;
        }
    }
}
