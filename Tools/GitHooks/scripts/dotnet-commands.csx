#load "logger.csx"
#load "command-line.csx"
public class DotnetCommands {
    public static int RunChecks() => ExecuteCommand("dotnet /Users/amakarevich/Documents/SmartSkating/.git/hooks/PreCommitHooks.dll");
    public static int BuildCode() => ExecuteCommand("dotnet build");
    public static int TestCode() => ExecuteCommand("dotnet test");
    private static int ExecuteCommand(string command) {
        string response = CommandLine.Execute(command);
        Int32.TryParse(response, out int exitCode);
        return exitCode;
    }
}