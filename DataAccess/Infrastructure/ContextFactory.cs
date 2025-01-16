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
            //return new MongoDataAccess(_configurations.ConnctionsStrings);
            return new MongoDataAccess(_configurations.ConnctionsStrings);
        }
    }
}
