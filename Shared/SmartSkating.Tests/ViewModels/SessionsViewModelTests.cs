﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Models.Geometry;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Services.Tracking;
using Sanet.SmartSkating.Tests.Models.Geometry;
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
        private readonly  ISessionProvider _sessionProvider = Substitute.For<ISessionProvider>();
        private readonly  ITrackService _trackService= Substitute.For<ITrackService>();
        private readonly INavigationService _navigationService = Substitute.For<INavigationService>();

        public SessionsViewModelTests()
        {
            _accountService.UserId.Returns(AccountId);
            _sut = new SessionsViewModel(_apiClient, _accountService, _sessionProvider, _trackService);
            _sut.SetNavigationService(_navigationService);
        }

        [Fact]
        public async Task Fetches_Sessions_For_User_From_Api_On_Page_Load()
        {
            _sut.AttachHandlers();

            await _apiClient.Received(1).GetSessionsAsync(AccountId,false, ApiNames.AzureApiSubscriptionKey);
        }

        [Fact]
        public void Gets_Sessions_From_Api()
        {
            var sessions = CreatSessions();

            _apiClient.GetSessionsAsync(AccountId,false,ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new GetSessionsResponse{Sessions = sessions}));
            
            _sut.AttachHandlers();

            _sut.Sessions.ToList().Should().Equal(sessions);
        }
        
        [Fact]
        public void Only_Gets_Active_Sessions_For_A_Rink_When_Rink_Is_Selected()
        {
            var rink = new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish, "rinkId");
            _trackService.SelectedRink.Returns(rink);
            var sessions = CreatSessions();
            sessions.First().RinkId = rink.Id;

            _apiClient.GetSessionsAsync(AccountId, true, ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new GetSessionsResponse{Sessions = sessions}));
            
            _sut.AttachHandlers();

            _sut.Sessions.ToList().Should().Equal(sessions.Take(1).ToList());
        }

        [Fact]
        public void SelectSession_Selects_Session()
        {
            var sessions = CreatSessions();
            _sut.AttachHandlers();
            var sessionToSelect = sessions.First();

            _sut.SelectSession(sessionToSelect);

            _sut.SelectedSession.Should().Be(sessionToSelect);
        }
        
        [Fact]
        public void SessionSelected_Is_True_When_Session_Is_Selected()
        {
            var sessions = CreatSessions();
            _sut.AttachHandlers();
            var sessionToSelect = sessions.First();

            _sut.SelectSession(sessionToSelect);

            _sut.SessionSelected.Should().BeTrue();
        }
        
        [Fact]
        public void CanStart_IsFalse_When_Session_Is_Not_Selected()
        {
            _sut.AttachHandlers();
            
            _sut.CanStart.Should().BeFalse();
        }
        
        [Fact]
        public void CanStart_IsFalse_When_SelectedRink_Is_Null()
        {
            _sut.AttachHandlers();
            _sut.SelectSession(CreatSessions().First());
            
            _sut.CanStart.Should().BeFalse();
        }
        
        [Fact]
        public void CanStart_IsTrue_When_SelectedRink_Is_Not_Null_And_Session_Is_Selected()
        {
            _sut.AttachHandlers();
            _sut.SelectSession(CreatSessions().First());
            _trackService.SelectedRink
                .Returns( new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId"));
            
            _sut.CanStart.Should().BeTrue();
        }

        [Fact]
        public void Updates_CanStart_When_Session_Is_Selected()
        {
            var canStartUpdated = false;
            _sut.AttachHandlers();
            _sut.PropertyChanged += (sender, args) =>
            {
                canStartUpdated = args.PropertyName == nameof(_sut.CanStart);
            };
            _sut.SelectSession(CreatSessions().First());

            canStartUpdated.Should().BeTrue();
        }
        
        [Fact]
        public void Updates_SessionSelected_When_Session_Is_Selected()
        {
            var sessionSelectedUpdated = false;
            _sut.AttachHandlers();
            _sut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_sut.SessionSelected))
                    sessionSelectedUpdated = true;
            };
            _sut.SelectSession(CreatSessions().First());

            sessionSelectedUpdated.Should().BeTrue();
        }
        
        [Fact]
        public void SessionSelected_Is_False_When_Session_Is_Not_Selected()
        {
            _sut.AttachHandlers();
            
            _sut.SessionSelected.Should().BeFalse();
        }

        [Fact]
        public async Task StartCommand_Does_Nothing_When_Session_Is_Not_Selected()
        {
            await _sut.StartCommand.ExecuteAsync();

            _sessionProvider.DidNotReceive().SetActiveSession(Arg.Any<SessionDto>(), Arg.Any<Rink>());
        }

        [Fact]
        public async Task StartCommand_SetsSelectedSession_When_Session_Is_Selected()
        {
            var rink = new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId");
            _trackService.SelectedRink.Returns(rink);
            var session = CreatSessions().First();
            _sut.SelectSession(session);
            
            await _sut.StartCommand.ExecuteAsync();

            _sessionProvider.Received(1).SetActiveSession(session, rink);
        }

        private static List<SessionDto> CreatSessions()
        {
            var sessions = new List<SessionDto>();
            for (var i = 0; i < 3; i++)
            {
                sessions.Add(new SessionDto
                {
                    AccountId = AccountId,
                    Id = Guid.NewGuid().ToString("N"),
                    IsCompleted = false
                });
            }

            return sessions;
        }
        
        [Fact]
        public async Task Goes_To_Session_When_Start_Pressed_And_CanStart()
        {
            _trackService.SelectedRink
                .Returns(new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,"rinkId"));
            _sut.SelectSession(CreatSessions().First());

            await _sut.StartCommand.ExecuteAsync();

            await _navigationService.Received().NavigateToViewModelAsync<LiveSessionViewModel>();
        }
    }
}