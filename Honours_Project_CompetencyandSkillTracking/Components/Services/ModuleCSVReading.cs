using Honours_Project_CompetencyandSkillTracking.Data;

namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public class ModuleCSVReading
    {
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

            var FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data", "ProgrammeConstructionReport_25.06.csv");

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
                };
                Modules.Add(Module);
            }
            return Modules;
        }

        private static List<string> ParseCsvLine(string line)
        {
            var Values = new List<string>();
            bool Inquotes = false;
            var CurrentValue = new System.Text.StringBuilder();

            foreach (char c in line)
            {
                if (c == '"')
                {
                    Inquotes = !Inquotes;
                }
                else if (c == ',' && !Inquotes)
                {
                    Values.Add(CurrentValue.ToString().Trim());
                    CurrentValue.Clear();
                }
                else
                {
                    CurrentValue.Append(c);
                }
            }
            Values.Add(CurrentValue.ToString().Trim());

            return Values;
        }
    }
}
