using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Components.Pages;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Bunit;
using Moq;

namespace HonoursUnitTests;

public class SignInTests : BunitContext
{
    private readonly Mock<UserService> _mockUserService = new();
    private readonly Mock<AuthStateService> _mockAuthState = new();
    private readonly NavigationManager _navManager;

    public SignInTests()
    {
        // Get bUnit's built-in fake NavigationManager
        _navManager = Services.GetRequiredService<NavigationManager>();

        // Register mocked services
        Services.AddSingleton(_mockUserService.Object);
        Services.AddSingleton(_mockAuthState.Object);
    }

    [Fact]
    public void SignInPageRendersCorrectly()
    {
        // Act
        var cut = Render<SignIn>();

        // Assert page elements
        cut.Find("h1").MarkupMatches("<h1>Login</h1>");
        cut.Find("input#email");
        cut.Find("input#password");
        cut.Find("button").MarkupMatches("<button class=\"module-button\" type=\"submit\">Login</button>");
        cut.Find("a[href='/PasswordReset']");
        cut.Find("a[href='/signup']");
    }

    [Fact]
    public void LoginFails_ShowsErrorMessage() 
    {
        // Arrange: authentication fails
        _mockUserService
            .Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((User)null);

        var cut = Render<SignIn>();

        // Act: fill form and submit
        cut.Find("input#email").Change("wrong@example.com");
        cut.Find("input#password").Change("wrongpassword");
        cut.Find("button").Click();

        // Assert error message
        Assert.Equal("Invalid email or password", cut.Find(".text-danger").TextContent);
    }

    [Fact]
    public void LoginSucceeds_NavigatesToHome_AndSetsAuthState()
    {
        // Arrange: authentication succeeds
        var testUser = new User { StudentId = "123", Role = "Student" };

        _mockUserService
            .Setup(s => s.AuthenticateAsync("test@example.com", "password"))
            .ReturnsAsync(testUser);

        var cut = Render<SignIn>();

        // Act: fill form and submit
        cut.Find("input#email").Change("test@example.com");
        cut.Find("input#password").Change("password");
        cut.Find("button").Click();

        // Assert navigation
        Assert.EndsWith("/home", _navManager.Uri);

        // Assert AuthState updated
        _mockAuthState.Verify(a => a.SetUserAsync("123", "Student"), Times.Once);
    }
}
