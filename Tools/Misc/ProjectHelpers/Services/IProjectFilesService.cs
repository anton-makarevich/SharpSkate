namespace ProjectHelpers.Services
{
    public interface IProjectFilesService
    {
        string GetProjectFilePath(string fileName);

        string ReadProjectFile(string filePath);

        string[] ReadProjectFileLines(string filePath);
        void SaveProjectFile(string apiFileNamePath, string fileContent);
    }
}