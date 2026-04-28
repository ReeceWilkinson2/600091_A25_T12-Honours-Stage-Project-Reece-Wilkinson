using Blazored.LocalStorage;
using Bunit;
using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Components.Pages;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

public class CompetencyDataServicesTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private CompetencyDataServices CreateService(AppDbContext context)
    {
        return new CompetencyDataServices(context);
    }

    [Fact]
    public async Task AddCompetencyDataAsync_AddsCompetency()
    {
        var context = CreateContext();
        var service = CreateService(context);

        var competency = new CompetencyData
        {
            CompetencyDbID = 1,
            CompetencyCode = "C1",
            CompetencyName = "Problem Solving"
        };

        var result = await service.AddCompetencyDataAsync(competency);

        Assert.Equal(1, context.CompetencyData.Count());
        Assert.Equal("C1", result.CompetencyCode);
    }

    [Fact]
    public async Task GetCompetencyByIdAsync_ReturnsCorrectCompetency()
    {
        var context = CreateContext();
        context.CompetencyData.Add(new CompetencyData
        {
            CompetencyDbID = 1,
            CompetencyCode = "C1",
            CompetencyName = "Problem Solving"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetCompetencyByIdAsync("C1");

        Assert.NotNull(result);
        Assert.Equal("C1", result!.CompetencyCode);
    }

    [Fact]
    public async Task GetCompetenciesByModuleAsync_ReturnsMatchingCompetencies()
    {
        var context = CreateContext();

        var competency = new CompetencyData
        {
            CompetencyDbID = 1,
            CompetencyCode = "C1",
            CompetencyName = "Problem Solving",
            Levels = new List<CompetencyLevels>
            {
                new CompetencyLevels
                {
                    LevelDbID = 10,
                    Modules = new List<CompetencyLevelModule>
                    {
                        new CompetencyLevelModule
                        {
                            Module = new ModuleData { ModCode = "MOD1" }
                        }
                    }
                }
            }
        };

        context.CompetencyData.Add(competency);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetCompetenciesByModuleAsync("MOD1");

        Assert.Single(result);
    }

    [Fact]
    public async Task UpdateCompetencyDataAsync_UpdatesFields()
    {
        var context = CreateContext();

        var competency = new CompetencyData
        {
            CompetencyDbID = 1,
            CompetencyCode = "C1",
            CompetencyName = "Old Name",
            AdditionalNotes = "Old"
        };

        context.CompetencyData.Add(competency);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var updated = new CompetencyData
        {
            CompetencyDbID = 1,
            CompetencyCode = "C1",
            CompetencyName = "New Name",
            AdditionalNotes = "New"
        };

        var result = await service.UpdateCompetencyDataAsync(updated);

        Assert.Equal("New Name", result.CompetencyName);
        Assert.Equal("New", result.AdditionalNotes);
    }

    [Fact]
    public async Task DeleteCompetencyDataAsync_RemovesEntity()
    {
        var context = CreateContext();

        var competency = new CompetencyData
        {
            CompetencyDbID = 1,
            CompetencyCode = "C1"
        };

        context.CompetencyData.Add(competency);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        await service.DeleteCompetencyDataAsync(competency);

        Assert.Empty(context.CompetencyData);
    }
}
