using Honours_Project_CompetencyandSkillTracking.Data;
using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Microsoft.EntityFrameworkCore;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public static class DbSeeder
    {
        public static async Task SeedTestData(AppDbContext context)
        {
            var random = new Random();

            // 1️⃣ Seed test student if not exists
            var student = await context.Users
                .FirstOrDefaultAsync(u => u.StudentId == "12345");

            if (student == null)
            {
                student = new User
                {
                    StudentId = "12345",
                    UserName = "Test Student",
                    StEmail = "student@test.com",
                    Password = "test",
                    Role = "Student"
                };
                context.Users.Add(student);
                await context.SaveChangesAsync();
            }

            // 2️⃣ Seed a competency if missing
            var competency = await context.CompetencyData
                .Include(c => c.Levels)
                    .ThenInclude(l => l.Modules)
                .FirstOrDefaultAsync(c => c.CompetencyID == "COMP001");

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

            // 3️⃣ Seed competency levels if missing
            var existingLevels = await context.CompetencyLevels
                .Include(l => l.Modules)
                .Where(l => l.CompetencyDbID == competency.CompetencyDbID)
                .ToListAsync();

            var existingLevelNumbers = existingLevels.Select(l => l.LevelNumber).ToHashSet();
            var levelsToAdd = new List<CompetencyLevels>();

            if (!existingLevelNumbers.Contains(4))
                levelsToAdd.Add(new CompetencyLevels
                {
                    LevelNumber = 4,
                    Description = "Year 1 - Basic understanding",
                    CompetencyDbID = competency.CompetencyDbID
                });

            if (!existingLevelNumbers.Contains(5))
                levelsToAdd.Add(new CompetencyLevels
                {
                    LevelNumber = 5,
                    Description = "Year 2 - Intermediate application",
                    CompetencyDbID = competency.CompetencyDbID
                });

            if (!existingLevelNumbers.Contains(6))
                levelsToAdd.Add(new CompetencyLevels
                {
                    LevelNumber = 6,
                    Description = "Year 3 - Advanced proficiency",
                    CompetencyDbID = competency.CompetencyDbID
                });

            if (levelsToAdd.Any())
            {
                context.CompetencyLevels.AddRange(levelsToAdd);
                await context.SaveChangesAsync();
                existingLevels.AddRange(levelsToAdd);
            }

            // 4️⃣ Seed module assignments if missing
            var levelModuleMap = new Dictionary<int, List<string>>
    {
        { 4, new List<string> { "441101", "441102", "441104", "441105", "441108" } },
        { 5, new List<string> { "551462", "551457", "551460", "500083" } },
        { 6, new List<string> { "600091", "661985" } }
    };

            var allModuleCodes = levelModuleMap.SelectMany(x => x.Value).Distinct().ToList();
            var modules = await context.ModulesCSV.Where(m => allModuleCodes.Contains(m.ModCode)).ToListAsync();

            foreach (var level in existingLevels)
            {
                if (!levelModuleMap.TryGetValue(level.LevelNumber, out var moduleCodes)) continue;

                foreach (var code in moduleCodes)
                {
                    var module = modules.FirstOrDefault(m => m.ModCode == code);
                    if (module == null) continue;

                    if (!level.Modules.Any(m => m.ModCode == code))
                        level.Modules.Add(module);
                }
            }
            await context.SaveChangesAsync();

            // 5️⃣ Seed competency achievements if missing
            foreach (var level in existingLevels)
            {
                if (!level.Modules.Any()) continue;

                foreach (var module in level.Modules)
                {
                    var existingCount = await context.CompetencyAchievements.CountAsync(a =>
                        a.StudentId == student.StudentId &&
                        a.CompetencyLevelId == level.LevelDbID &&
                        a.ModuleId == module.DatabaseID);

                    if (existingCount == 0) // Only add if no achievements exist
                    {
                        // Set the year difference based on the level number
                        int yearDifference = level.LevelNumber - 4;

                        var competencyAchievement = new CompetencyAchievement
                        {
                            StudentId = student.StudentId,
                            User = student,
                            CompetencyLevelId = level.LevelDbID,
                            CompetencyLevel = level,
                            ModuleId = module.DatabaseID,
                            Module = module,
                            MasteryPoints = 6,
                            AchievedDate = DateTime.Now.AddYears(yearDifference) // Adjust date based on level
                        };
                        context.CompetencyAchievements.Add(competencyAchievement);
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}