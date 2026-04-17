using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Microsoft.EntityFrameworkCore;
using static Honours_Project_CompetencyandSkillTracking.Data.CompetencyLevels;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<ModuleData> ModulesCSV => Set<ModuleData>();
        public DbSet<Assignment> Assignments => Set<Assignment>();
        public DbSet<Submission> Submissions => Set<Submission>();
        public DbSet<SubmissionComment> SubmissionComments => Set<SubmissionComment>();
        public DbSet<Outcome> Outcomes => Set<Outcome>();
        public DbSet<OutcomeResult> OutcomeResults => Set<OutcomeResult>();

        public DbSet<CompetencyData> CompetencyData { get; set; }
        public DbSet<CompetencyLevels> CompetencyLevels { get; set; }
        public DbSet<CompetencyLevelModule> CompetencyLevelModules { get; set; }
        public DbSet<CompetencyAchievement> CompetencyAchievements { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CompetencyLevels>()
                .HasOne(c => c.Competency)
                .WithMany(c => c.Levels)
                .HasForeignKey(c => c.CompetencyDbID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompetencyAchievement>()
                .HasOne(a => a.User)
                .WithMany(u => u.CompetencyAchievements)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompetencyAchievement>()
                .HasOne(a => a.CompetencyLevel)
                .WithMany(c => c.Achievements)
                .HasForeignKey(a => a.CompetencyLevelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompetencyLevelModule>()
                .HasKey(x => new { x.CompetencyLevelId, x.ModuleId });

            modelBuilder.Entity<CompetencyLevelModule>()
                .HasOne(x => x.CompetencyLevel)
                .WithMany(cl => cl.Modules)
                .HasForeignKey(x => x.CompetencyLevelId);

            modelBuilder.Entity<CompetencyLevelModule>()
                .HasOne(x => x.Module)
                .WithMany(m => m.CompetencyLevelModules)
                .HasForeignKey(x => x.ModuleId);

            modelBuilder.Entity<Assignment>()
                .Property(x => x.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Submission>()
                .Property(x => x.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<SubmissionComment>()
                .Property(x => x.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<CompetencyAchievement>()
                .HasOne(a => a.Submission)
                .WithMany(s => s.CompetencyAchievements)
                .HasForeignKey(a => a.SubmissionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}