using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PairMatching.DomainModel.Email
{
    internal class CreateEmailTemplate
    {
        private string GetPropValue(object obj, string name)
        {
            foreach (string part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }

            return obj.ToString();
        }

        public string Compile(object model, string template)
        {
            var allPropToReplace = from s in template.Split(' ')
                                   where s.StartsWith("@Model")
                                   select s;

            foreach (var strToReplace in allPropToReplace)
            {
                var indexOfProp = strToReplace.IndexOf("@Model.") + "@Model.".Length;
                var temp = strToReplace.Substring(indexOfProp);

                var propName = temp.TrimEnd(',', '?', '!', '\r', '\n');
                var propVal = new StringBuilder()
                    .Append(GetPropValue(model, propName));
                if (Regex.IsMatch(temp, @"\r\n,!"))
                {
                    var charctersThatLeft = from c in temp
                                            where @"\r\n,?!".Contains(c)
                                            select c;
                    propVal.Append(string.Join("", charctersThatLeft));
                }

                template = template.Replace(strToReplace, propVal.ToString());
            }

            return template;
        }
    }
}