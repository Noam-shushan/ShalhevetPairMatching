using PairMatching.DataAccess.Repositories;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Services
{
    public class CleanupAfterUI_Test
    {
        Dictionary<string, (IModelRepository<BaseModel>, IEnumerable<string>)> _tasksToDelete = new();

        public async Task TestsCleanup(string collectionZone)
        {

            if (_tasksToDelete.TryGetValue(collectionZone, out (IModelRepository<BaseModel>, IEnumerable<string>) myTask))
            {
                List<Task> tasks = new();
                var repository = myTask.Item1;
                var idList = myTask.Item2;
                foreach (var id in idList)
                {
                    tasks.Add(repository.Delete(id));

                }

                await Task.WhenAll(tasks);
            }
        }

        public void AddTaskToDelete(string collectionZone, IModelRepository<BaseModel> repository, string id)
        {
            if (_tasksToDelete.TryGetValue(collectionZone, out (IModelRepository<BaseModel>, IEnumerable<string>) myTask))
            {
                var idList = myTask.Item2;
                if(idList != null)
                {
                    idList.Append(id);
                }
            }
            else
            {
                _tasksToDelete.Add(collectionZone, (repository, new List<string> { id }));
            }    

        }
    }
}
