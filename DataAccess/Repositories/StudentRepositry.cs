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
    public class StudentRepositry : IModelRepository<Student>
    {
        IDataAccess _dal;
        private const string studentsCollectionName = "Students";

        public StudentRepositry(IDataAccess dal)
        {
            _dal = dal;
        }

        public async Task SaveStudentsToDrive()
        {
            var students =
                await _dal.LoadManyAsync<Student>(studentsCollectionName);

            var js = new JsonDataAccess();
            await js.InsertMany(studentsCollectionName, students);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            var students = 
                await _dal.LoadManyAsync<Student>(studentsCollectionName);
            return students;
        }

        public async Task<IEnumerable<Student>> GetAllAsync(Expression<Func<Student, bool>> predicate)
        {
            var students =
                    await _dal.LoadManyAsync(studentsCollectionName, predicate);
            return students;
        }

        public Task<Student> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task Insert(Student model)
        {
            throw new NotImplementedException();
        }

        public Task Insert(IEnumerable<Student> students)
        {
            throw new NotImplementedException();
        }

        public Task Update(Student student)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
