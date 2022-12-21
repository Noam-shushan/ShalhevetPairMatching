using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.FileSystemGlobbing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DomainTesting
{
    public class Model
    {
        public string Name { get; set; }

        public string Track { get; set; }
    }

    [TestFixture]
    public class UtileTest
    {

        [Test]
        public void GetProps()
        {
            string body = "Hi @Name how r u doing\nI found your @Track email in moam@gmail.com sdsd";

            var models = new Model[]
            {
                new Model
                {
                    Name = "noam",
                    Track = "hasidot"
                },
                new Model
                {
                    Name = "sara",
                    Track = "pilosh"
                }
            };

            var matchs =  Regex.Matches(body, @"(?<!\w)@\w+");

            if (matchs.Any())
            {
                var bodys = new List<string>();
                foreach (var m in models)
                {
                    var newBody = body;
                    foreach(var propName in matchs.ToList())
                    {
                        var propVal = m.GetType()
                            .GetProperty(propName.Value.Replace("@", ""));
                        if(propVal != null)
                        {
                            newBody = newBody.Replace(propName.Value, propVal.GetValue(m).ToString());
                        }
                    }
                    bodys.Add(newBody);
                }
            }

        }
    }
}
