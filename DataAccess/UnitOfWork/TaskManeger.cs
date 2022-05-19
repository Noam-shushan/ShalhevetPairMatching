using System.Collections.Generic;
using System.Threading.Tasks;
using PairMatching.Tools;

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
            await Extensions.WhenAll<object>(Tasks);
            Tasks.Clear();
        } 
    }
}