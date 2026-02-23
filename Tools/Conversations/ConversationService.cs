using System.Net.Http;

namespace chat.net.Conversations;

public static class ConversationService {
    public static string Call(string input) {
        // TODO: add Provider to config, and set up an enum for each type of provider so we
        // can grab their endpoint for our call.
        // we can leave Model to be free so it is future proof.
        using var client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync("")

    }
}
