using System.ComponentModel.DataAnnotations;

namespace Honours_Project_CompetencyandSkillTracking.Classes
{
    public class SignUpForm
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        //[HullEmailOnly]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string StaffPassword { get; set; }
    }
}
