using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sanet.SmartSkating.Backend.Azure.Services;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Services
{
    public class AzureLoginServiceTests
    {
        private readonly IDataService _dataService;
        private readonly AzureLoginService _sut;

        public AzureLoginServiceTests()
        {
            _dataService = Substitute.For<IDataService>();
            _sut = new AzureLoginService(_dataService);
        }

        [Fact]
        public async Task ReturnsAccount_WhenFoundSessionsForThisId()
        {
            const string accountId = "someId";
            var sessions = new List<SessionDto>
            {
                new SessionDto
                {
                    Id = "id",
                    AccountId = accountId,
                    IsCompleted = true,
                    IsSaved = true
                }
            };
            
            _dataService.GetAllSessionsForAccountAsync(accountId).Returns(Task.FromResult(sessions));

            var result = await _sut.LoginUserAsync(accountId, "password");

            result.Should().NotBeNull();
        }
        
        [Fact]
        public async Task ReturnsNull_WhenSessionsForThisIdAreNotFound()
        {
            const string accountId = "someId";
            var sessions = new List<SessionDto>();
            
            _dataService.GetAllSessionsForAccountAsync(accountId).Returns(Task.FromResult(sessions));

            var result = await _sut.LoginUserAsync(accountId, "password");

            result.Should().BeNull();
        }
    }
}