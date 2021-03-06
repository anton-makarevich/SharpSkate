using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class SessionsViewModelTests
    {
        private const string AccountId = "accountId";

        private readonly IApiService _apiClient = Substitute.For<IApiService>();
        
        private readonly SessionsViewModel _sut;
        private readonly IAccountService _accountService = Substitute.For<IAccountService>();

        public SessionsViewModelTests()
        {
            _accountService.UserId.Returns(AccountId);
            _sut = new SessionsViewModel(_apiClient, _accountService);
        }

        [Fact]
        public async Task Fetches_Sessions_For_User_From_Api_On_Page_Load()
        {
            _sut.AttachHandlers();

            await _apiClient.Received(1).GetSessionsAsync(AccountId, ApiNames.AzureApiSubscriptionKey);
        }

        [Fact]
        public void Gets_Sessions_From_Api()
        {
            var sessions = new List<SessionDto>();
            for (var i = 0; i < 3; i++)
            {
                sessions.Add( new SessionDto
                    {
                        AccountId = AccountId,
                        Id = Guid.NewGuid().ToString("N")
                    });
            }

            _apiClient.GetSessionsAsync(AccountId,ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new GetSessionsResponse{Sessions = sessions}));
            
            _sut.AttachHandlers();

            _sut.Sessions.ToList().Should().Equal(sessions);
        }
        //todo: select should select
        //todo: next should pass selected to session manager and go to live session
    }
}