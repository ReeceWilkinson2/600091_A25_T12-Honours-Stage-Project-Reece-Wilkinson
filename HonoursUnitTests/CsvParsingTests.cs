using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Xunit;

namespace HonoursUnitTests;

public class CSVReadingTests
{
    private string CreateTempDirWithFile(string folderName, string relativePath, string fileName, string content)
    {
        var root = Path.Combine(Path.GetTempPath(), folderName);

        var fullDir = Path.Combine(root, relativePath);
        Directory.CreateDirectory(fullDir);

        var filePath = Path.Combine(fullDir, fileName);
        File.WriteAllText(filePath, content);

        return root;
    }

    [Fact]
    public void ReadModules_ReturnsModules_WhenCsvIsValid()
    {
        var csv =
@"Course,Programme,Title,MAV_Name,Route,Award,CBO_Year,Trimester,Selection_Status,Level,Credits,Mod Code,Video_URLs
CS,Computing,Intro,Module A,Full,BSc,2024,T1,Yes,4,15,CS101,link";

        var basePath = CreateTempDirWithFile(
            "csvtest_modules",
            "CSV Files",
            "ProgrammeConstructionReport_25.06.csv",
            csv
        );

        var service = new CSVReading(null!, basePath);

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

        var basePath = CreateTempDirWithFile(
            "csvtest_comp",
            "CSV Files",
            "Competencies.csv",
            csv
        );

        var service = new CSVReading(null!, basePath);

        var result = service.ReadCompetencies();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.CompetencyCode == "C1");
        Assert.Contains(result, c => c.CompetencyCode == "C2");
    }

    [Fact]
    public void ReadUsers_IgnoresDuplicateStudents()
    {
        var csv =
@"StudentId,UserName,StEmail,Password,Role
123,John,john@email.com,pass,Student
123,JohnDuplicate,john2@email.com,pass,Student
456,Alice,alice@email.com,pass,Student";

        var basePath = CreateTempDirWithFile(
            "csvtest_users",
            "CSV Files/User CSVs",
            "Users.csv",
            csv
        );

        var service = new CSVReading(null!, basePath);

        var result = service.ReadUsers();

        Assert.Equal(2, result.Count); // NOT Single anymore
        Assert.Contains(result, u => u.StudentId == "123");
        Assert.Contains(result, u => u.StudentId == "456");
    }
}