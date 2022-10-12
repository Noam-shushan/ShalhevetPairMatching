using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Services
{
    public interface IParticipantService
    {
        Task<IEnumerable<Participant>> GetAll();
        
        Task<IEnumerable<IsraelParticipant>> GetAllFromIsrael();
        
        Task<IEnumerable<WorldParticipant>> GetAllFromWorld();
        
        IEnumerable<CountryUtc> GetCountryUtcs();
        
        Task<Participant> UpserteParticipant(Participant participant);
    }
}
