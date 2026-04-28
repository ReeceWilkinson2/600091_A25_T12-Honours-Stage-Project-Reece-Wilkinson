using Blazored.LocalStorage;
using Bunit;
using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Components.Pages;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

public class AuthStateServiceTests
{
    private readonly Mock<ILocalStorageService> _localStorageMock;
    private readonly AuthStateService _service;

    public AuthStateServiceTests()
    {
        _localStorageMock = new Mock<ILocalStorageService>();
        _service = new AuthStateService(_localStorageMock.Object);
    }

    [Fact]
    public async Task InitializeAsync_LoadsUserFromLocalStorage()
    {
        _localStorageMock.Setup(x => x.GetItemAsync<string>("UserId", default))
            .ReturnsAsync("123");

        _localStorageMock.Setup(x => x.GetItemAsync<string>("UserRole", default))
            .ReturnsAsync("Student");

        await _service.InitializeAsync();

        Assert.Equal("123", _service.UserId);
        Assert.Equal("Student", _service.Role);
        Assert.True(_service.IsInitialized);
    }

    [Fact]
    public async Task SetUserAsync_SavesToLocalStorage_AndUpdatesState()
    {
        await _service.SetUserAsync("456", "Admin");

        Assert.Equal("456", _service.UserId);
        Assert.Equal("Admin", _service.Role);
        Assert.True(_service.IsInitialized);

        _localStorageMock.Verify(x => x.SetItemAsync("UserId", "456", default), Times.Once);
        _localStorageMock.Verify(x => x.SetItemAsync("UserRole", "Admin", default), Times.Once);
    }

    [Fact]
    public async Task ClearAsync_RemovesUserFromStateAndStorage()
    {
        await _service.SetUserAsync("123", "Student");
        await _service.ClearAsync();

        Assert.Null(_service.UserId);
        Assert.Null(_service.Role);

        _localStorageMock.Verify(x => x.RemoveItemAsync("UserId", default), Times.Once);
        _localStorageMock.Verify(x => x.RemoveItemAsync("UserRole", default), Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_DoesNotRunTwice()
    {
        _localStorageMock.Setup(x => x.GetItemAsync<string>(It.IsAny<string>(), default))
            .ReturnsAsync("value");

        await _service.InitializeAsync();
        await _service.InitializeAsync();

        _localStorageMock.Verify(x => x.GetItemAsync<string>("UserId", default), Times.Once);
    }
}
