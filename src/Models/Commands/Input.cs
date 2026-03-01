namespace chat.net.Models;

public record Input : Command{ 
    public string Text => _text;
    private string _text;

   public InputCommand(string input) {
       _text = input;
   }
}
