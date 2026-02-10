using Microsoft.EntityFrameworkCore;
using Honours_Project_CompetencyandSkillTracking.Canvas;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Assignment> Assignments => Set<Assignment>();
        public DbSet<Submission> Submissions => Set<Submission>();
        public DbSet<UserData> UserData => Set<UserData>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}


