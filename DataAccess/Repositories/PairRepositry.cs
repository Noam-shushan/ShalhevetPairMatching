using PairMatching.DataAccess.Infrastructure;
using PairMatching.DataAccess.JsonLocalImplementation;
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
        readonly IDataAccess _dataAccess;

        const string pairsCollectionName = "Pairs";

        public PairRepositry(IDataAccess dal)
        {
            _dataAccess = dal;
        }

        public async Task<IEnumerable<Pair>> GetAllAsync()
        {
            var pairs = await 
                _dataAccess.LoadManyAsync<Pair>(pairsCollectionName);
            return pairs;
        }

        public async Task<IEnumerable<Pair>> GetAllAsync(Expression<Func<Pair, bool>> predicate)
        {
            var pairs = await
                        _dataAccess.LoadManyAsync(pairsCollectionName, predicate);
            return pairs;
        }

        public async Task<Pair> GetByIdAsync(int id)
        {
            var pair = await _dataAccess.LoadOneAsync<Pair>(pairsCollectionName, id);
            return pair ?? throw new KeyNotFoundException($"Pair {id} not found");
        }

        public Task Insert(Pair pair)
        {
            return _dataAccess.InsertOne(pairsCollectionName, pair);
        }

        public Task Insert(IEnumerable<Pair> pairs)
        {
            return _dataAccess.InsertMany(pairsCollectionName, pairs);
        }

        public Task Update(Pair pair)
        {
            return _dataAccess.UpdateOne(pairsCollectionName, pair, pair.Id);
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task SaveToDrive()
        {
            var pairs =
                 await _dataAccess.LoadManyAsync<Pair>(pairsCollectionName);

            var js = new JsonDataAccess();
            await js.InsertMany(pairsCollectionName, pairs);
        }
    }
}
