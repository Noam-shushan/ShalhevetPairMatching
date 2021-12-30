using PairMatching.DataAccess.Infrastructure;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.Repositories
{
    public class PairRepositry : IModelRepository<Pair>
    {
        IDataAccess _dal;
        private const string pairsCollectionName = "Pairs";

        public PairRepositry(IDataAccess dal)
        {
            _dal = dal;
        }

        public Task<IEnumerable<Pair>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Pair>> GetAllAsync(Expression<Func<Pair, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Pair> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task Insert(Pair model)
        {
            throw new NotImplementedException();
        }

        public Task Insert(IEnumerable<Pair> models)
        {
            throw new NotImplementedException();
        }

        public Task Update(Pair model)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task SaveStudentsToDrive()
        {
            throw new NotImplementedException();
        }
    }
}
