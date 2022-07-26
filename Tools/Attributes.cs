using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Tools
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumDescriptionAttribute : Attribute
    {
        public EnumDescriptionAttribute()
        {

        }
        
        public EnumDescriptionAttribute(string hebDescription, params string[] engDescription)
        {
            EngDescriptions = engDescription;
            HebDescription = hebDescription;
            DescriptionId = EngDescriptions.LastOrDefault();
        }

        public IEnumerable<string> EngDescriptions { get; set; }

        public string HebDescription { get; set; }

        public string DescriptionId { get; set; }

        public string DisplyEngDescription { get => EngDescriptions.FirstOrDefault(); }
    }
}
