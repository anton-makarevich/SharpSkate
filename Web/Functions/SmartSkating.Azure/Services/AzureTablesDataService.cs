using System;
using System.Collections.Generic;
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
        private const string WayPointsTableName = "WayPointsTable";
        private const string SessionsTableName = "SessionsTable";

        private readonly CloudTable _wayPointsTable;
        private readonly CloudTable _sessionsTable;

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
            
            _wayPointsTable = tableClient.GetTableReference(WayPointsTableName);
            _sessionsTable = tableClient.GetTableReference(SessionsTableName);
        }
        
        public async Task<bool> SaveWayPointAsync(WayPointDto wayPoint)
        {
            var entity = new WayPointEntity(wayPoint);
            return await SaveEntityAsync(entity,_wayPointsTable);
        }

        private async Task<bool> SaveEntityAsync(TableEntity entity, CloudTable table)
        {
            if (!_hasStorageAccess)
                return false;
            try
            {
                await _wayPointsTable.CreateIfNotExistsAsync();
                var insertOperation = TableOperation.InsertOrMerge(entity);
                await table.ExecuteAsync(insertOperation);

                return true;
            }
            catch (Exception exception)
            {
                _log.LogError(exception, exception.Message);
                return false;
            }
        }

        public string ErrorMessage { get; } = string.Empty;
        public Task<List<WayPointDto>> GetAllWayPointsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteWayPointAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveSessionAsync(SessionDto session)
        {
            var entity = new SessionEntity(session);
            return await SaveEntityAsync(entity,_sessionsTable);
        }

        public Task<List<SessionDto>> GetAllSessionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteSessionAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveBleScanAsync(BleScanResultDto session)
        {
            throw new NotImplementedException();
        }

        public Task<List<BleScanResultDto>> GetAllBleScansAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBleScanAsync(string bleScanId)
        {
            throw new NotImplementedException();
        }
    }
}