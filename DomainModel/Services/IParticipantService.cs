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
        Task<IEnumerable<IsraelParticipant>> GetAllFromIsrael();
        
        Task<IEnumerable<WorldParticipant>> GetAllFromWorld();
        
        IEnumerable<CountryUtc> GetCountryUtcs();

        Task<IEnumerable<Participant>> GetParticipantsWix();
        
        Task SetNewParticipints();
        
        Task UpserteParticipant(Participant participant);
    }
}
