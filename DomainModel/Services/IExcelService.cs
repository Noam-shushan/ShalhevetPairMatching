using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Tls.Crypto;
using PairMatching.ExcelTool;
using PairMatching.Models;
using PairMatching.Tools;

namespace PairMatching.DomainModel.Services
{
    public interface IExcelService
    {
        Task CreateSpredsheet<T>(SpredsheetInfo<T> spredsheetInfo);
    }

    public class ExcelService : IExcelService
    {
        ExcelGenerator _excelGenerator = new(); 

        public async Task CreateSpredsheet<T>(SpredsheetInfo<T> spredsheetInfo)
        {
            await _excelGenerator.Generate(spredsheetInfo);
        }
    }
}
