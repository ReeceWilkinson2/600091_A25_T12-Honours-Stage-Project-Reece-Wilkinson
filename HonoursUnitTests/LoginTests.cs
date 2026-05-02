using Bunit;
using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Components.Pages;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HonoursUnitTests;

public class SignInTests : IDisposable
{
    private readonly TestContext _ctx;

    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IAuthStateService> _authStateMock = new();

    public SignInTests()
    {
        _ctx = new TestContext();

        _ctx.Services.AddSingleton(_userServiceMock.Object);
        _ctx.Services.AddSingleton(_authStateMock.Object);

        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();
        _ctx.Services.AddSingleton(localStorageMock.Object);
    }

    [Fact]
    public void SignInPageRendersCorrectly()
    {
        var cut = _ctx.Render<SignIn>(); 

        Assert.Contains("Login", cut.Markup);
        cut.Find("input#email");
        cut.Find("input#password");
        cut.Find("button");
        cut.Find("a[href='/PasswordReset']");
        //cut.Find("a[href='/signup']");
    }

    [Fact]
    public void LoginFails_ShowsErrorMessage()
    {
        _userServiceMock
            .Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var cut = _ctx.Render<SignIn>();

        cut.Find("input#email").Change("wrong@example.com");
        cut.Find("input#password").Change("wrongpassword");
        cut.Find("button").Click();

        Assert.Contains("Invalid email or password", cut.Markup);
    }

    [Fact]
    public void LoginSucceeds_NavigatesAndSetsAuthState()
    {
        var nav = _ctx.Services.GetRequiredService<NavigationManager>();

        var testUser = new User
        {
            StudentId = "123",
            Role = "Student"
        };

        _userServiceMock
            .Setup(s => s.AuthenticateAsync("test@example.com", "password"))
            .ReturnsAsync(testUser);

        var cut = _ctx.Render<SignIn>();

        cut.Find("input#email").Change("test@example.com");
        cut.Find("input#password").Change("password");
        cut.Find("button").Click();

        Assert.EndsWith("/home", nav.Uri);

        _authStateMock.Verify(a => a.SetUserAsync("123", "Student"), Times.Once);
    }

    public void Dispose()
    {
        _ctx.Dispose();
    }
}
