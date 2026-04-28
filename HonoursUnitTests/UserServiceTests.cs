using Bunit;
using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Components.Pages;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HonoursUnitTests;

public class UserServiceTests
{

    private AppDbContext GetDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private UserService GetService(AppDbContext db)
        => new UserService(db);

    [Fact]
    public async Task AuthenticateAsync_ReturnsUser_WhenPlainPasswordMatches()
    {
        var db = GetDb();

        db.Users.Add(new User
        {
            StEmail = "test@email.com",
            Password = "1234",
            StudentId = "U1"
        });

        await db.SaveChangesAsync();

        var service = GetService(db);

        var result = await service.AuthenticateAsync("test@email.com", "1234");

        Assert.NotNull(result);
        Assert.Equal("U1", result!.StudentId);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_WhenWrongPassword()
    {
        var context = GetDb();

        context.Users.Add(new User
        {
            StudentId = "123",
            StEmail = "test@hull.ac.uk",
            Password = BCrypt.Net.BCrypt.HashPassword("correct")
        });

        await context.SaveChangesAsync();

        var service = new UserService(context);

        // Act
        var result = await service.AuthenticateAsync("test@hull.ac.uk", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser()
    {
        var db = GetDb();

        db.Users.Add(new User
        {
            StEmail = "find@me.com",
            StudentId = "X1"
        });

        await db.SaveChangesAsync();

        var service = GetService(db);

        var result = await service.GetUserByEmailAsync("find@me.com");

        Assert.NotNull(result);
        Assert.Equal("X1", result!.StudentId);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenNotFound()
    {
        var db = GetDb();
        var service = GetService(db);

        var result = await service.GetUserByIdAsync("missing");

        Assert.Null(result);
    }

    [Fact]
    public async Task AddUserAsync_AddsUser()
    {
        var db = GetDb();
        var service = GetService(db);

        var user = new User { StudentId = "1", StEmail = "a@a.com" };

        var result = await service.AddUserAsync(user);

        Assert.Single(db.Users);
        Assert.Equal("1", result.StudentId);
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesUser()
    {
        var db = GetDb();

        var user = new User { StudentId = "1", StEmail = "a@a.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = GetService(db);

        user.StEmail = "updated@email.com";

        var result = await service.UpdateUserAsync(user);

        Assert.Equal("updated@email.com", result.StEmail);
    }

    [Fact]
    public async Task DeleteUserAsync_RemovesUser()
    {
        var db = GetDb();

        var user = new User { StudentId = "1" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = GetService(db);

        await service.DeleteUserAsync(user);

        Assert.Empty(db.Users);
    }

    [Fact]
    public async Task ChangePasswordAsync_ReturnsFalse_WhenUserMissing()
    {
        var db = GetDb();
        var service = GetService(db);

        var result = await service.ChangePasswordAsync("nope", "old", "new");

        Assert.False(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_ChangesPassword_WhenValid()
    {
        var db = GetDb();

        db.Users.Add(new User
        {
            StudentId = "1",
            Password = "oldpass"
        });

        await db.SaveChangesAsync();

        var service = GetService(db);

        var result = await service.ChangePasswordAsync("1", "oldpass", "newpass");

        Assert.True(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_ReturnsFalse_WhenWrongPassword()
    {
        var db = GetDb();

        db.Users.Add(new User
        {
            StudentId = "1",
            Password = "oldpass"
        });

        await db.SaveChangesAsync();

        var service = GetService(db);

        var result = await service.ChangePasswordAsync("1", "wrong", "newpass");

        Assert.False(result);
    }

    [Fact]
    public async Task GetStudentDataAsync_FiltersByRole()
    {
        var db = GetDb();

        db.Users.AddRange(
            new User { StudentId = "1", Role = "Student" },
            new User { StudentId = "2", Role = "Staff" }
        );

        await db.SaveChangesAsync();

        var service = GetService(db);

        var result = await service.GetStudentDataAsync("Student");

        Assert.Single(result);
        Assert.Equal("1", result[0].StudentId);
    }

    [Fact]
    public async Task GenerateUniqueStudentIdAsync_Returns6Digits()
    {
        var db = GetDb();
        var service = GetService(db);

        var id = await service.GenerateUniqueStudentIdAsync();

        Assert.Equal(6, id.Length);
        Assert.True(int.TryParse(id, out _));
    }
}