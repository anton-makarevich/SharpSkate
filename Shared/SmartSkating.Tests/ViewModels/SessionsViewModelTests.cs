using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
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
using Sanet.SmartSkating.ViewModels.Wrappers;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class SessionsViewModelTests
    {
        private const string AccountId = "accountId";

        private readonly SessionsViewModel _sut;

        private readonly IApiService _apiClient = Substitute.For<IApiService>();
        private readonly IAccountService _accountService = Substitute.For<IAccountService>();
        private readonly  ISessionProvider _sessionProvider = Substitute.For<ISessionProvider>();
        private readonly  ITrackService _trackService= Substitute.For<ITrackService>();
        private readonly INavigationService _navigationService = Substitute.For<INavigationService>();
        private readonly IDataSyncService _dataSyncService = Substitute.For<IDataSyncService>();

        public SessionsViewModelTests()
        {
            _accountService.UserId.Returns(AccountId);
            _sut = new SessionsViewModel(_apiClient, _accountService, _sessionProvider, _trackService, _dataSyncService);
            _sut.SetNavigationService(_navigationService);
        }

        [Fact]
        public async Task Fetches_Sessions_For_User_From_Api_On_Page_Load()
        {
            _sut.AttachHandlers();

            await _apiClient.Received(1).GetSessionsAsync(AccountId,false, ApiNames.AzureApiSubscriptionKey);
        }

        [Fact]
        public async Task Loads_All_tracks_If_No_Selected_Track()
        {
            _trackService.SelectedRink.ReturnsNull();
            _sut.AttachHandlers();

            await _trackService.Received(1).LoadTracksAsync();
        }

        [Fact]
        public async Task DoesNot_Load_All_tracks_If_Track_Is_Selected()
        {
            var rink = new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish, "rinkId");
            _trackService.SelectedRink.Returns(rink);

            _sut.AttachHandlers();

            await _trackService.DidNotReceive().LoadTracksAsync();
        }

        [Fact]
        public async Task DoesNot_Call_Api_If_UserId_Is_Empty()
        {
            _accountService.UserId.ReturnsNull();
            _sut.AttachHandlers();

            await _apiClient.DidNotReceiveWithAnyArgs()
                .GetSessionsAsync(AccountId,false, ApiNames.AzureApiSubscriptionKey);
        }

        [Fact]
        public void Gets_Sessions_From_Api()
        {
            var sessions = CreatSessions();

            _apiClient.GetSessionsAsync(AccountId,false,ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new GetSessionsResponse{Sessions = sessions}));

            _sut.AttachHandlers();

            _sut.Sessions.Select(s=>s.Session).ToList()
                .Should().Equal(sessions);
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

            _sut.Sessions.Count.Should().Be(1);
            _sut.Sessions.First().Session.Should().Be(sessions[0]);
        }

        [Fact]
        public void Gets_All_Sessions_For_User_When_Rink_Is_NotSelected()
        {
            _trackService.SelectedRink.ReturnsNull();
            var sessions = CreatSessions();

            _apiClient.GetSessionsAsync(AccountId, false, ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new GetSessionsResponse{Sessions = sessions}));

            _sut.AttachHandlers();

            _sut.Sessions.Count.Should().Be(sessions.Count);
        }

        [Fact]
        public void Starts_New_Session_If_No_Active_Session_For_Rink_Is_Found()
        {
            var rink = new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish, "rinkId");
            _trackService.SelectedRink.Returns(rink);
            var sessions = CreatSessions();

            _apiClient.GetSessionsAsync(AccountId, true, ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new GetSessionsResponse{Sessions = sessions}));

            _sut.AttachHandlers();

            _sessionProvider.Received(1).CreateSessionForRink(rink);
        }

        [Fact]
        public async void Does_Not_Start_New_Session_On_StartNewSession_Command_When_Rink_Is_Not_Selected()
        {
            _trackService.SelectedRink.ReturnsNull();

            await _sut.StartNewCommand.ExecuteAsync();

            _sessionProvider.DidNotReceiveWithAnyArgs().CreateSessionForRink(
                new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish, "rinkId"));
        }

        [Fact]
        public async void Starts_New_Session_On_StartNewSession_Command_When_Rink_IsSelected()
        {
            var rink = new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish, "rinkId");
            _trackService.SelectedRink.Returns(rink);

            await _sut.StartNewCommand.ExecuteAsync();

            _sessionProvider.Received(1).CreateSessionForRink(rink);
        }

        [Fact]
        public async Task NavigatesTo_LiveSession_Right_Away_If_No_Active_Session_For_Rink_Is_Found()
        {
            var rink = new Rink(RinkTests.EindhovenStart, RinkTests.EindhovenFinish, "rinkId");
            _trackService.SelectedRink.Returns(rink);
            var sessions = CreatSessions();

            _apiClient.GetSessionsAsync(AccountId, true, ApiNames.AzureApiSubscriptionKey)
                .Returns(Task.FromResult(new GetSessionsResponse{Sessions = sessions}));

            _sut.AttachHandlers();

            await _navigationService.Received().NavigateToViewModelAsync<LiveSessionViewModel>();
        }

        [Fact]
        public void SelectSession_Selects_Session()
        {
            var sessions = CreatSessions();
            _sut.AttachHandlers();
            var sessionToSelect = new SessionViewModel(sessions.First(),new List<TrackDto>(),_dataSyncService);

            _sut.SelectedSession=sessionToSelect;

            _sut.SelectedSession.Should().Be(sessionToSelect);
        }

        [Fact]
        public void SessionSelected_Is_True_When_Session_Is_Selected()
        {
            var sessions = CreatSessions();
            _sut.AttachHandlers();
            var sessionToSelect = new SessionViewModel(sessions.First(),new List<TrackDto>(),_dataSyncService);

            _sut.SelectedSession=sessionToSelect;

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
            _sut.SelectedSession=new SessionViewModel(CreatSessions().First(),new List<TrackDto>(),_dataSyncService);

            _sut.CanStart.Should().BeFalse();
        }

        [Fact]
        public void CanStart_IsTrue_When_SelectedRink_Is_Not_Null_And_Session_Is_Selected()
        {
            _sut.AttachHandlers();
            _sut.SelectedSession=new SessionViewModel(CreatSessions().First(),new List<TrackDto>(),_dataSyncService);
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
                if (args.PropertyName == nameof(_sut.CanStart))
                    canStartUpdated = true;
            };
            _sut.SelectedSession= new SessionViewModel(CreatSessions().First(),
                new List<TrackDto>(),_dataSyncService);

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
            _sut.SelectedSession= new SessionViewModel(CreatSessions().First(),new List<TrackDto>(),
                _dataSyncService);

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
            var session = new SessionViewModel(CreatSessions().First(),new List<TrackDto>(),
                _dataSyncService);
            _sut.SelectedSession=session;

            await _sut.StartCommand.ExecuteAsync();

            _sessionProvider.Received(1).SetActiveSession(session.Session, rink);
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
            _sut.SelectedSession= new SessionViewModel(CreatSessions().First(),new List<TrackDto>(),
                _dataSyncService);

            await _sut.StartCommand.ExecuteAsync();

            await _navigationService.Received().NavigateToViewModelAsync<LiveSessionViewModel>();
        }

        [Fact]
        public async Task OpenSessionDetails_Navigates_To_SessionDetailsView()
        {
            const string rinkId = "TrackId";
            _sut.SelectedSession = new SessionViewModel(new SessionDto
            {
                AccountId = AccountId,
                Id = Guid.NewGuid().ToString("N"),
                IsCompleted = false,
                RinkId = rinkId
            }, new[] {new TrackDto
                {
                    Id = rinkId,
                    Name = "TrackName",
                    Start = default,
                    Finish = default
                }
            },_dataSyncService);

            _trackService.SelectedRink
                .Returns(new Rink(RinkTests.EindhovenStart,RinkTests.EindhovenFinish,rinkId));

            await _sut.OpenDetailsCommand.ExecuteAsync();

            await _navigationService.Received(1).NavigateToViewModelAsync<SessionDetailsViewModel>();
        }

        [Fact]
        public async Task OpenSessionDetails_DoesNot_Navigate_To_SessionDetailsView_When_Rink_IsNot_Selected()
        {
            const string rinkId = "TrackId";
            _sut.SelectedSession = new SessionViewModel(new SessionDto
            {
                AccountId = AccountId,
                Id = Guid.NewGuid().ToString("N"),
                IsCompleted = false,
                RinkId = rinkId
            }, new[] {new TrackDto
                {
                    Id = rinkId,
                    Name = "TrackName",
                    Start = default,
                    Finish = default
                }
            },_dataSyncService);

            await _sut.OpenDetailsCommand.ExecuteAsync();

            await _navigationService.DidNotReceive().NavigateToViewModelAsync<SessionDetailsViewModel>();
        }

        [Fact]
        public async Task OpenSessionDetails_DoesNot_Navigate_To_SessionDetailsView_When_Session_IsNot_Selected()
        {
            await _sut.OpenDetailsCommand.ExecuteAsync();

            await _navigationService.DidNotReceive().NavigateToViewModelAsync<SessionDetailsViewModel>();
        }

        [Fact]
        public async Task OpenSessionDetails_SelectsRink_By_SelectedSession_RinkName()
        {
            const string rinkId = "TrackId";
            const string rinkName = "TrackName";
            _sut.SelectedSession = new SessionViewModel(new SessionDto
            {
                AccountId = AccountId,
                Id = Guid.NewGuid().ToString("N"),
                IsCompleted = false,
                RinkId = rinkId
            }, new[] {new TrackDto
                {
                    Id = rinkId,
                    Name = rinkName,
                    Start = default,
                    Finish = default
                }
            },_dataSyncService);

            await _sut.OpenDetailsCommand.ExecuteAsync();

            _trackService.Received(1).SelectRinkByName(rinkName);
        }

        [Fact]
        public void CanOpenSessionDetails_When_Session_Is_Selected_And_Has_RinkName()
        {
            const string rinkId = "TrackId";
            const string rinkName = "TrackName";
            _sut.SelectedSession = new SessionViewModel(new SessionDto
            {
                AccountId = AccountId,
                Id = Guid.NewGuid().ToString("N"),
                IsCompleted = false,
                RinkId = rinkId
            }, new[] {new TrackDto
                {
                    Id = rinkId,
                    Name = rinkName,
                    Start = default,
                    Finish = default
                }
            },_dataSyncService);

            _sut.CanOpenSessionDetails.Should().BeTrue();
        }

        [Fact]
        public void CanNotOpenSessionDetails_When_Session_Is_Selected_But_RinkName_Is_Unknown()
        {
            _sut.SelectedSession = new SessionViewModel(new SessionDto
            {
                AccountId = AccountId,
                Id = Guid.NewGuid().ToString("N"),
                IsCompleted = false,
                RinkId = "dd"
            }, new[] {new TrackDto
                {
                    Id = "rinkId",
                    Name = "rinkName",
                    Start = default,
                    Finish = default
                }
            },_dataSyncService);

            _sut.CanOpenSessionDetails.Should().BeFalse();
        }

        [Fact]
        public void CanNotOpenSessionDetails_When_Session_Is_NotSelected()
        {
            _sut.CanOpenSessionDetails.Should().BeFalse();
        }

        [Fact]
        public void Updates_CanOpenDetails_When_Session_Is_Selected()
        {
            var canOpenDetailsUpdated = false;
            _sut.AttachHandlers();
            _sut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_sut.CanOpenSessionDetails))
                    canOpenDetailsUpdated = true;
            };
            _sut.SelectedSession = new SessionViewModel(CreatSessions().First(), new List<TrackDto>(),_dataSyncService);

            canOpenDetailsUpdated.Should().BeTrue();
        }
    }
}
