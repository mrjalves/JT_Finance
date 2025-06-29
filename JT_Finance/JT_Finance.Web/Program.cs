using JT_Finance.Shared.Services;
using JT_Finance.Web.Components;
using JT_Finance.Web.Services;

using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the JT_Finance.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped<JT_Finance.Shared.Services.ToastService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(JT_Finance.Shared._Imports).Assembly,
        typeof(JT_Finance.Web.Client._Imports).Assembly);

app.Run();
