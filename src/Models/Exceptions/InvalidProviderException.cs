namespace chat.net.Models;

public class InvalidProviderException : Exception {
  public InvalidProviderException(string message) : base(message) { }
}