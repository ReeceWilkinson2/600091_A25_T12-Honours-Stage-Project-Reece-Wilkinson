using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Microsoft.EntityFrameworkCore;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public static class DbSeeder
    {
        public static async Task SeedTestData(AppDbContext context)
        {
            // Ensure the student exists
            var user = await context.Students.FirstOrDefaultAsync(u => u.StudentId == "12345");
            if (user == null)
            {
                user = new User
                {
                    StudentId = "12345",
                    UserName = "Test Student",
                    StEmail = "student@test.com",
                    Password = "test",
                    Role = "Student"
                };
                context.Students.Add(user);
                await context.SaveChangesAsync();
            }

            var competency = await context.CompetencyData.FirstOrDefaultAsync(c => c.CompetencyID == "COMP001");
            if (competency == null)
            {
                competency = new CompetencyData
                {
                    CompetencyName = "Programming",
                    CompetencyID = "COMP001",
                    AdditionalNotes = "Core programming skills"
                };
                context.CompetencyData.Add(competency);
                await context.SaveChangesAsync();
            }

            // Ensure levels exist (idempotent)
            var existingLevels = await context.CompetencyLevels
                .Where(l => l.CompetencyDbID == competency.CompetencyDbID)
                .ToListAsync();

            var existingLevelNumbers = existingLevels.Select(l => l.LevelNumber).ToHashSet();

            var levelsToAdd = new List<CompetencyLevels>();

            if (!existingLevelNumbers.Contains(4))
                levelsToAdd.Add(new CompetencyLevels
                {
                    LevelNumber = 4,
                    Description = "Basic understanding",
                    CompetencyDbID = competency.CompetencyDbID,
                    ModCode = "441101"
                });

            if (!existingLevelNumbers.Contains(5))
                levelsToAdd.Add(new CompetencyLevels
                {
                    LevelNumber = 5,
                    Description = "Intermediate application",
                    CompetencyDbID = competency.CompetencyDbID,
                    ModCode = "551462"
                });

            if (!existingLevelNumbers.Contains(6))
                levelsToAdd.Add(new CompetencyLevels
                {
                    LevelNumber = 6,
                    Description = "Advanced proficiency",
                    CompetencyDbID = competency.CompetencyDbID,
                    ModCode = "600091"
                });

            if (levelsToAdd.Any())
            {
                context.CompetencyLevels.AddRange(levelsToAdd);
                await context.SaveChangesAsync();
                existingLevels.AddRange(levelsToAdd);
            }

            var achievementsExist = await context.CompetencyAchievements
                .Where(a => a.StudentId == user.StudentId)
                .Select(a => a.CompetencyLevelId)
                .ToListAsync();

            var achievementsToAdd = new List<CompetencyAchievement>();

            foreach (var level in existingLevels)
            {
                if ((level.LevelNumber == 4 || level.LevelNumber == 5) && !achievementsExist.Contains(level.LevelDbID))
                {
                    var daysAgo = level.LevelNumber == 4 ? -10 : -5;  // Set dates for Level 4 and Level 5

                    achievementsToAdd.Add(new CompetencyAchievement
                    {
                        User = user,
                        StudentId = user.StudentId,
                        CompetencyLevel = level,
                        CompetencyLevelId = level.LevelDbID,
                        AchievedDate = DateTime.Now.AddDays(daysAgo)
                    });
                }
            }

            if (achievementsToAdd.Any())
            {
                context.CompetencyAchievements.AddRange(achievementsToAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}