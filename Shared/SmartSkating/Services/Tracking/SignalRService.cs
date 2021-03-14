using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Sanet.SmartSkating.Dto;

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
            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
               Console.WriteLine($"SIGNALR MSG: {message}");
            });

            await _connection.StartAsync();
        }

        public async Task CloseConnection()
        {
            if (_connection != null) await _connection.StopAsync();
        }
    }
}