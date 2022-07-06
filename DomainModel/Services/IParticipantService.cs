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
        Task<IEnumerable<Participant>> GetAllParticipants();

        Task<IEnumerable<Student>> GetAllStudents();

        IEnumerable<CountryUtc> GetCountryUtcs();

        Task<IEnumerable<Participant>> GetParticipantsWix();
        Task MoveOneToNewDatabaseTest();
        Task MoveToNewDatabaseTest();
        Task UpdateParticipant(Participant participant);
    }
}
