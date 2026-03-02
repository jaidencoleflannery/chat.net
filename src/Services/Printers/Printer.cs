using System.Text;

namespace chat.net.Printers;

public static class Printer {
    public static bool PrintToConsole(StringBuilder text) {
        try {
            Console.WriteLine("\n| [chat.net]:");
            Console.WriteLine($"{text.ToString()}");
            return true;
        } catch (Exception exception){
            throw new IOException($"Failed to print.", exception);
        }
    }
}
