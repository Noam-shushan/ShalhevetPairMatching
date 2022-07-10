using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models.Dtos
{
    public class EmailWixDto
    {
        public IEnumerable<string> to { get; set; }

        public string subject { get; set; }

        public string body { get; set; }
        
        
    }
}
