using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.Infrastructure
{

    public interface IDataAccess
    { 
        Task<T> LoadOneAsync<T>(string collectionName, string id);
        
        Task<T> LoadOneAsync<T>(string collectionName, int id);

        Task<IEnumerable<T>> LoadManyAsync<T>(string collectionName, Expression<Func<T, bool>> predicate = null);

        Task InsertMany<T>(string collectionName, IEnumerable<T> records);

        Task<T> InsertOne<T>(string collectionName, T record);

        Task UpdateOne<T>(string collectionName, T record, dynamic id);

        Task Delete<T>(string collectionName, dynamic id);
    }
}
