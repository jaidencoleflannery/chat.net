namespace chat.net.Models;

public class ProviderNotSetException : Exception {
  public ProviderNotSetException() : base($"provider has not been set.") { }
}