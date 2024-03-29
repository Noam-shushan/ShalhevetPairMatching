﻿using PairMatching.DataAccess.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Configurations;

namespace PairMatching.DomainModel.DataAccessFactory
{
    public class DataAccessFactory : IDataAccessFactory 
    {
        readonly MyConfiguration _config;

        public DataAccessFactory(MyConfiguration config)
        {
            _config = config;
        }

        public IUnitOfWork GetDataAccess()
        {
            return new UnitOfWork(_config);
        }
    }
}
