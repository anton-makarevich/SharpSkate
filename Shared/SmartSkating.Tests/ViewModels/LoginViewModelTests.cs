using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Dto.Services.Account;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class LoginViewModelTests
    {
        private readonly LoginViewModel _sut;
        private readonly ILoginService _loginService;

        public LoginViewModelTests()
        {
            _loginService = Substitute.For<ILoginService>();
            _sut = new LoginViewModel(_loginService);
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
            _sut.Username = "someName";
            canLoginUpdatedTimes.Should().Be(1);
        }
        
        [Fact]
        public void CannotLogin_WhenUsernameIsNotEmptyButPasswordIsEmpty()
        {
            _sut.Username = "someName";
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
            _sut.Password = "some";
            canLoginUpdatedTimes.Should().Be(1);
        }

        [Fact]
        public void CanLogin_WhenUserNameAndPasswordAreNotEmpty()
        {
            _sut.Password = "somePassword";
            _sut.Username = "someName";
            _sut.CanLogin.Should().BeTrue();
        }

        [Fact]
        public async Task CallsLoginService_WhenLoginCommandIsExecuted()
        {
            const string username = "someUser";
            const string password = "password";

            _sut.Username = username;
            _sut.Password = password;
            
            _sut.LoginCommand.Execute(null);

            await _loginService.Received().LoginUserAsync(username,password);
        }
    }
}