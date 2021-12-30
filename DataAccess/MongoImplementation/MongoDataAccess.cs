using PairMatching.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace PairMatching.DataAccess.MongoImplementation
{
    public class MongoDataAccess : IDataAccess
    {
        // The connections strings of the database
        readonly string _connctionsStrings;

        const string databaseName = "Shalhevet";

        public MongoDataAccess(string connctionsStrings)
        {
            _connctionsStrings = connctionsStrings;
        }

        private IMongoCollection<T> ConnectToMongo<T>(string collectionName)
        {
            var client = new MongoClient(_connctionsStrings);

            var db = client.GetDatabase(databaseName);

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

        public async Task<IEnumerable<T>> LoadManyAsync<T>(string collectionName)
        {
            var collection = ConnectToMongo<T>(collectionName);
            
            var records = await collection.FindAsync(_ => true);
            
            return await records.ToListAsync();
        }

        public async Task<IEnumerable<T>> LoadManyAsync<T>(string collectionName, Expression<Func<T, bool>> predicate)
        {
            var collection = ConnectToMongo<T>(collectionName);

            var filter = Builders<T>.Filter.Where(predicate);

            var records = await collection.FindAsync(filter);

            return await records.ToListAsync();
        }

        public Task InsertMany<T>(string collectionName, IEnumerable<T> records)
        {
            var collection = ConnectToMongo<T>(collectionName);

            var task = collection.InsertManyAsync(records);

            return task;
        }

        public Task InsertOne<T>(string collectionName, T record)
        {
            var collection = ConnectToMongo<T>(collectionName);

            var task = collection.InsertOneAsync(record);

            return task;
        }

        public Task UpdateOne<T>(string collectionName, T record, int id)
        {
            var collection = ConnectToMongo<T>(collectionName);

            var filter = Builders<T>.Filter.Eq("Id", id);

            var task = collection.ReplaceOneAsync(filter,
                record, new ReplaceOptions { IsUpsert = true });

            return task;
        }
    }
}
