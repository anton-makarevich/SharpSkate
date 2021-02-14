using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Services.Account;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Account
{
    public class AccountServiceTest
    {
        private readonly IPreferences _preferences = Substitute.For<IPreferences>();
        private readonly IDeviceInfo _deviceInfo = Substitute.For<IDeviceInfo>();

        private readonly AccountService _sut;

        public AccountServiceTest()
        {
            _sut = new AccountService(_preferences, _deviceInfo);
        }

        [Fact]
        public void UserId_Is_Always_AntonM_Until_Login_Is_Implemented()
        {
            var userId = _sut.UserId;
            userId.Should().Be("AntonM");
        }

        [Fact]
        public void Generates_New_DeviceId_When_It_Is_Unknown()
        {
            _preferences.Get("deviceId", "").Returns("");

            var deviceInfo =  _sut.GetDeviceInfo();

            deviceInfo.Id.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Returns_DeviceId_Saved_In_Preferences()
        {
            const string savedId = "savedId";
            _preferences.Get("deviceId", "").Returns(savedId);

            var deviceInfo =  _sut.GetDeviceInfo();

            deviceInfo.Id.Should().Be(savedId);
        }

        [Fact]
        public void DeviceId_Has_AccountId_Equal_To_UserIs()
        {
            var userId = _sut.UserId;
            var deviceInfo = _sut.GetDeviceInfo();

            deviceInfo.AccountId.Should().Be(userId);
        }

        [Fact]
        public void Returns_DeviceInfo_From_The_Service()
        {
            const string manufacturer = "manufacturer";
            const string model = "model";
            const string osName = "osName";
            const string osVersion = "version";

            _deviceInfo.Manufacturer.Returns(manufacturer);
            _deviceInfo.Model.Returns(model);
            _deviceInfo.Platform.Returns(osName);
            _deviceInfo.Version.Returns(osVersion);

            var deviceInfo = _sut.GetDeviceInfo();

            deviceInfo.Manufacturer.Should().Be(manufacturer);
            deviceInfo.Model.Should().Be(model);
            deviceInfo.OsName.Should().Be(osName);
            deviceInfo.OsVersion.Should().Be(osVersion);
        }

        [Fact]
        public void Getting_DeviceId_Calls_Preferences_Only_Once()
        {
            _preferences.Get("deviceId", "").Returns("id");

            var id1 = _sut.DeviceId;
            var id2 = _sut.DeviceId;

            id1.Should().Be(id2);
            _preferences.Received(1).Get(Arg.Any<string>(),Arg.Any<string>());
        }
    }
}
