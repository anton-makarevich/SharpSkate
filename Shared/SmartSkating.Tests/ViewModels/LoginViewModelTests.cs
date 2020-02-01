using FluentAssertions;
using Sanet.SmartSkating.ViewModels;
using Xunit;

namespace Sanet.SmartSkating.Tests.ViewModels
{
    public class LoginViewModelTests
    {
        private readonly LoginViewModel _sut;

        public LoginViewModelTests()
        {
            _sut = new LoginViewModel();
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
    }
}