using System.Text;
using chat.net.Models;

using static chat.net.Conversations.ResponseBuilder;
using static chat.net.Printers.Printer;
using static chat.net.Parsers.ResponseParser;

namespace chat.net.Conversations;

public static class ResponseService {
    public static void PrintResult(ResponseDto result) =>
        PrintToConsole(ParseResponse(result));

    public static void HandleException(Exception exception, bool debug) =>
        PrintToConsole(BuildException(exception, debug));
}
