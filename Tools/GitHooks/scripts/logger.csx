public class Logger {
    public static void LogInfo(string message) {
        Console.Error.WriteLine(message);
    }
    public static void LogError(string message)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
        Console.ForegroundColor = color;
    }
}