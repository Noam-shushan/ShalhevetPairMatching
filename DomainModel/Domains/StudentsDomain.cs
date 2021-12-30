using PairMatching.DataAccess.Repositories;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Domains
{
    public class StudentsDomain
    {
        readonly IModelRepository<Student> _studentRepository;

        public StudentsDomain(IModelRepository<Student> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            var students = await _studentRepository.GetAllAsync(s => !s.IsDeleted);
            return students;
        }
        
        public Task InsertManyStudents(IEnumerable<Student> students)
        {
            return _studentRepository.Insert(students);
        }

        public async Task SaveStudentToDrive()
        {
            await _studentRepository.SaveToDrive();
        }
    }
}
