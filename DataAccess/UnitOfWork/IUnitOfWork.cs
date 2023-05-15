using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using PairMatching.DataAccess.Repositories;
using PairMatching.Models;
using System;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.UnitOfWorks
{
    public interface IUnitOfWork
    {
        ConfigRepositry ConfigRepositry { get; init; }

        IModelRepository<OldPairDto> OldPairsRepositry { get; init; }

        IModelRepository<Student> StudentRepositry { get; init; }

        IModelRepository<IsraelParticipant> IsraelParticipantsRepositry { get; init; }
        
        IModelRepository<WorldParticipant> WorldParticipantsRepositry { get; init; }

        IModelRepository<Pair> PairsRepositry { get; init; }

        IModelRepository<MatchingHistory> MatchingHistorisRepositry { get; init; }

        IModelRepository<EmailModel> EmailRepositry { get; init; }

        Task Complete();
    }
}