using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace PairMatching.DataAccess.Infrastructure
{
    public class MongoDataAccess : IDataAccess
    {
        // The connections strings of the database
        readonly string _connctionsStrings;

        readonly string _databaseName;

        public MongoDataAccess(string connctionsStrings, string databaseName)
        {
            _connctionsStrings = connctionsStrings;
            _databaseName = databaseName;
        }

        public MongoDataAccess(string connctionsStrings) : this(connctionsStrings, "Shalhevet")
        {
        }

        private IMongoCollection<T> ConnectToMongo<T>(string collectionName)
        {
            var client = new MongoClient(_connctionsStrings);

            var db = client.GetDatabase(_databaseName);

            return db.GetCollection<T>(collectionName);
        }

        public async Task<T> LoadOneAsync<T>(string collectionName, int id)
        {
            try
            {
                var collection = ConnectToMongo<T>(collectionName);

                var filter = Builders<T>.Filter.Eq("Id", id);

                var record = await collection.FindAsync(filter);
                
                return await record.FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> LoadOneAsync<T>(string collectionName, string id)
        {
            try
            {
                var collection = ConnectToMongo<T>(collectionName);

                var filter = Builders<T>.Filter.Eq("Id", id);

                var record = await collection.FindAsync(filter);

                return await record.FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> LoadManyAsync<T>(string collectionName, Expression<Func<T, bool>> predicate = null)
        {
            var collection = ConnectToMongo<T>(collectionName);

            IAsyncCursor<T> records;

            if (predicate != null)
            {
                var filter = Builders<T>.Filter.Where(predicate);
                records = await collection.FindAsync(filter);
            }
            else
            {
                records = await collection.FindAsync(_ => true);
            }

            return await records.ToListAsync();
        }

        public Task InsertMany<T>(string collectionName, IEnumerable<T> records)
        {
            var collection = ConnectToMongo<T>(collectionName);

            var task = collection.InsertManyAsync(records);

            return task;
        }

        public async Task<T> InsertOne<T>(string collectionName, T record)
        {
            var collection = ConnectToMongo<T>(collectionName);

            await collection.InsertOneAsync(record);

            return record;
        }

        public Task UpdateOne<T>(string collectionName, T record, dynamic id)
        {
            var collection = ConnectToMongo<T>(collectionName);

            var filter = Builders<T>.Filter.Eq("Id", id);

            var task = collection.ReplaceOneAsync(filter,
                record, new ReplaceOptions { IsUpsert = true });

            return task;
        }

        public Task Delete<T>(string collectionName, dynamic id)
        {
            var collection = ConnectToMongo<T>(collectionName);

            var filter = Builders<T>.Filter.Eq("Id", id);

            var task = collection.DeleteOneAsync(filter);

            return task;
        }
    }
}
