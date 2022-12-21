using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixApi
{
    public class WixIdNotCreateException : Exception
    {
        public string DbId { get; init; }

        public WixIdNotCreateException(string massge, string dbId) : base(massge)
        {
            DbId = dbId;
        }
    }

    public class WixIdNotFoundException : Exception
    {
        public string DbId { get; init; }

        public WixIdNotFoundException(string massge, string dbId) : base(massge)
        {
            DbId = dbId;
        }
    }

    public class WixBadResponsForPostException : Exception
    {
        public string Data { get; init; }

        public WixBadResponsForPostException(string massge, string jsonData) : base(massge)
        {
            Data = jsonData;
        }
    }
}
