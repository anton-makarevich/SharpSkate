using System.IO;
using System.Linq;
using System.Reflection;

namespace PreCommitHooks.Checks
{
    public class SecureKeysCheck:IPreCommitCheck
    {
        public bool CanCommit()
        {
            const string apiFilePath = "Shared/SmartSkating.Dto/ApiNames.cs";
            var dllPath = Assembly.GetExecutingAssembly().Location;
            const string solutionName = "SmartSkating";
            var solutionPath = Path.Combine(dllPath.Split(solutionName).First(), solutionName);
            var apiFileInSolutionPath = Path.Combine(solutionPath, apiFilePath);

            var apiFilesLines = File.ReadAllLines(apiFileInSolutionPath);
            return apiFilesLines
                .Select(line => line.Trim())
                .Count(trimmedLine => trimmedLine.StartsWith("public static string AzureApiSubscriptionKey")
                                                                  && trimmedLine.Contains("SSS_AZURE_API_KEY")) == 1;
        }
    }
}