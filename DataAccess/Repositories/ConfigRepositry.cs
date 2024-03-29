﻿using PairMatching.DataAccess.Infrastructure;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PairMatching.Configurations;

namespace PairMatching.DataAccess.Repositories
{
    public class ConfigRepositry
    {
        readonly IDataAccess _dataAccess;

        readonly string countersAndSpredsheetLastRange = "CountersAndLastDataOfSpredsheet";

        const string configTableName = "Config";

        public ConfigRepositry(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public Task<SpredsheetLastRange> GetSpredsheetLastRange()
        {
            var lastRange = 
                 _dataAccess.LoadOneAsync<SpredsheetLastRange>(countersAndSpredsheetLastRange, 2);
            return lastRange;
        }

        public Task SaveSpredsheetLastRange(SpredsheetLastRange spredsheetLastRange)
        {
            return _dataAccess.InsertOne(countersAndSpredsheetLastRange, spredsheetLastRange);
        }

        public Task<DbConfig> GetDbConfig(string id)
        {
            return _dataAccess.LoadOneAsync<DbConfig>(configTableName, id);
        }

        public Task UpdateDbConfig(DbConfig dbConfig)
        {
            return _dataAccess.UpdateOne(configTableName, dbConfig, dbConfig.Id);
        }
    }
}
