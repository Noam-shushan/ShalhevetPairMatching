using PairMatching.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Services
{
    public interface IPairsService
    {
        Task<IEnumerable<Pair>> GetAllPairs();
    }

}
