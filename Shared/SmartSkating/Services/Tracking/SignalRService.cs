using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Models.EventArgs;

namespace Sanet.SmartSkating.Services.Tracking
{
    public class SignalRService:ISyncService
    {
        private HubConnection? _connection;
        public async Task ConnectToHub(string sessionId)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{ApiNames.BaseUrl}/{sessionId}", (opts) =>
                {
                    opts.Headers.Add("Ocp-Apim-Subscription-Key", ApiNames.AzureApiSubscriptionKey);
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