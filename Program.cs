using DotNetEnv;
using PatientManagementSystem.Models;

// Load .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Register DatabaseHelper
builder.Services.AddSingleton<DatabaseHelper>(new DatabaseHelper(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

// Register GroqService using .env key
builder.Services.AddSingleton<GroqService>(new GroqService(
    Environment.GetEnvironmentVariable("GROQ_API_KEY")
));


// Add MVC services
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();