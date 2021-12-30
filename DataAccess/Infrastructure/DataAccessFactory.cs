using Microsoft.Extensions.Configuration;
using System;
using PairMatching.DataAccess.MongoImplementation;
using PairMatching.DataAccess.JsonLocalImplementation;

namespace PairMatching.DataAccess.Infrastructure
{
    public class DataAccessFactory
    {
        public static IDataAccess GetDalFactory(IConfiguration config)
        {      
            var connectionString = config.GetConnectionString("Remote");
            var dalImp = config.GetSection("DataAccess")["Default"];
            if(dalImp == "Mongo")
            {
                return new MongoDataAccess(connectionString);
            }
            else if(dalImp == "Json")
            {
                return new JsonDataAccess();
            }
            throw new Exception("No Data Access implementation");
        }
    }
}
