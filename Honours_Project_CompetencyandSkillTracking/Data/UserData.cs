using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class UserData
    {
        [Key]
        public int UserId { get; set; }
        public string? UserUsername { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }
        public bool UserEmailConfirmed { get; set; }
        public string? UserTheme { get; set; }
        public string? UserRole { get; set; }
    }
}
