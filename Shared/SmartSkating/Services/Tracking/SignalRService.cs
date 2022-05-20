using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Models.EventArgs;

namespace Sanet.SmartSkating.Services.Tracking
{
    public class SignalRService:ISyncService
    {
        private HubConnection? _connection;
        private readonly IConfigService _configService;

        public SignalRService(IConfigService configService)
        {
            _configService = configService;
        }

        public async Task ConnectToHub(string sessionId)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{_configService.BaseUrl}/{sessionId}", (opts) =>
                {
                    opts.Headers.Add("Ocp-Apim-Subscription-Key", _configService.AzureApiSubscriptionKey);
                })
                .Build();
            
            _connection.On(SyncHubMethodNames.AddWaypoint, (WayPointDto wayPointDto) =>
            {
                WayPointReceived?.Invoke(null, new WayPointEventArgs(wayPointDto));
            });
            
            _connection.On(SyncHubMethodNames.AddWaypoint, (SessionDto session) =>
            {
                if (session.IsCompleted)
                {
                    SessionClosedReceived?.Invoke(null, new SessionEventArgs(session));
                }
            });

            try
            {
                await _connection.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"SIGNALR ERR: {e.Message}");
            }
            Console.WriteLine($"SIGNALR STT: {_connection.State}");
        }

        public event EventHandler<WayPointEventArgs>? WayPointReceived;
        public event EventHandler<SessionEventArgs>? SessionClosedReceived;
   
        public async Task CloseConnection()
        {
            if (_connection != null) await _connection.StopAsync();
        }
    }
}