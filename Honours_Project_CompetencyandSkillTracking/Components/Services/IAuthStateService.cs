namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public interface IAuthStateService
    {
        string? UserId { get; }
        string? Role { get; }
        bool IsInitialized { get; }
        event Action? OnChange;
        Task InitializeAsync();
        Task SetUserAsync(string userId, string role);
        Task ClearAsync();
    }
}
