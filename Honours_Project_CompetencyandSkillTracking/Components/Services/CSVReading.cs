using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public class CSVReading
    {
        private readonly AppDbContext _dbContext;
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

            // Build dictionary: column name -> index
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

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data","CSV Files","Competencies.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found at: {filePath}");

            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
                return competencies;

            var headerValues = ParseCsvLine(lines[0]);

            // Build column index map
            var columnIndex = new Dictionary<string, int>();

            for (int i = 0; i < headerValues.Count; i++)
            {
                var header = headerValues[i].Trim();
                if (!columnIndex.ContainsKey(header))
                {
                    columnIndex.Add(header, i);
                }
            }

            // Helper function
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

            // Process rows
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

        public List<CompetencyLevels> ReadCompetencyLevels()
        {
            var competencyLevels = new List<CompetencyLevels>();

            var filePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "CSV Files",
                "CompetencyLevels.csv" // Ensure this is the correct path to your CSV
            );

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found at: {filePath}");

            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
                return competencyLevels;

            var headerValues = ParseCsvLine(lines[0]);

            // Build column index map
            var columnIndex = new Dictionary<string, int>();

            for (int i = 0; i < headerValues.Count; i++)
            {
                var header = headerValues[i].Trim();
                if (!columnIndex.ContainsKey(header))
                {
                    columnIndex.Add(header, i);
                }
            }

            // Helper function to get the value of a column
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

            // Process rows (competency levels)
            // Process rows (competency levels)
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = ParseCsvLine(lines[i]);

                // Extract values from the current row
                string competencyId = GetValue(values, "CompetencyID");
                string modCode = GetValue(values, "ModCode");

                // Check if the competencyId and modCode are valid
                if (string.IsNullOrWhiteSpace(competencyId) || string.IsNullOrWhiteSpace(modCode))
                {
                    Console.WriteLine($"CompetencyID or ModCode is missing or malformed at row {i}. Skipping this row.");
                    continue; // Skip this row if there's no valid CompetencyID or ModCode
                }

                try
                {
                    // Trim the CompetencyID to avoid spaces causing issues
                    competencyId = competencyId.Trim();

                    // Find the corresponding Competency in the database
                    var competency = _dbContext.CompetencyData
                        .FirstOrDefault(c => c.CompetencyID == competencyId);

                    // Check if the competency is null
                    if (competency == null)
                    {
                        Console.WriteLine($"Competency not found for CompetencyID: {competencyId} at row {i}. Skipping this row.");
                        continue; // Skip if the competency doesn't exist in the database
                    }

                    // Log for debugging
                    Console.WriteLine($"Found Competency: {competency.CompetencyName} for CompetencyID: {competencyId}");

                    // Now, create the CompetencyLevel object
                    var competencyLevel = new CompetencyLevels
                    {
                        LevelNumber = int.Parse(GetValue(values, "LevelNumber")),
                        Description = GetValue(values, "Description"),
                        CompetencyDbID = competency.CompetencyDbID, // Link to the correct Competency
                    };

                    // Now associate this CompetencyLevel with its modules
                    var modCodes = modCode.Split('-'); // Split ModCode by dash if there are multiple codes

                    foreach (var mod in modCodes)
                    {
                        var trimmedMod = mod.Trim(); // Ensure no extra spaces

                        // Look for the module matching the trimmed ModCode
                        var module = _dbContext.ModulesCSV
                            .FirstOrDefault(m => m.ModCode == trimmedMod);

                        // If module is null, log and skip the module
                        if (module == null)
                        {
                            Console.WriteLine($"Module with ModCode {trimmedMod} not found in database. Skipping this module.");
                            continue; // Skip this module if not found in the database
                        }

                        // Log the module that is being added
                        Console.WriteLine($"Adding Module {module.ModuleName} with ModCode {module.ModCode} to CompetencyLevel {competencyLevel.LevelNumber}");

                        // Add module to the competency level
                        competencyLevel.Modules.Add(module);
                    }

                    // Add the competencyLevel to the list
                    competencyLevels.Add(competencyLevel);

                }
                catch (Exception ex)
                {
                    // Log the exact error
                    Console.WriteLine($"Error processing row {i} for CompetencyID: {competencyId} and ModCode: {modCode}. Error: {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");
                    // Optionally, you can skip this row or handle it further.
                    continue;  // Continue processing the next rows
                }
            }

            return competencyLevels;
        }

        private static List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            bool inQuotes = false;
            var currentValue = new StringBuilder();

            // Iterate through the characters in the line
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"') // Handle quoted text
                {
                    // If it's a quote, toggle the inQuotes flag
                    if (i + 1 < line.Length && line[i + 1] == '"') // Handle double quotes within quoted text
                    {
                        currentValue.Append(c);
                        i++; // Skip the next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes; // Toggle the flag
                    }
                }
                else if (c == ',' && !inQuotes) // Not in quotes and it's a comma, so it's a delimiter
                {
                    values.Add(currentValue.ToString().Trim());
                    currentValue.Clear();
                }
                else
                {
                    // Otherwise, append the character to the current value
                    currentValue.Append(c);
                }
            }

            // Add the last value
            values.Add(currentValue.ToString().Trim());

            return values;
        }
    }
}
