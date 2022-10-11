using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PairMatching.Tools.HelperFunction;

namespace PairMatching.DataAccess.Infrastructure
{
    internal class JsonDataAccess : IDataAccess
    {

        const string dir = @"localDB\";

        const string suffixFile = ".json";
        
        public JsonDataAccess()
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public async Task<T> LoadOneAsync<T>(string collectionName, int id)
        {
            var filePath = dir + collectionName + suffixFile;
            try
            {
                if (File.Exists(filePath))
                {
                    var jsonString = await File.ReadAllTextAsync(filePath);
                    var list = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
                    return (from m in list
                            where m.GetCurrentId() == id
                            select m)
                            .FirstOrDefault();

                }
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"can not load the file {filePath}" + ex.Message);
            }
        }

        public async Task<T> LoadOneAsync<T>(string collectionName, string id)
        {
            var filePath = dir + collectionName + suffixFile;
            try
            {
                if (File.Exists(filePath))
                {
                    var jsonString = await File.ReadAllTextAsync(filePath);
                    var list = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
                    return (from m in list
                            where m.GetCurrentId() == id
                            select m)
                            .FirstOrDefault();

                }
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"can not load the file {filePath}" + ex.Message);
            }
        }

        public async Task<IEnumerable<T>> LoadManyAsync<T>(string collectionName, Expression<Func<T, bool>> predicate = null)
        {
            var filePath = dir + collectionName + suffixFile;
            try
            {
                if (File.Exists(filePath))
                {
                    var jsonString = await File.ReadAllTextAsync(filePath);

                    //// To semulate the acushal time 
                    //await Task.Run(() => Thread.Sleep( 3000));

                    var list = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
                    var result = predicate is null ? list : list.AsQueryable().Where(predicate);
                    return result;
                }
                else
                {
                    return new List<T>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"can not load the file {filePath}" + ex.Message);
            }
        }

        public async Task InsertMany<T>(string collectionName, IEnumerable<T> records)
        {
            var filePath = dir + collectionName + suffixFile;
            try
            {
                using (StreamWriter file = File.CreateText(filePath))
                {
                    var serializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented
                    };
                    await Task.Run(() => serializer.Serialize(file, records));

                    //// To semulate the acushal time 
                    //await Task.Run(() => Thread.Sleep(3000));
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<T> InsertOne<T>(string collectionName, T record)
        {
            try
            {
                var curId = record.GetCurrentId();
                if (string.IsNullOrEmpty(curId) || curId == 0)
                {
                    record.GetType().GetProperty("Id").SetValue(record, GetNewId());
                }

                var items = await LoadManyAsync<T>(collectionName);

                var list = items.ToList();
                
                list.Add(record);
                
                await InsertMany(collectionName, list);

                return record;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }        

        private string GetNewId()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task UpdateOne<T>(string collectionName, T record, dynamic id)
        { 
            var items = await LoadManyAsync<T>(collectionName);

            var list = items.ToList();
            
            var found = list.Find(x => x.GetCurrentId() == id);
            if(found == null)
            {
                throw new Exception($"Can not find item with id: {id}");
            }
            
            list.Remove(found);
            list.Add(record);
            
            await InsertMany(collectionName, list);
        }
        
        public async Task Delete<T>(string collectionName, dynamic id)
        {
            var items = await LoadManyAsync<T>(collectionName);
            
            var list = items.ToList();
            
            var found = list.Find(x => x.GetCurrentId() == id);
            if (found == null)
            {
                throw new Exception($"Can not find item with id: {id}");
            }
            
            list.Remove(found);
            
            await InsertMany(collectionName, list);
        }
    }
}
