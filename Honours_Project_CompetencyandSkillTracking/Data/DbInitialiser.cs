using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Microsoft.EntityFrameworkCore;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public static class DbSeeder
    {
        public static async Task SeedTestData(AppDbContext context)
        {
            //Console.WriteLine("Starting DB seeding...");

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
                //Console.WriteLine("User added.");
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
                //Console.WriteLine("Competency added.");
            }

            var levels = await context.CompetencyLevels.Where(l => l.CompetencyDbID == competency.CompetencyDbID).OrderBy(l => l.LevelNumber).ToListAsync();

            if (!levels.Any())
            {
                var level1 = new CompetencyLevels
                {
                    LevelNumber = 4,
                    Description = "Basic understanding",
                    Competency = competency,  // navigation property
                    ModCode = "441101"
                };
                var level2 = new CompetencyLevels
                {
                    LevelNumber = 5,
                    Description = "Intermediate application",
                    Competency = competency,
                    ModCode = "551462"
                };
                var level3 = new CompetencyLevels
                {
                    LevelNumber = 6,
                    Description = "Advanced proficiency",
                    Competency = competency,
                    ModCode = "600091"
                };

                context.CompetencyLevels.AddRange(level1, level2, level3);
                await context.SaveChangesAsync();

                levels = new List<CompetencyLevels> { level1, level2, level3 };
                //Console.WriteLine("Competency levels added.");
            }

            var achievementsExist = await context.CompetencyAchievements.AnyAsync(a => a.StudentId == user.StudentId);

            if (!achievementsExist)
            {
                var achievements = new List<CompetencyAchievement>
                {
                    new CompetencyAchievement
                    {
                        User = user,
                        StudentId = user.StudentId,
                        CompetencyLevel = levels[0],
                        CompetencyLevelId = levels[0].LevelDbID,
                        AchievedDate = DateTime.Now.AddDays(-10)
                    },
                    new CompetencyAchievement
                    {
                        User = user,
                        StudentId = user.StudentId,
                        CompetencyLevel = levels[1],
                        CompetencyLevelId = levels[1].LevelDbID,
                        AchievedDate = DateTime.Now.AddDays(-5)
                    },
                    new CompetencyAchievement
                    {
                        User = user,
                        StudentId = user.StudentId,
                        CompetencyLevel = levels[2],
                        CompetencyLevelId = levels[2].LevelDbID,
                        AchievedDate = DateTime.Now.AddDays(-1)
                    }
                };

                context.CompetencyAchievements.AddRange(achievements);
                await context.SaveChangesAsync();
                //Console.WriteLine("Competency achievements added.");
            }
            //Console.WriteLine("DB seeding completed successfully!");
        }
    }
}