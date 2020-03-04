using System;
using FluentAssertions;
using Sanet.SmartSkating.Backend.Azure.Models;
using Sanet.SmartSkating.Dto.Models;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Models
{
    public class SessionEntityTests
    {
        [Fact]
        public void CanBeCreatedFromDto()
        {
            var dto = new SessionDto
            {
                Id = "id",
                AccountId = "accountId",
                IsCompleted = true,
                IsSaved = true
            };
            var sut = new SessionEntity(dto);

            sut.IsCompleted.Should().BeTrue();
            sut.PartitionKey.Should().Be(dto.AccountId);
            sut.RowKey.Should().Be(dto.Id);
        }
        
        [Fact]
        public void CanBeExportedToDto()
        {
            var sut = new SessionEntity
            {
                ETag = "tag",
                PartitionKey = "accountId",
                RowKey = "id",
                Timestamp = DateTimeOffset.Now,
                IsCompleted = true
            };

            var dto = sut.ToDto();

            dto.IsCompleted.Should().BeTrue();
            dto.IsSaved.Should().BeTrue();
            dto.AccountId.Should().Be(sut.PartitionKey);
            dto.Id.Should().Be(sut.RowKey);
        }
    }
}