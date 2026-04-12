using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public class CSVReading
    {
        private readonly AppDbContext _dbContext;
        public CSVReading(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // what to ignore:
        // VCO Seqn
        // Scheme
        // Collaborative Indicator
        // VCO Start Year
        // VCO End Year
        // Online Module Choice
        // CBO Block
        // CBO Occ
        // Block Type
        // Current Online Diet
        // Online Diet Errors
        // Projected 2023/24 Diet
        // 2023/04 Diet Translation
        // 2023/24 Diet Errors
        // Diet User Help
        // Diet Element Seqn
        // FMC Code
        // Mod Code
        // MAV Name
        // Assessment Pattern
        // Diet Element User Help

        public List<ModuleData> ReadModules()
        {
            var Modules = new List<ModuleData>();

            var FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data","CSV Files", "ProgrammeConstructionReport_25.06.csv");

            if (!File.Exists(FilePath))
                throw new FileNotFoundException($"CSV file not found at: {FilePath}");

            var Lines = File.ReadAllLines(FilePath);

            if (Lines.Length == 0)
                return Modules;

            var HeaderValues = ParseCsvLine(Lines[0]);

            var ColumnIndex = new Dictionary<string, int>();

            for (int i = 0; i < HeaderValues.Count; i++)
            {
                var Header = HeaderValues[i].Trim();
                if (!ColumnIndex.ContainsKey(Header))
                {
                    ColumnIndex.Add(Header, i);
                }
            }

            // Process data rows
            for (int i = 1; i < Lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(Lines[i]))
                    continue; // skip blank lines

                var Values = ParseCsvLine(Lines[i]);

                string GetValue(string columnName)
                {
                    if (ColumnIndex.ContainsKey(columnName))
                    {
                        int Index = ColumnIndex[columnName];

                        if (Index < Values.Count)
                            return Values[Index].Trim();
                    }
                    return ""; // Default if missing
                }

                var Module = new ModuleData
                {
                    Course = GetValue("Course"),
                    Programme = GetValue("Programme"),
                    Title = GetValue("Title"),
                    ModuleName = GetValue("MAV_Name"),
                    Route = GetValue("Route"),
                    Award = GetValue("Award"),
                    CBOYear = GetValue("CBO_Year"),
                    Trimester = GetValue("Trimester"),
                    SelectionStatus = GetValue("Selection_Status"),
                    Level = GetValue("Level"),
                    Credits = GetValue("Credits"),
                    ModCode = GetValue("Mod Code"),
                    VideoLink = GetValue("Video_URLs")
                };
                Modules.Add(Module);
            }
            return Modules;
        }

        public List<CompetencyData> ReadCompetencies()
        {
            var competencies = new List<CompetencyData>();

            var filePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "CSV Files",
                "Competencies.csv"
            );

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found at: {filePath}");

            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
                return competencies;

            var headerValues = ParseCsvLine(lines[0]);

            var columnIndex = new Dictionary<string, int>();

            for (int i = 0; i < headerValues.Count; i++)
            {
                var header = headerValues[i].Trim();
                if (!columnIndex.ContainsKey(header))
                    columnIndex.Add(header, i);
            }

            string GetValue(List<string> values, string columnName)
            {
                if (columnIndex.ContainsKey(columnName))
                {
                    int index = columnIndex[columnName];
                    if (index < values.Count)
                        return values[index].Trim();
                }
                return "";
            }

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);

                var competency = new CompetencyData
                {
                    CompetencyID = GetValue(values, "CompetencyID"),
                    CompetencyName = GetValue(values, "CompetencyName"),
                    AdditionalNotes = GetValue(values, "AdditionalNotes")
                };

                competencies.Add(competency);
            }

            return competencies;
        }

        public List<CompetencyLevels> ReadCompetencyLevels(List<CompetencyData> competencies)
        {
            var competencyLevels = new List<CompetencyLevels>();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data","CSV Files","CompetencyLevels.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found at: {filePath}");

            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
                return competencyLevels;

            var headerValues = ParseCsvLine(lines[0]);

            var columnIndex = new Dictionary<string, int>();

            for (int i = 0; i < headerValues.Count; i++)
            {
                var header = headerValues[i].Trim();
                if (!columnIndex.ContainsKey(header))
                    columnIndex.Add(header, i);
            }

            string GetValue(List<string> values, string columnName)
            {
                if (columnIndex.ContainsKey(columnName))
                {
                    int index = columnIndex[columnName];
                    if (index < values.Count)
                        return values[index].Trim();
                }
                return "";
            }

            var competencyLookup = competencies.Where(c => !string.IsNullOrWhiteSpace(c.CompetencyID)).ToDictionary(c => c.CompetencyID.Trim(), c => c);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);

                string competencyId = (GetValue(values, "CompetencyID") ?? "").Trim().Replace("\r", "").Replace("\n", "");
                string modCode = GetValue(values, "ModCode");

                if (string.IsNullOrWhiteSpace(competencyId) || string.IsNullOrWhiteSpace(modCode))
                {
                    Console.WriteLine($"Missing CompetencyID or ModCode at row {i}");
                    continue;
                }

                if (!competencyLookup.TryGetValue(competencyId, out var competency))
                {
                    Console.WriteLine($"Competency not found for ID {competencyId} at row {i}");
                    continue;
                }

                if (!int.TryParse(GetValue(values, "LevelNumber"), out int levelNumber))
                {
                    Console.WriteLine($"Invalid LevelNumber at row {i}");
                    continue;
                }

                var competencyLevel = new CompetencyLevels
                {
                    LevelNumber = levelNumber,
                    Description = GetValue(values, "Description"),
                    CompetencyDbID = competency.CompetencyDbID,
                    Competency = competency
                };

                var modCodes = modCode.Split('-');

                foreach (var mod in modCodes)
                {
                    var trimmedMod = mod.Trim();
                    var module = _dbContext.ModulesCSV.FirstOrDefault(m => m.ModCode == trimmedMod);

                    if (module == null)
                    {
                        Console.WriteLine($"Module {trimmedMod} not found");
                        continue;
                    }
                    competencyLevel.Modules.Add(module);
                }

                competencyLevels.Add(competencyLevel);
            }

            return competencyLevels;
        }

        public List<User> ReadUsers()
        {
            var users = new List<User>();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data", "CSV Files", "User CSVs", "Users.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return users;

            var map = BuildHeaderMap(ParseCsvLine(lines[0]));
            var seen = new HashSet<string>();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);

                var studentId = GetValue(values, map, "StudentId") ?? GetValue(values, map, "StudentID");

                studentId = studentId.Trim();

                if (string.IsNullOrWhiteSpace(studentId))
                    continue;

                if (!seen.Add(studentId))
                    continue;

                users.Add(new User
                {
                    StudentId = studentId,
                    UserName = GetValue(values, map, "UserName"),
                    StEmail = GetValue(values, map, "StEmail"),
                    Password = GetValue(values, map, "Password"),
                    Role = GetValue(values, map, "Role")
                });
            }

            return users;
        }

        public List<Course> ReadCourses(List<User> users)
        {
            var courses = new List<Course>();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data", "CSV Files", "User CSVs", "StudentCourses.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return courses;

            var map = BuildHeaderMap(ParseCsvLine(lines[0]));

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);

                var studentId = GetValue(values, map, "StudentId")?.Trim();

                if (string.IsNullOrWhiteSpace(studentId))
                    continue;

                var user = users.FirstOrDefault(u => u.StudentId == studentId);
                if (user == null)
                {
                    Console.WriteLine($"User with StudentId {studentId} not found for course {GetValue(values, map, "Name")}");
                    continue;
                }

                var course = new Course
                {
                    Name = GetValue(values, map, "Name"),
                    Code = GetValue(values, map, "Code"),
                    Syllabus = GetValue(values, map, "Syllabus"),
                    Term = GetValue(values, map, "Term"),
                    StudentId = studentId,
                    Student = user
                };

                courses.Add(course);
            }

            return courses;
        }


        private static List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            bool inQuotes = false;
            var currentValue = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentValue.Append(c);
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue.ToString().Trim());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }
            values.Add(currentValue.ToString().Trim());
            return values;
        }

        private Dictionary<string, int> BuildHeaderMap(List<string> headers)
        {
            return headers
                .Select((h, i) => new { h = h.Trim(), i })
                .GroupBy(x => x.h)
                .ToDictionary(g => g.Key, g => g.First().i);
        }

        private string GetValue(List<string> values, Dictionary<string, int> map, string column)
        {
            return map.TryGetValue(column, out int index) && index < values.Count ? values[index].Trim()
                : "";
        }
    }
}