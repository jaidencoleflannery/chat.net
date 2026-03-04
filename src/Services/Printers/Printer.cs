using System.Text;

namespace chat.net.Printers;

public static class Printer {
    public static void PrintToConsole(StringBuilder text) {
        Console.WriteLine("\n| [chat.net]:");
        Console.WriteLine($"| {text}");
    }
}
