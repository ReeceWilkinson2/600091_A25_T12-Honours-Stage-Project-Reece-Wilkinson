using Bunit;
using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Components.Pages;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HonoursUnitTests;

public class CSVReadingTests
{
    private string CreateTempCsv(string fileName, string content)
    {
        var path = Path.Combine(Path.GetTempPath(), fileName);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void ReadModules_ReturnsModules_WhenCsvIsValid()
    {
        var csv =
@"Course,Programme,Title,MAV_Name,Route,Award,CBO_Year,Trimester,Selection_Status,Level,Credits,Mod Code,Video_URLs
CS,Computing,Intro,Module A,Full,BSc,2024,T1,Yes,4,15,CS101,link";

        var path = CreateTempCsv("modules.csv", csv);

        var service = new CSVReading(null!);

        AppDomain.CurrentDomain.SetData("APPBASE", Path.GetDirectoryName(path));

        var result = service.ReadModules();

        // Assert
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

        var path = CreateTempCsv("competencies.csv", csv);
        AppDomain.CurrentDomain.SetData("APPBASE", Path.GetDirectoryName(path));

        var service = new CSVReading(null!);

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
123,JohnDuplicate,john2@email.com,pass,Student";

        var path = CreateTempCsv("users.csv", csv);
        AppDomain.CurrentDomain.SetData("APPBASE", Path.GetDirectoryName(path));

        var service = new CSVReading(null!);

        var result = service.ReadUsers();

        Assert.Single(result);
        Assert.Equal("123", result[0].StudentId);
    }
}
