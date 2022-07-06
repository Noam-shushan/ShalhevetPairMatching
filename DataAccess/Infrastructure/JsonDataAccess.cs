using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
                            where (int)GetCurrentId(m) == id
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
                            where GetCurrentId(m) == id
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
                    var list = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
                    return predicate is null ? list : list.AsQueryable().Where(predicate);
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
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task InsertOne<T>(string collectionName, T record)
        {
            var curId = GetCurrentId(record);
            if (string.IsNullOrEmpty(curId) || curId == 0)
            {
                record.GetType().GetProperty("Id").SetValue(record, GetNewId());
            }
            var filePath = dir + collectionName + suffixFile;
            try
            {
                using (StreamWriter file = File.CreateText(filePath))
                {
                    var serializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented
                    };
                    await Task.Run(() => serializer.Serialize(file, record));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task UpdateOne<T>(string collectionName, T record, int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOne<T>(string collectionName, T record, string id)
        {
            throw new NotImplementedException();
        }        

        private string GetNewId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
