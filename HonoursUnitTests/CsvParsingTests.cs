using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Xunit;

namespace HonoursUnitTests;

public class CSVReadingTests
{
    private string CreateCsvStructure(string root, string relativePath, string fileName, string content)
    {
        var fullPath = Path.Combine(root, "Data", relativePath);

        Directory.CreateDirectory(fullPath);

        var filePath = Path.Combine(fullPath, fileName);
        File.WriteAllText(filePath, content);

        return root;
    }

    [Fact]
    public void ReadModules_ReturnsModules_WhenCsvIsValid()
    {
        var csv =
@"Course,Programme,Title,MAV_Name,Route,Award,CBO_Year,Trimester,Selection_Status,Level,Credits,Mod Code,Video_URLs
CS,Computing,Intro,Module A,Full,BSc,2024,T1,Yes,4,15,CS101,link";

        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        CreateCsvStructure(
            root,
            "CSV Files",
            "ProgrammeConstructionReport_25.06.csv",
            csv
        );

        var service = new CSVReading(null!, root);

        var result = service.ReadModules();

        Assert.Single(result);
        Assert.Equal("CS101", result[0].ModCode);
    }

    [Fact]
    public void ReadCompetencies_ReturnsCompetencies_WhenCsvValid()
    {
        var csv =
@"CompetencyID,CompetencyName,AdditionalNotes
C1,Problem Solving,Note 1
C2,Team Work,Note 2";

        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        CreateCsvStructure(
            root,
            "CSV Files",
            "Competencies.csv",
            csv
        );

        var service = new CSVReading(null!, root);

        var result = service.ReadCompetencies();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.CompetencyCode == "C1");
    }

    [Fact]
    public void ReadUsers_IgnoresDuplicateStudents()
    {
        var csv =
@"StudentId,UserName,StEmail,Password,Role
123,John,john@email.com,pass,Student
123,JohnDuplicate,john2@email.com,pass,Student
456,Alice,alice@email.com,pass,Student";

        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        CreateCsvStructure(
            root,
            "CSV Files/User CSVs",
            "Users.csv",
            csv
        );

        var service = new CSVReading(null!, root);

        var result = service.ReadUsers();

        Assert.Equal(2, result.Count);
    }
}