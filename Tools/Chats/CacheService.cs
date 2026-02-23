namespace chat.net.Caching;

public class CacheService {
    public static CacheService? Instance => _instance;
    private static CacheService? _instance;

    List<string> inputCache = new();

    private CacheService() { }

    public static CacheService Init() {
        if(_instance != null)
            throw new InvalidOperationException("CacheService has already been initialized.");
        return _instance = new CacheService();
    }

    public bool Store(string input) {
        if(string.IsNullOrEmpty(input.Trim()))
            throw new ArgumentException($"{nameof(input)} is empty.");
        inputCache.Add(input);
        return true;
    }
}
