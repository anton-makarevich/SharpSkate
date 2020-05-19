using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using ProjectHelpers;
using ProjectHelpers.Services;

namespace KeysProvider.Services
{
    public class SecretKeysService:ISecretKeysService
    {
        private readonly IProjectFilesService _projectFilesService;
        private readonly IConfiguration _config;

        public SecretKeysService(IProjectFilesService projectFilesService, IConfiguration config)
        {
            _projectFilesService = projectFilesService;
            _config = config;
        }
        public void SetSecrets()
        {
            var azureApiKeyValue = _config[SolutionConstants.EnvAzureApiKey];
            if (string.IsNullOrEmpty(azureApiKeyValue))
                throw new NoNullAllowedException($"Config var {SolutionConstants.EnvAzureApiKey} is not found");
            var apiFileNamePath = _projectFilesService.GetProjectFilePath(SolutionConstants.FileApiNames);
            var fileContent = _projectFilesService.ReadProjectFile(apiFileNamePath);
            if (!fileContent.Contains(SolutionConstants.PlaceholderAzureApiKey))
                throw new Exception($"Placeholder {SolutionConstants.PlaceholderAzureApiKey} not found");
            fileContent = fileContent.Replace(SolutionConstants.PlaceholderAzureApiKey, azureApiKeyValue);
            _projectFilesService.SaveProjectFile(apiFileNamePath, fileContent);
        }

        public void RemoveSecrets()
        {
            throw new System.NotImplementedException();
        }
    }
}