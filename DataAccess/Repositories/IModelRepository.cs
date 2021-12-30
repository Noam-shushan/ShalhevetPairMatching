using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.Repositories
{
    public interface IModelRepository<TModel> where TModel : class
    {
        Task SaveStudentsToDrive();
        /// <summary>
        /// Get all records of this model
        /// </summary>
        /// <returns>All records of this model</returns>
        Task<IEnumerable<TModel>> GetAllAsync();

        /// <summary>
        /// Get all records of this model
        /// </summary>
        /// <returns>All records of this model</returns>
        Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TModel, bool>> predicate);

        /// <summary>
        /// Get single record of this model
        /// </summary>
        /// <param name="id">The id of the record</param>
        /// <param name="getSimple">Optional flag to get flat object and not full object</param>
        /// <returns>The record match to this id</returns>
        Task<TModel> GetByIdAsync(int id);

        /// <summary>
        /// Insert one record of this model to the database
        /// </summary>
        /// <param name="model">The new record</param>
        /// <returns></returns>
        Task Insert(TModel model);

        /// <summary>
        /// Insert many record of this model to the database
        /// </summary>
        /// <param name="models">The new records to insert</param>
        /// <returns></returns>
        Task Insert(IEnumerable<TModel> models);

        /// <summary>
        /// Update one record of this model
        /// </summary>
        /// <param name="model">The record to update</param>
        /// <returns></returns>
        Task Update(TModel model);

        /// <summary>
        /// Delete one record of this model match to the given id
        /// </summary>
        /// <param name="id">The id of the record to delete</param>
        /// <returns></returns>
        Task Delete(int id);
    }
}
