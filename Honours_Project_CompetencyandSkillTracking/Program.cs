using Blazored.LocalStorage;
using Honours.Services;
using Honours_Project_CompetencyandSkillTracking.Canvas;
using Honours_Project_CompetencyandSkillTracking.Components;
using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<UserInfoDbContext>(option => { option.UseSqlite("Data Source = UserDatabase.db"); });
builder.Services.AddScoped<UserDataServices>();
builder.Services.AddSingleton<UserInfoService>();
builder.Services.AddBlazoredLocalStorage();

var dbPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "UserDatabase.db");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<CanvasSyncService>();
builder.Services.AddHttpClient<CanvasService>();



var app = builder.Build();

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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();