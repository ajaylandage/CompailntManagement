using ComplaintManagementSystem.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ComplaintManagementSystem.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient to point to the backend API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7249/") });

// Register client services
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ComplaintService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();

// Authentication
builder.Services.AddAuthorizationCore();
// Register JwtAuthenticationStateProvider and expose as AuthenticationStateProvider
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());

var host = builder.Build();

// Initialize auth service before rendering
var authService = host.Services.GetRequiredService<AuthService>();
await authService.InitializeAsync();

await host.RunAsync();
