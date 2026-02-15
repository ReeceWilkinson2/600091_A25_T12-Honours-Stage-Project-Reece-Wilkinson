namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public class AuthStateService
    {
        public string? UserId { get; private set; }
        public string? Role { get; private set; }

        public event Action? OnChange;

        public void SetUser(string userId, string role)
        {
            UserId = userId;
            Role = role;
            OnChange?.Invoke();
        }

        public void Clear()
        {
            UserId = null;
            Role = null;
            OnChange?.Invoke();
        }
    }

}