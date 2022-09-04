using PairMatching.DataAccess.Infrastructure;
using PairMatching.DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static PairMatching.Tools.HelperFunction;

namespace PairMatching.DataAccess.Repositories
{
    public class ModelRepositroy<TModel> : IModelRepository<TModel> where TModel : class
    {
        private readonly IDataAccess _dataAccess;

        private readonly string _collectionName;

        readonly TaskManeger _taskManeger;

        public ModelRepositroy(IDataAccess dal, string collectionName, TaskManeger taskManeger)
        {
            _dataAccess = dal;
            _collectionName = collectionName;
            _taskManeger = taskManeger;
        }       

        public async Task SaveToDrive<U>(IEnumerable<U>  itemsToSave, string docName)
        {
            var jr = new JsonDataAccess();

            await jr.InsertMany(docName, itemsToSave);
        }

        public async Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TModel, bool>> predicate = null)
        {
            var recods = 
                await _dataAccess.LoadManyAsync(_collectionName, predicate);

            return recods;
        }

        public async Task<TModel> GetByIdAsync(int id)
        {
            var model = await _dataAccess.LoadOneAsync<TModel>(_collectionName, id);
            return model ?? throw new KeyNotFoundException($"Model with id = '{id}' not found");
        }

        public async Task<TModel> Insert(TModel model)
        {
            var result = await _dataAccess.InsertOne(_collectionName, model);
            
            //_taskManeger.Add(task);
            return result;
        }

        public Task InsertMany(IEnumerable<TModel> models)
        {
            var task = _dataAccess.InsertMany(_collectionName, models);
            //_taskManeger.Add(task);
            return task;
        }

        public Task Update(TModel model)
        {
            var id = model.GetCurrentId();
            var task = _dataAccess.UpdateOne(_collectionName, model, id);
            //_taskManeger.Add(task);
            return task;
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }
         
        public async Task<TModel> GetByIdAsync(string id)
        {
            var model = await _dataAccess.LoadOneAsync<TModel>(_collectionName, id);
            return model ?? throw new KeyNotFoundException($"Model with id = '{id}' not found");
        }
    }
}
