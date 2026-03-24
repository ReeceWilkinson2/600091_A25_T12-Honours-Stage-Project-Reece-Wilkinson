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

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<UserInfoService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("Smtp"));


var dbPath = Path.Combine(builder.Environment.ContentRootPath,"UserDatabase.db");
builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<CanvasSyncService>();
builder.Services.AddHttpClient<CanvasService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<ModuleCSVReading>();
builder.Services.AddScoped<CSVReaderStartup>();
builder.Services.AddScoped<CompetencyDataServices>();

var app = builder.Build();

// Ensure database exists
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.OpenConnection();
    db.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
    db.Database.EnsureCreated();
    try
    {
        var CSVService = scope.ServiceProvider.GetRequiredService<CSVReaderStartup>();
        await CSVService.ReadModuleCSV();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    await DbSeeder.SeedTestData(db);

    db.Database.CloseConnection();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();