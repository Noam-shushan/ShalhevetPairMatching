using Microsoft.Extensions.Configuration;
using PairMatching.DataAccess.Infrastructure;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.Repositories
{
    public class RepositoriesContainer
    {
        public IModelRepository<Student> StudentRepositry { get; init; }
        public IModelRepository<Pair> PairRepositry { get; init; }
        public ConfigRepositry ConfigRepositry { get; init; }

        public RepositoriesContainer(IConfiguration config)
        {
            var dataAccess = DataAccessFactory.GetDalFactory(config);
            StudentRepositry = new StudentRepositry(dataAccess);
            PairRepositry = new PairRepositry(dataAccess);
            ConfigRepositry = new ConfigRepositry(dataAccess);
        }
    }
}
