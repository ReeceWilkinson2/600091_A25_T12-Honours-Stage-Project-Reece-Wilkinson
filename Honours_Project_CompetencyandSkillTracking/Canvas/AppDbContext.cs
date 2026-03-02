using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Microsoft.EntityFrameworkCore;
using static Honours_Project_CompetencyandSkillTracking.Data.CompetencyLevels;

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

        public DbSet<CompetencyData> CompetencyData { get; set; }
        public DbSet<CompetencyLevels> CompetencyLevels { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}


