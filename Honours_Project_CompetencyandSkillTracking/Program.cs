using Blazored.LocalStorage;
using Honours.Services;
using Honours_Project_CompetencyandSkillTracking.Canvas;
using Honours_Project_CompetencyandSkillTracking.Components;
using Honours_Project_CompetencyandSkillTracking.Data;
using Honours_Project_CompetencyandSkillTracking.Components.Services;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
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
builder.Services.AddSyncfusionBlazor();
builder.Services.AddSingleton<ISyncfusionStringLocalizer, SyncfusionStringLocalizer>();

var app = builder.Build();

// Ensure databases are created at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
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