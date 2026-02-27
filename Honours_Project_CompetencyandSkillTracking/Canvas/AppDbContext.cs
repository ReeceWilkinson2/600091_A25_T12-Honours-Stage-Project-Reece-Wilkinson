using Microsoft.EntityFrameworkCore;
using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Students => Set<User>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<ModuleData> ModulesCSV => Set<ModuleData>();
        public DbSet<Assignment> Assignments => Set<Assignment>();
        public DbSet<Submission> Submissions => Set<Submission>();
        public DbSet<Outcome> Outcomes => Set<Outcome>();
        public DbSet<OutcomeResult> OutcomeResults => Set<OutcomeResult>();

        public DbSet<UserData> UserData => Set<UserData>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}


