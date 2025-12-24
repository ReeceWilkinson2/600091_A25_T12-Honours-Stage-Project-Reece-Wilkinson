using Honours_Project_CompetencyandSkillTracking.Components;
using Honours_Project_CompetencyandSkillTracking.Data;
using Honours.Services;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.EntityFrameworkCore;
using Blazored.LocalStorage;


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
