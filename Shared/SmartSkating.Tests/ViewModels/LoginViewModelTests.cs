using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services.Account;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class LoginViewModelTests
    {
        private readonly LoginViewModel _sut;
        private readonly ILoginService _loginService;
        private readonly IAccountService _accountService;
        private readonly INavigationService _navigationService;

        private const string Username = "someUser";
        private const string Password = "password";

        public LoginViewModelTests()
        {
            _navigationService = Substitute.For<INavigationService>();
            _loginService = Substitute.For<ILoginService>();
            _accountService = Substitute.For<IAccountService>();
            _sut = new LoginViewModel(_loginService,_accountService);
            _sut.SetNavigationService(_navigationService);
        }

        [Fact]
        public void InitialUsernameValueIsEmpty()
        {
            _sut.Username.Should().BeEmpty();
        }

        [Fact]
        public void InitialPasswordValueIsEmpty()
        {
            _sut.Password.Should().BeEmpty();
        }

        [Fact]
        public void CannotLogin_WhenUsernameIsEmpty()
        {
            _sut.CanLogin.Should().BeFalse();
        }

        [Fact]
        public void UpdatesCanLogin_WhenUsernameChanges()
        {
            var canLoginUpdatedTimes =0;
            _sut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_sut.CanLogin))
                {
                    canLoginUpdatedTimes++;
                }
            };
            _sut.Username = Username;
            canLoginUpdatedTimes.Should().Be(1);
        }

        [Fact]
        public void CannotLogin_WhenUsernameIsNotEmptyButPasswordIsEmpty()
        {
            _sut.Username = Username;
            _sut.CanLogin.Should().BeFalse();
        }

        [Fact]
        public void UpdatesCanLogin_WhenPasswordChanges()
        {
            var canLoginUpdatedTimes =0;
            _sut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_sut.CanLogin))
                {
                    canLoginUpdatedTimes++;
                }
            };
            _sut.Password = Password;
            canLoginUpdatedTimes.Should().Be(1);
        }

        [Fact]
        public void CanLogin_WhenUserNameAndPasswordAreNotEmpty()
        {
            _sut.Password = Username;
            _sut.Username = Password;
            _sut.CanLogin.Should().BeTrue();
        }

        [Fact]
        public async Task CallsLoginService_WhenLoginCommandIsExecuted()
        {
            _sut.Username = Username;
            _sut.Password = Password;

            _sut.LoginCommand.Execute(null);

            await _loginService.Received().LoginUserAsync(Username,Password);
        }

        [Fact]
        public async Task DoesNotCallLoginService_WhenLoginCommandIsExecuted_ButCannotLogin()
        {
            _sut.LoginCommand.Execute(null);

            await _loginService.DidNotReceive().LoginUserAsync(Arg.Any<string>(),Arg.Any<string>());
        }

        [Fact]
        public async Task NavigatesToSessionsView_WhenLoginIsSuccessful()
        {
            _sut.Username = Username;
            _sut.Password = Password;
            _loginService.LoginUserAsync(Username,Password)
                .Returns(Task.FromResult(new AccountDto
                {
                    Id = "someId"
                }));

            _sut.LoginCommand.Execute(null);

            await _navigationService.Received().NavigateToViewModelAsync<SessionsViewModel>();
        }

        [Fact]
        public void Set_UserId_OnAccountService_WhenLoginIsSuccessful()
        {
            _sut.Username = Username;
            _sut.Password = Password;
            _loginService.LoginUserAsync(Username,Password)
                .Returns(Task.FromResult(new AccountDto
                {
                    Id = "someId"
                }));

            _sut.LoginCommand.Execute(null);

            _accountService.Received(1).UserId = _sut.Username;
        }

        [Fact]
        public void ShowsErrorMessage_WhenLoginIsNotSuccessful()
        {
            _sut.Username = Username;
            _sut.Password = Password;
            _loginService.LoginUserAsync(Username,Password)
                .Returns(Task.FromResult<AccountDto>(null));

            _sut.LoginCommand.Execute(null);

            _sut.ValidationMessage.Should().Be(LoginViewModel.CheckCredentialsMessage);
        }

        [Fact]
        public void ShowsErrorMessage_WhenCannotLogin()
        {
            _loginService.LoginUserAsync(Username,Password)
                .Returns(Task.FromResult(new AccountDto
                {
                    Id = "someId"
                }));

            _sut.LoginCommand.Execute(null);

            _sut.ValidationMessage.Should().Be(LoginViewModel.CheckCredentialsMessage);
        }

        [Fact]
        public void ResetsValidationMessage_WhenUsernameIsChanged()
        {
            _loginService.LoginUserAsync(Username,Password)
                .Returns(Task.FromResult(new AccountDto
                {
                    Id = "someId"
                }));

            _sut.LoginCommand.Execute(null);

            _sut.Username = Username;

            _sut.ValidationMessage.Should().BeEmpty();
        }

        [Fact]
        public void ResetsValidationMessage_WhenPasswordIsChanged()
        {
            _loginService.LoginUserAsync(Username,Password)
                .Returns(Task.FromResult(new AccountDto
                {
                    Id = "someId"
                }));

            _sut.LoginCommand.Execute(null);

            _sut.Password = Password;

            _sut.ValidationMessage.Should().BeEmpty();
        }

        [Fact]
        public void UpdatesCanLogin_WhenPageIsLoaded()
        {
            var canLoginUpdatedTimes =0;
            _sut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_sut.CanLogin))
                {
                    canLoginUpdatedTimes++;
                }
            };
            _sut.AttachHandlers();
            canLoginUpdatedTimes.Should().Be(1);
        }
    }
}
