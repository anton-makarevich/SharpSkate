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
            var url = "https://smart-skating-sync.service.signalr.net/client/?hub={sessionId}";
            var token = "eyJhbGciOiJIUzI1NiIsImtpZCI6IjE0NjQ5MTYwMjkiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE2MTU3NjMwMTksImV4cCI6MTYxNTc2NjYxOSwiaWF0IjoxNjE1NzYzMDE5LCJhdWQiOiJodHRwczovL3NtYXJ0LXNrYXRpbmctc3luYy5zZXJ2aWNlLnNpZ25hbHIubmV0L2NsaWVudC8_aHViPWNpbGx1bSBlbGl0IG5vbiJ9.a0lBqgXFIHH8H28rOHfWPOIF5mWSQM451-Vclumg0Ug";
            // _connection = new HubConnectionBuilder()
            //     .WithUrl($"{ApiNames.BaseUrl}/{sessionId}", (opts) =>
            //     {
            //         opts.Headers.Add("Ocp-Apim-Subscription-Key", ApiNames.AzureApiSubscriptionKey);
            //     })
            //     .Build();
            _connection = new HubConnectionBuilder()
                .WithUrl($"https://smart-skating-sync.service.signalr.net/client/?hub={sessionId}", (opts) =>
                {
                    opts.AccessTokenProvider = () => Task.FromResult(token);
                })
                .Build();
            _connection.On<string, string>("newWaypoint", (user, message) =>
            {
               Console.WriteLine($"SIGNALR MSG: {message}");
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

        public async Task CloseConnection()
        {
            if (_connection != null) await _connection.StopAsync();
        }
    }
}