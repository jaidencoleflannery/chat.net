namespace chat.net.Models;

public record InputCommand : Command{ 
    public string Text => _text;
    private string _text;

   public InputCommand(string input) {
       _text = input;
   }
}
