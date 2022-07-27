using System;
using PairMatching.Configurations;

namespace PairMatching.DataAccess.Infrastructure
{
    public class ContextFactory
    {
        readonly MyConfiguration _configurations;

        public ContextFactory(MyConfiguration configurations)
        {
            _configurations = configurations;
        }

        public IDataAccess GetContext()
        {
            return new MongoDataAccess(_configurations.ConnctionsStrings);
            //if (_configurations.IsTest)
            //{
            //    return new JsonDataAccess();
            //}
            //else
            //{
            //    return new MongoDataAccess(_configurations.ConnctionsStrings);
            //}
            //throw new Exception("No Data Access implementation");
        }
    }
}
