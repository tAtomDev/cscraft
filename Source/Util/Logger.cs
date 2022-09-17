namespace Util;
internal class Logger {
    private static readonly object _syncObject = new object();

    private static void _Log(string level, string message, ConsoleColor color, bool resetColor = true) {
        lock(_syncObject) {
            Console.ForegroundColor = color;
            Console.Write($"[{level}] ");
            if (resetColor) Console.ResetColor();
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    public static void Log(string message) {
        _Log("LOG", message, ConsoleColor.White);
    }

    public static void Info(string message) {
        _Log("INFO", message, ConsoleColor.Blue);
    }

    public static void Info2(string message) {
        _Log("INFO", message, ConsoleColor.Cyan);
    }

    public static void Success(string message) {
        _Log("SUCCESS", message, ConsoleColor.Green);
    }

    public static void Success2(string message) {
        _Log("SUCCESS", message, ConsoleColor.DarkGreen);
    }

    public static void LogMainInfo(string message) {
        _Log("MAIN", message, ConsoleColor.DarkMagenta, false);
    }

    public static void Warn(string message) {
        _Log("WARNING", message, ConsoleColor.Yellow, false);
    }

    public static void Error(string message) {
        _Log("ERROR", message, ConsoleColor.Red, false);
    }
}