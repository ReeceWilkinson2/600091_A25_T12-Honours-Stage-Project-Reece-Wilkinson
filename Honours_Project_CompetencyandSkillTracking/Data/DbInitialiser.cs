using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Microsoft.EntityFrameworkCore;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public static class DbSeeder
    {
        public static async Task SeedTestData(AppDbContext context)
        {
            var user = await context.Students
                .FirstOrDefaultAsync(u => u.StudentId == "12345");

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

            var competency = context.CompetencyData
                .Include(c => c.Levels)
                    .ThenInclude(l => l.Modules)
                .FirstOrDefault(c => c.CompetencyID == "COMP001");

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

            var existingLevels = await context.CompetencyLevels
                .Include(l => l.Modules)
                .Where(l => l.CompetencyDbID == competency.CompetencyDbID)
                .ToListAsync();

            var existingLevelNumbers = existingLevels
                .Select(l => l.LevelNumber)
                .ToHashSet();

            var levelsToAdd = new List<CompetencyLevels>();

            if (!existingLevelNumbers.Contains(4))
            {
                levelsToAdd.Add(new CompetencyLevels
                {
                    LevelNumber = 4,
                    Description = "Basic understanding",
                    CompetencyDbID = competency.CompetencyDbID
                });
            }

            if (!existingLevelNumbers.Contains(5))
            {
                levelsToAdd.Add(new CompetencyLevels
                {
                    LevelNumber = 5,
                    Description = "Intermediate application",
                    CompetencyDbID = competency.CompetencyDbID
                });
            }

            if (!existingLevelNumbers.Contains(6))
            {
                levelsToAdd.Add(new CompetencyLevels
                {
                    LevelNumber = 6,
                    Description = "Advanced proficiency",
                    CompetencyDbID = competency.CompetencyDbID
                });
            }

            if (levelsToAdd.Any())
            {
                context.CompetencyLevels.AddRange(levelsToAdd);
                await context.SaveChangesAsync();
                existingLevels.AddRange(levelsToAdd);
            }

            var levelModuleMap = new Dictionary<int, List<string>>
            {
                { 4, new List<string> { "441101", "441105" } },
                { 5, new List<string> { "551462", "551457" } },
                { 6, new List<string> { "600091", "661985" } }
            };

            var allModuleCodes = levelModuleMap.SelectMany(x => x.Value).Distinct().ToList();

            var modules = await context.ModulesCSV.Where(m => allModuleCodes.Contains(m.ModCode)).ToListAsync();

            foreach (var level in existingLevels)
            {
                if (!levelModuleMap.TryGetValue(level.LevelNumber, out var moduleCodes))
                    continue;

                foreach (var code in moduleCodes)
                {
                    var module = modules.FirstOrDefault(m => m.ModCode == code);

                    if (module == null)
                        continue;

                    if (!level.Modules.Any(m => m.ModCode == code))
                    {
                        level.Modules.Add(module);
                    }
                }
            }
            await context.SaveChangesAsync();

            var achievementsExist = await context.CompetencyAchievements.Where(a => a.StudentId == user.StudentId).Select(a => a.CompetencyLevelId).ToListAsync();

            var achievementsToAdd = new List<CompetencyAchievement>();

            foreach (var level in existingLevels)
            {
                if ((level.LevelNumber == 4 || level.LevelNumber == 5) &&
                    !achievementsExist.Contains(level.LevelDbID))
                {
                    var daysAgo = level.LevelNumber == 4 ? -10 : -5;

                    achievementsToAdd.Add(new CompetencyAchievement
                    {
                        User = user,
                        StudentId = user.StudentId,
                        CompetencyLevel = level,
                        CompetencyLevelId = level.LevelDbID,
                        MasteryPoints = 6,
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