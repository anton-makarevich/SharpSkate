using System;
using System.Data;
using KeysProvider.Services;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using ProjectHelpers;
using ProjectHelpers.Services;
using Xunit;

namespace KeysProviderTests.Services
{
    public class SecretKeysServiceTests
    {
        private readonly SecretKeysService _sut;
        private readonly IProjectFilesService _projectFilesServiceMock;
        private readonly IConfiguration _configurationMock;
        
        private const string ApiNamesFileNameStub = "pathToApiNamesFile";

        public SecretKeysServiceTests()
        {
            _projectFilesServiceMock = Substitute.For<IProjectFilesService>();
            _configurationMock = Substitute.For<IConfiguration>();
            _projectFilesServiceMock.GetProjectFilePath(SolutionConstants.FileApiNames)
                .Returns(ApiNamesFileNameStub);
            _sut = new SecretKeysService(_projectFilesServiceMock,_configurationMock);
        }
        
        [Fact]
        public void Throws_WhenConfigDoesNotContainApiKeyValue()
        {
            Assert.Throws<NoNullAllowedException>(() => _sut.SetSecrets());
        }

        [Fact]
        public void Throws_WhenFileDoesNotContainPlaceholder()
        {
            _configurationMock[SolutionConstants.EnvAzureApiKey].Returns("someValue");
            _projectFilesServiceMock.ReadProjectFile(ApiNamesFileNameStub).Returns("noPlaceholder");


            Assert.Throws<Exception>(() => _sut.SetSecrets());
        }
        
        [Fact]
        public void ReplacesPlaceholder_WithApiKeyValueFromConfig()
        {
            const string apiKeyValue = "someValue";
            _configurationMock[SolutionConstants.EnvAzureApiKey].Returns(apiKeyValue);
            _projectFilesServiceMock
                .ReadProjectFile(ApiNamesFileNameStub)
                .Returns($"this is {SolutionConstants.PlaceholderAzureApiKey}");

            _sut.SetSecrets();
            
            _projectFilesServiceMock
                .Received()
                .SaveProjectFile(ApiNamesFileNameStub,$"this is {apiKeyValue}");
        }
    }
}