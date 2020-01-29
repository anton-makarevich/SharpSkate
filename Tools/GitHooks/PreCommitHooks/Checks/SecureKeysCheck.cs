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
                .Where(trimmedLine => trimmedLine.StartsWith("public static readonly string AzureApiSubscriptionKey"))
                .All(trimmedLine => trimmedLine.EndsWith("<Ocp-Apim-Subscription-Key>\";"));
        }
    }
}