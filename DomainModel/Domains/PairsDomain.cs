using PairMatching.DataAccess.Repositories;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Domains
{
    public class PairsDomain
    {
        IModelRepository<Pair> _pairRepository;
        public PairsDomain(IModelRepository<Pair> pairRepository)
        {
            _pairRepository = pairRepository;
        }
    }
}
