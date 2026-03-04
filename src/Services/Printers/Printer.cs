using System.Text;

namespace chat.net.Printers;

public static class Printer {
    public static void PrintToConsole(StringBuilder text) {
        if(text.Length > 1) {
            Console.WriteLine("\n[chat.net]:\n");
            Console.WriteLine($"{text}");
        }
    }
}
