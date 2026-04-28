using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public class CSVReading
    {
        private readonly AppDbContext _dbContext;
        private readonly string _basePath;
        public CSVReading(AppDbContext dbContext, string? basePath = null)
        {
            _dbContext = dbContext;
            _basePath = basePath ?? AppDomain.CurrentDomain.BaseDirectory;
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

            var FilePath = Path.Combine(_basePath, "Data", "CSV Files", "ProgrammeConstructionReport_25.06.csv");

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
                    Credits = int.TryParse(GetValue("Credits"), out int credits) ? credits : 0,
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

            var filePath = Path.Combine(_basePath, "Data", "CSV Files", "Competencies.csv");

            ValidateFile(filePath);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return competencies;

            var map = BuildHeaderMap(ParseCsvLine(lines[0]));

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var values = ParseCsvLine(lines[i]);

                var code = GetValue(values, map, "CompetencyID");

                if (string.IsNullOrWhiteSpace(code))
                {
                    Console.WriteLine("Skipping competency with empty ID");
                    continue;
                }

                competencies.Add(new CompetencyData
                {
                    CompetencyCode = code.Trim(),
                    CompetencyName = GetValue(values, map, "CompetencyName"),
                    AdditionalNotes = GetValue(values, map, "AdditionalNotes")
                });
            }
            Console.WriteLine($"Loaded Competencies: {competencies.Count}");
            return competencies;
        }

        public List<CompetencyLevels> ReadCompetencyLevels(List<CompetencyData> competencies)
        {
            var levels = new List<CompetencyLevels>();

            var filePath = Path.Combine(_basePath, "Data", "CSV Files", "CompetencyLevels.csv");

            ValidateFile(filePath);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return levels;

            var map = BuildHeaderMap(ParseCsvLine(lines[0]));

            var competencyLookup = competencies
                .ToDictionary(c => c.CompetencyCode.Trim().ToLower(), c => c);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var values = ParseCsvLine(lines[i]);

                var competencyCode = GetValue(values, map, "CompetencyID").Trim().ToLower();

                if (!competencyLookup.TryGetValue(competencyCode, out var competency))
                {
                    Console.WriteLine($"No match for CompetencyID: '{competencyCode}'");
                    continue;
                }

                if (!int.TryParse(GetValue(values, map, "LevelNumber"), out int levelNumber))
                {
                    Console.WriteLine("Invalid LevelNumber");
                    continue;
                }

                levels.Add(new CompetencyLevels
                {
                    LevelNumber = levelNumber,
                    Description = GetValue(values, map, "Description"),
                    CompetencyDbID = competency.CompetencyDbID,
                    Competency = competency
                });
            }
            Console.WriteLine($"Loaded Levels: {levels.Count}");
            return levels;
        }

        public List<CompetencyLevelModule> ReadCompetencyLevelModules(List<CompetencyLevels> levels, List<ModuleData> modules)
        {
            var result = new List<CompetencyLevelModule>();

            var filePath = Path.Combine(_basePath, "Data", "CSV Files", "CompetencyLevelModules.csv");

            ValidateFile(filePath);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return result;

            var map = BuildHeaderMap(ParseCsvLine(lines[0]));

            var levelLookup = levels.ToDictionary(l => l.LevelDbID);
            var moduleLookup = modules.GroupBy(m => m.ModCode.Trim()).ToDictionary(g => g.Key, g => g.First());

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var values = ParseCsvLine(lines[i]);

                if (!int.TryParse(GetValue(values, map, "CompetencyLevelId"), out int levelId))
                    continue;

                var moduleCode = GetValue(values, map, "ModuleId").Trim();

                if (!levelLookup.TryGetValue(levelId, out var level))
                {
                    Console.WriteLine($"Level not found: {levelId}");
                    continue;
                }

                if (!moduleLookup.TryGetValue(moduleCode, out var module))
                {
                    Console.WriteLine($"Module not found: '{moduleCode}'");
                    continue;
                }

                result.Add(new CompetencyLevelModule
                {
                    CompetencyLevelId = level.LevelDbID,
                    CompetencyLevel = level,
                    ModuleId = module.DatabaseID,
                    Module = module
                });
            }
            Console.WriteLine($"Loaded Level-Module links: {result.Count}");
            return result;
        }

        public List<User> ReadUsers()
        {
            var users = new List<User>();

            var filePath = Path.Combine(_basePath, "Data", "CSV Files", "User CSVs", "Users.csv");

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

            var filePath = Path.Combine(_basePath, "Data", "CSV Files", "User CSVs", "StudentCourses.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return courses;

            var map = BuildHeaderMap(ParseCsvLine(lines[0]));

            var userLookup = users.ToDictionary(u => u.StudentId.Trim().ToLowerInvariant());

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);

                var studentId = GetValue(values, map, "StudentId")
                    ?.Trim()
                    .ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(studentId))
                    continue;

                if (!userLookup.TryGetValue(studentId, out var user))
                {
                    Console.WriteLine($"User not found for StudentId {studentId}");
                    continue;
                }

                if (!long.TryParse(GetValue(values, map, "Id"), out long id))
                {
                    Console.WriteLine($"Invalid Course Id at row {i + 1}");
                    continue;
                }

                courses.Add(new Course
                {
                    Id = id,
                    Name = GetValue(values, map, "Name"),
                    Code = GetValue(values, map, "Code"),
                    Syllabus = GetValue(values, map, "Syllabus"),
                    Term = GetValue(values, map, "Term"),
                    StudentId = studentId,
                    Student = user
                });
                foreach (var c in courses)
                {
                    var extracted = ExtractCourseCode(c.Code);

                    if (string.IsNullOrWhiteSpace(extracted))
                    {
                        Console.WriteLine($"Failed to extract from: '{c.Code}'");
                    }
                }
            }
            Console.WriteLine($"Courses Loaded: {courses.Count}");
            return courses;
        }

        public List<Assignment> ReadAssignments(List<Course> courses)
        {
            var assignments = new List<Assignment>();

            var filePath = Path.Combine(_basePath, "Data", "CSV Files", "User CSVs", "StudentAssignments.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found at: {filePath}");

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return assignments;

            var map = BuildHeaderMap(ParseCsvLine(lines[0]));

            var courseLookup = courses
                .Select(c => new
                {
                    Course = c,
                    Code = ExtractCourseCode(c.Code)
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .GroupBy(x => x.Code)
                .ToDictionary(g => g.Key, g => g.First().Course);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);

                var courseKey = GetValue(values, map, "CourseId")?.Trim();

                if (!courseLookup.TryGetValue(courseKey, out var course))
                {
                    Console.WriteLine($"No match for CourseId: '{courseKey}'");

                    // Debug
                    Console.WriteLine("Available keys:");
                    foreach (var key in courseLookup.Keys.Take(10))
                    {
                        Console.WriteLine($" - '{key}'");
                    }
                    continue;
                }

                if (!long.TryParse(GetValue(values, map, "Id"), out long id))
                {
                    Console.WriteLine($"Invalid Assignment Id at row {i + 1}");
                    continue;
                }

                assignments.Add(new Assignment
                {
                    Id = id,
                    CourseId = course.Id,
                    Name = GetValue(values, map, "Name"),
                    Description = GetValue(values, map, "Description"),
                    DueAt = ParseDate(GetValue(values, map, "DueAt")),
                    Course = course,
                    Weighting = int.TryParse(GetValue(values, map, "Weighting"), out int w) ? w : (int?)null
                });
            }

            Console.WriteLine($"Loaded Assignments: {assignments.Count}");
            return assignments;
        }

        public List<Submission> ReadSubmissions(List<Assignment> assignments, List<User> users)
        {
            var submissions = new List<Submission>();

            var filePath = Path.Combine(_basePath, "Data", "CSV Files","User CSVs", "StudentSubmissions.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return submissions;

            var map = BuildHeaderMap(ParseCsvLine(lines[0]));

            var assignmentLookup = assignments.ToDictionary(a => a.Id);

            var userLookup = users.ToDictionary(
                u => u.StudentId.Trim().ToLowerInvariant()
            );

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);

                if (!long.TryParse(GetValue(values, map, "Id"), out long id))
                    continue;

                if (!long.TryParse(GetValue(values, map, "AssignmentId"), out long assignmentId))
                {
                    Console.WriteLine($"Invalid AssignmentId at row {i + 1}");
                    continue;
                }

                var studentId = GetValue(values, map, "StudentId")
                    ?.Trim()
                    .ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(studentId))
                {
                    Console.WriteLine($"Empty StudentId at row {i + 1}");
                    continue;
                }

                if (!assignmentLookup.TryGetValue(assignmentId, out var assignment))
                {
                    Console.WriteLine($"Missing Assignment {assignmentId}");
                    continue;
                }

                if (!userLookup.TryGetValue(studentId, out var user))
                {
                    Console.WriteLine($"Missing User {studentId}");
                    continue;
                }

                submissions.Add(new Submission
                {
                    Id = id,
                    AssignmentId = assignmentId,
                    Assignment = assignment,
                    StudentId = studentId,
                    Student = user,
                    Score = double.TryParse(GetValue(values, map, "Score"), out var s) ? s : null,
                    SubmittedAt = ParseDateTime(GetValue(values, map, "SubmittedAt"))
                });
            }

            Console.WriteLine($"Loaded Submissions: {submissions.Count}");
            return submissions;
        }
        public List<SubmissionComment> ReadSubmissionComments(List<Submission> submissions)
        {
            var comments = new List<SubmissionComment>();

            var filePath = Path.Combine(_basePath, "Data", "CSV Files", "User CSVs", "StudentSubmissionComments.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found at: {filePath}");

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return comments;

            var map = BuildHeaderMap(ParseCsvLine(lines[0]));

            var submissionLookup = submissions.ToDictionary(s => s.Id);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);

                if (!long.TryParse(GetValue(values, map, "Id"), out long id))
                    continue;

                if (!long.TryParse(GetValue(values, map, "SubmissionId"), out long submissionId))
                    continue;

                if (!submissionLookup.TryGetValue(submissionId, out var submission))
                {
                    Console.WriteLine($"Submission not found: {submissionId}");
                    continue;
                }

                var commentText = GetValue(values, map, "Comment");
                var author = GetValue(values, map, "AuthorName");

                DateTime? createdAt = null; // must be DateTime
                var createdRaw = GetValue(values, map, "CreatedAt");

                if (DateTime.TryParseExact(
                    createdRaw,
                    "dd/MM/yyyy HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var parsedDate))
                {
                    createdAt = parsedDate;
                }

                comments.Add(new SubmissionComment
                {
                    Id = id,
                    SubmissionId = submissionId,
                    Submission = submission,
                    Comment = commentText,
                    AuthorName = author,
                    CreatedAt = createdAt
                });
            }
            Console.WriteLine($"Loaded Submission Comments: {comments.Count}");
            return comments;
        }

        public List<CompetencyAchievement> ReadCompetencyAchievements(List<User> users, List<CompetencyLevels> levels, List<ModuleData> modules, List<Submission> submissions)
        {
            var results = new List<CompetencyAchievement>();

            var filePath = Path.Combine(_basePath, "Data", "CSV Files", "User CSVs", "StudentCompetencyAchievements.csv");

            ValidateFile(filePath);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length == 0) return results;

            var map = BuildHeaderMap(ParseCsvLine(lines[0]));

            var userLookup = users.ToDictionary(u => u.StudentId.Trim().ToLower());
            var levelLookup = levels.ToDictionary(l => l.LevelDbID);

            var duplicates = modules.GroupBy(m => m.ModCode).Where(g => g.Count() > 1).ToList();

            foreach (var d in duplicates)
            {
                Console.WriteLine($"Duplicate ModCode: {d.Key} → {d.Count()} entries");
            }

            var moduleLookup = modules
                .GroupBy(m => m.ModCode.Trim())
                .ToDictionary(
                    g => g.Key,
                    g => g.First()
                );

            var submissionLookup = submissions.ToDictionary(s => s.Id);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);

                if (!int.TryParse(GetValue(values, map, "Id"), out int id))
                    continue;

                var studentId = GetValue(values, map, "StudentId")?.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(studentId) || !userLookup.TryGetValue(studentId, out var user))
                {
                    Console.WriteLine($"❌ Missing User: {studentId}");
                    continue;
                }

                if (!int.TryParse(GetValue(values, map, "CompetencyLevelId"), out int levelId) ||
                    !levelLookup.TryGetValue(levelId, out var level))
                {
                    Console.WriteLine($"❌ Missing CompetencyLevel: {levelId}");
                    continue;
                }

                var rawModule = GetValue(values, map, "ModuleId")?.Trim();

                if (string.IsNullOrWhiteSpace(rawModule) ||
                    !moduleLookup.TryGetValue(rawModule, out var module))
                {
                    Console.WriteLine($"❌ Module not found: '{rawModule}'");
                    continue;
                }

                if (!int.TryParse(GetValue(values, map, "MasteryPoints"), out int masteryPoints))
                    masteryPoints = 0;

                DateTime achievedDate = DateTime.Now;
                var rawDate = GetValue(values, map, "AchievedDate");

                if (!DateTime.TryParseExact(
                        rawDate,
                        "dd/MM/yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out achievedDate))
                {
                    Console.WriteLine($"⚠️ Invalid date: {rawDate}");
                }

                Submission? submission = null;
                long? submissionId = null;

                var rawSubmission = GetValue(values, map, "SubmissionId");

                if (long.TryParse(rawSubmission, out var subId))
                {
                    if (submissionLookup.TryGetValue(subId, out var sub))
                    {
                        submission = sub;
                        submissionId = subId;
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Submission not found: {subId}");
                    }
                }

                results.Add(new CompetencyAchievement
                {
                    Id = id,
                    StudentId = studentId,
                    User = user,

                    CompetencyLevelId = level.LevelDbID,
                    CompetencyLevel = level,

                    MasteryPoints = masteryPoints,
                    AchievedDate = achievedDate,

                    ModuleId = module.DatabaseID,
                    Module = module,

                    SubmissionId = submissionId,
                    Submission = submission
                });
            }

            Console.WriteLine($"Loaded CompetencyAchievements: {results.Count}");
            return results;
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
                .Select((h, i) => new
                {
                    h = h.Trim()
                          .Trim('\uFEFF')
                          .ToLowerInvariant(),
                    i
                })
                .GroupBy(x => x.h)
                .ToDictionary(g => g.Key, g => g.First().i);
        }

        private string GetValue(List<string> values, Dictionary<string, int> map, string column)
        {
            column = column.Trim().ToLowerInvariant();

            return map.TryGetValue(column, out int index) && index < values.Count
                ? values[index].Trim()
                : "";
        }

        private void ValidateFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"CSV file not found: {path}");
        }

        private DateTimeOffset? ParseDate(string input)
        {
            if (DateTimeOffset.TryParseExact(
                input,
                "dd/MM/yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var result))
                return result;

            return null;
        }

        private DateTime? ParseDateTime(string input)
        {
            if (DateTime.TryParseExact(
                input,
                "dd/MM/yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var result))
                return result;

            return null;
        }

        private string ExtractCourseCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            input = input.Trim();

            // Take everything before the first underscore
            var underscoreIndex = input.IndexOf('_');

            if (underscoreIndex > 0)
                return input.Substring(0, underscoreIndex).Trim();

            return "";
        }
    }
}