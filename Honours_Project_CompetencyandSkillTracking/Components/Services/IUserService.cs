using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Data;

namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> AddUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(string userId);
        Task<List<ModuleData>> GetModuleDataAsync(string courseTitle, string level);
        Task<List<User>> GetStudentDataAsync(string role);
        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<string> GenerateUniqueStudentIdAsync();
        Task<List<Assignment>> GetAssignmentsByModuleCodeAsync(string modCode);
        Task<List<string>> GetAllCourseTitlesAsync();
    }
}