using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Sanet.SmartSkating.Azure.Models;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;

namespace Sanet.SmartSkating.Azure.Services
{
    public class AzureTablesDataService:IDataService
    {
        private readonly ILogger _log;
        private const string TableName = "WayPointsTable";

        private readonly CloudTable _scoresTable;

        private readonly bool _hasStorageAccess;

        public AzureTablesDataService(ILogger log)
        {
            _log = log;
            var connectionString = Environment.GetEnvironmentVariable("TableConnectionString");
            _hasStorageAccess = !string.IsNullOrEmpty(connectionString);
            if (!_hasStorageAccess)
            {
                ErrorMessage = "NoStorageAccess";
                log.LogCritical(ErrorMessage);
                return;
            }

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _scoresTable = tableClient.GetTableReference(TableName);
        }
        
        public async Task<bool> SaveWayPointAsync(WayPointDto wayPoint)
        {
            if (!_hasStorageAccess)
                return false;
            try
            {
                await _scoresTable.CreateIfNotExistsAsync();
                var entity = new WayPointEntity(wayPoint);
                var insertOperation = TableOperation.InsertOrMerge(entity);
                var result = await _scoresTable.ExecuteAsync(insertOperation);

                return result.Result is WayPointEntity;
            }
            catch(Exception exception)
            {
                _log.LogError(exception,exception.Message);
                return false;
            }
        }

        public string ErrorMessage { get; } = string.Empty;
    }
}