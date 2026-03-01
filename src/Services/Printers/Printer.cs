using System.Text;

namespace chat.net.Printers;

public static class Printer {
    public static bool PrintToConsole(StringBuilder text) {
        try {
            Console.WriteLine("\n");
            Console.WriteLine("[chat.net]:");
            Console.WriteLine(text.ToString());
            Console.WriteLine("\n");
            return true;
        } catch (Exception exception){
            throw new IOException($"Failed to print.", exception);
        }
    }
}
