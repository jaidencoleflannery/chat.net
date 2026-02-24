namespace chat.net.Commands;

public record Input : Command{ 
    public string Text => _text;
    private string _text;

   public Input(string input) {
       _text = input;
   }
}
