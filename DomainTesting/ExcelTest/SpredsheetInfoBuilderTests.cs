using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.ExcelTool;
using PairMatching.DomainModel.Services;
using System.Reflection;

namespace DomainTesting.ExcelTest
{
    public class Stam
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    [TestFixture]
    public class SpredsheetInfoBuilderTests
    {
        [Test]
        public void SpredsheetInfoBuilderTest_Simple()
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
                .AddProperties(stam.GetType().GetProperty(nameof(Stam.Age)),
                    stam.GetType().GetProperty(nameof(Stam.Name))
                    ).Build();
        }
    }
}
