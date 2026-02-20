using Blazored.LocalStorage;

public class AuthStateService
{
    private readonly ILocalStorageService _localStorage;

    public string? UserId { get; private set; }
    public string? Role { get; private set; }
    public bool IsInitialized { get; private set; } = false;

    public event Action? OnChange;

    public AuthStateService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized) return;

        try
        {
            UserId = await _localStorage.GetItemAsync<string>("UserId");
            Role = await _localStorage.GetItemAsync<string>("UserRole");
        }
        catch
        {
            // JS interop not ready yet
        }

        IsInitialized = true;
        OnChange?.Invoke();
    }

    public async Task SetUserAsync(string userId, string role)
    {
        UserId = userId;
        Role = role;
        IsInitialized = true;
        await _localStorage.SetItemAsync("UserId", userId);
        await _localStorage.SetItemAsync("UserRole", role);
        OnChange?.Invoke();
    }

    public async Task ClearAsync()
    {
        UserId = null;
        Role = null;
        IsInitialized = true;
        await _localStorage.RemoveItemAsync("UserId");
        await _localStorage.RemoveItemAsync("UserRole");
        OnChange?.Invoke();
    }
}