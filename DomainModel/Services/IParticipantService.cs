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
        Task DeleteParticipaint(Participant participant);
        Task<IEnumerable<Participant>> GetAll();
        
        Task<IEnumerable<IsraelParticipant>> GetAllFromIsrael();
        
        Task<IEnumerable<WorldParticipant>> GetAllFromWorld();
        
        IEnumerable<CountryUtc> GetCountryUtcs();
        Task SendToArcive(Participant participant);
        Task<Participant> InsertParticipant(Participant participant);
        Task UpdateParticipaint(Participant participant, bool isChaengCountery = false);
        Task AddNote(Note note, Participant participant);
        Task DeleteNote(Note selectedNote, Participant participant);
        Task SetNewParticipintsFromWix();
        Task ExloadeFromArcive(Participant selectedParticipant);
    }
}
