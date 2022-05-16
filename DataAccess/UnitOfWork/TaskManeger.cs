using System.Collections.Generic;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.UnitOfWork
{
    public class TaskManeger
    {
        public List<Task> Tasks { get; set; } = new();

        public void Add(params Task[] tasks)
        {
            Tasks.AddRange(tasks);
        }

        public async Task SaveChangesAsync()
        {
            await Task.WhenAll(Tasks);
            Tasks.Clear();
        } 
    }
}