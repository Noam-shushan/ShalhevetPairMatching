using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.ExcelTool;
using PairMatching.DomainModel.Services;

namespace DomainTesting.ExcelTest
{
    [TestFixture]
    public class CreateExcelTests
    {
        [Test]
        public async Task CreateExcelTest_Simple()
        {
            var spredsheet = GetSpredsheetInfo();

            var eg = new ExcelGenerator();

            await eg.Generate(spredsheet);
        }

        public SpredsheetInfo<Stam> GetSpredsheetInfo()
        {
            var builder = new SpredsheetInfoBuilder<Stam>("stam");
            var stam = new Stam();
            var spredsheet = builder.AddItems(new List<Stam>
            {
                new (){ Age = 10, Email = "email", Name = "Noam1" },
                new (){ Age = 11, Email = "email1", Name = "Noam2" },
                new (){ Age = 30, Email = "email2", Name = "Noam3" },
                new (){ Age = 50, Email = "email3", Name = "Noam4" }
            }, "stam items")
                .AddProperties(
                stam.GetType().GetProperty(nameof(Stam.Age)),
                stam.GetType().GetProperty(nameof(Stam.Name)))
                .Build();
            return spredsheet;
        }

    }
}
