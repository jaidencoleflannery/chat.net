namespace chat.net.Models;

public class ProviderNotSetException : Exception {
  public ProviderNotSetException(string message) : base(message) { }
}