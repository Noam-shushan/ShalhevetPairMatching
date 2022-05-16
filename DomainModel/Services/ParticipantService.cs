using PairMatching.DataAccess.UnitOfWork;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Services
{
    public class ParticipantService : IParticipantService
    {
        readonly IUnitOfWork _unitOfWork;

        public ParticipantService(IDataAccessFactory dataAccessFactory)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();
        }

        public Task<IEnumerable<Participant>> GetAllParticipants()
        {
            return _unitOfWork
                .ParticipantsRepositry
                .GetAllAsync();
        }

        public Task<IEnumerable<Student>> GetAllStudents()
        {
            return _unitOfWork
                .StudentRepositry
                .GetAllAsync(s => !s.IsDeleted);
        }
    }
}
