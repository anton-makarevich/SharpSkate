using System.Linq;
using ProjectHelpers;
using ProjectHelpers.Services;

namespace PreCommitHooks.Checks
{
    public class SecureKeysCheck:IPreCommitCheck
    {
        private readonly IProjectFilesService _projectFilesService;

        public SecureKeysCheck(IProjectFilesService projectFilesService)
        {
            _projectFilesService = projectFilesService;
        }
        public bool CanCommit()
        {
            var apiFileInSolutionPath = _projectFilesService.GetProjectFilePath(SolutionConstants.FileApiNames);

            var apiFilesLines = _projectFilesService.ReadProjectFileLines(apiFileInSolutionPath);
            return apiFilesLines
                .Select(line => line.Trim())
                .Count(trimmedLine => trimmedLine
                                          .StartsWith($"public const string {SolutionConstants.VarAzureApiKey}") 
                                      && trimmedLine.Contains(SolutionConstants.PlaceholderAzureApiKey)) == 1;
        }
    }
}