using System;
using PairMatching.Models;
using PairMatching.Configuration;

namespace PairMatching.DataAccess.Infrastructure
{
    public class ContextFactory
    {
        readonly Configuration.MyConfiguration _configurations;

        public ContextFactory(Configuration.MyConfiguration configurations)
        {
            _configurations = configurations;
        }

        public IDataAccess GetContext()
        {
            if(_configurations.IsTest)
            {
                return new JsonDataAccess();
            }
            else
            {
                return new MongoDataAccess(_configurations.ConnctionsStrings);
            }
            throw new Exception("No Data Access implementation");
        }
    }
}
