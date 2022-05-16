using PairMatching.DataAccess.Infrastructure;
using PairMatching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.Repositories
{
    public class ConfigRepositry
    {
        readonly IDataAccess _dataAccess;

        readonly string countersAndSpredsheetLastRange = "CountersAndLastDataOfSpredsheet";

        public ConfigRepositry(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<SpredsheetLastRange> GetSpredsheetLastRange()
        {
            var lastRange = 
                await _dataAccess.LoadOneAsync<SpredsheetLastRange>(countersAndSpredsheetLastRange, 2);
            return lastRange;
        }

        public Task SaveSpredsheetLastRange(SpredsheetLastRange spredsheetLastRange)
        {
            return _dataAccess.InsertOne(countersAndSpredsheetLastRange, spredsheetLastRange);
        }
    }
}
