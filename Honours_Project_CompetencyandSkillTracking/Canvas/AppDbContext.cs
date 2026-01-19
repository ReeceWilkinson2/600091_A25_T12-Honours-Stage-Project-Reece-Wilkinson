using Microsoft.EntityFrameworkCore;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Assignment> Assignments => Set<Assignment>();
        //public DbSet<Submission> Submissions => Set<Submission>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}
