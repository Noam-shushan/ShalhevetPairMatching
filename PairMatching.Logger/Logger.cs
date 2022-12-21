using PairMatching.DataAccess.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PairMatching.Loggin
{

    public class Logger
    {
        readonly IDataAccess _dataAccess;

        const string collectionName = "Logs";

        public List<Log> Logs { get; init; }

        public Logger(string connactionStrings)
        {
            _dataAccess = new MongoDataAccess(connactionStrings);
            Logs = new();
        }

        public void LogError(string msg, Exception ex, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
        {
            var log = new Log
            {
                Date = DateTime.Now,
                Message = msg,
                Source = $"path: '{callerPath}', line: '{callerLine}'",
                Type = "error",
                Exception = new ExceptionDto(ex)
            };
            Logs.Add(log);
        }

        public void LogInformation(string msg, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
        {
            var log =  new Log
            {
                Date = DateTime.Now,
                Message = msg,
                Source = $"path: '{callerPath}', line: '{callerLine}'",
                Type = "info",
                Exception = new ExceptionDto()
            };
            Logs.Add(log);
        }

        public bool IsLogged(string logId)
        {
            return Logs.Find(l => l.Id == logId) != null;
        }

        public async Task SendLogs()
        {
            await _dataAccess.InsertMany(collectionName, Logs);
        }
    }
}