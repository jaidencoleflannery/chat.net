namespace chat.net.Models;

public record Input : Command{ 
    public string Text => _text;
    private string _text;

   public Input(string input) {
       _text = input;
   }
}
