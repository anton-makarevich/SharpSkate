using System.IO;
using System.Linq;
using System.Reflection;

namespace ProjectHelpers.Services
{
    public class ProjectFilesService:IProjectFilesService
    {
        public string GetProjectFilePath(string fileName)
        {
            var dllPath = Assembly.GetExecutingAssembly().Location;
            var solutionPath = Path.Combine(dllPath.Split(SolutionConstants.SolutionName).First(), 
                    SolutionConstants.SolutionName);
            return Path.Combine(solutionPath, fileName);
        }

        public string ReadProjectFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public string[] ReadProjectFileLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public void SaveProjectFile(string filePath, string fileContent)
        {
            File.WriteAllText(filePath,fileContent);
        }
    }
}