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
        readonly IDataAccess _dataAccess;

        const string studentsCollectionName = "Students";

        public StudentRepositry(IDataAccess dal)
        {
            _dataAccess = dal;
        }

        public async Task SaveToDrive()
        {
            var students =
                await _dataAccess.LoadManyAsync<Student>(studentsCollectionName);

            var js = new JsonDataAccess();
            await js.InsertMany(studentsCollectionName, students);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            var students = 
                await _dataAccess.LoadManyAsync<Student>(studentsCollectionName);
            return students;
        }

        public async Task<IEnumerable<Student>> GetAllAsync(Expression<Func<Student, bool>> predicate)
        {
            var students =
                    await _dataAccess.LoadManyAsync(studentsCollectionName, predicate);
            return students;
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            var student = await _dataAccess.LoadOneAsync<Student>(studentsCollectionName, id);
            return student ?? throw new KeyNotFoundException($"Student {id} not found");
        }

        public Task Insert(Student student)
        {
            return _dataAccess.InsertOne(studentsCollectionName, student);
        }

        public Task Insert(IEnumerable<Student> students)
        {
            return _dataAccess.InsertMany(studentsCollectionName, students);
        }

        public Task Update(Student student)
        {
            return _dataAccess.UpdateOne(studentsCollectionName, student, student.Id);
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
