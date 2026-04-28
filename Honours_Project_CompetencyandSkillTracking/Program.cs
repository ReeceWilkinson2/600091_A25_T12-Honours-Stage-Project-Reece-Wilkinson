using Blazored.LocalStorage;
using Honours.Services;
using Honours_Project_CompetencyandSkillTracking.Canvas;
using Honours_Project_CompetencyandSkillTracking.Components;
using Honours_Project_CompetencyandSkillTracking.Data;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<UserInfoService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("Smtp"));

// Configure SQLite database
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "UserDatabase.db");
builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlite($"Data Source={dbPath}"));

// Add scoped services
builder.Services.AddScoped<CanvasSyncService>();
builder.Services.AddHttpClient<CanvasService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<IAuthStateService, AuthStateService>();
builder.Services.AddScoped<CSVReading>();
builder.Services.AddScoped<CSVReaderStartup>();
builder.Services.AddScoped<CompetencyDataServices>();
builder.Services.AddScoped<ICompetencyDataServices, CompetencyDataServices>();

var app = builder.Build();

// Ensure database exists and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Open SQLite connection and enforce foreign keys
    db.Database.OpenConnection();
    db.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
    db.Database.EnsureCreated();

    try
    {
        var csvService = scope.ServiceProvider.GetRequiredService<CSVReaderStartup>();

        if (!db.Users.Any())
        {
            Console.WriteLine("Seeding database from CSV...");
            await csvService.SeedAllAsync();
            Console.WriteLine("Seeding complete.");
        }
        else
        {
            Console.WriteLine("Database already seeded.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Seeder error: {ex.Message}");
    }
    finally
    {
        db.Database.CloseConnection();
    }
}

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();