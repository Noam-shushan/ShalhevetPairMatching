using PairMatching.DataAccess.UnitOfWork;
using PairMatching.Models;
using PairMatching.WixApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.Configurations;

namespace PairMatching.DomainModel.Services
{
    public class PairService : IPairsService
    {
        readonly IUnitOfWork _unitOfWork;

        readonly WixDataReader _wix;

        public PairService(IDataAccessFactory dataAccessFactory, MyConfiguration config)
        {
            _unitOfWork = dataAccessFactory.GetDataAccess();

            _wix = new WixDataReader(config);
        }

        public async Task<IEnumerable<Pair>> GetAllPairs()
        {
            var oldPairs = await _unitOfWork
                .PairsRepositry
                .GetAllAsync();
            var studList = await _unitOfWork
                    .StudentRepositry
                    .GetAllAsync();
            foreach(var pair in oldPairs)
            {
                pair.StudentFromIsrael = studList
                    .FirstOrDefault(s => s.Id == pair.StudentFromIsraelId);
                pair.StudentFromWorld = studList
                    .FirstOrDefault(s => s.Id == pair.StudentFromWorldId);
            }
            var list = oldPairs.Select(p => p.ToNewPair());

            return list;
        }
    }

}
