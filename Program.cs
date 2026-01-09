using Microsoft.EntityFrameworkCore;
using DierentuinApp.Data;
using DierentuinApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Database configuratie
builder.Services.AddDbContext<ZooContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ZooService registreren voor dependency injection (Scoped = per request een nieuwe instance)
builder.Services.AddScoped<ZooService>();

// Swagger configuratie
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MVC en API controllers toevoegen
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    }); // Voorkomt infinite loops

var app = builder.Build();

// Database migratie en seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ZooContext>();
    context.Database.Migrate();  // Voert alle pending migrations uit
    DbSeeder.Seed(context);      // Vult database met testdata als deze leeg is
}

// Swagger configuratie
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Foutafhandeling en beveiliging voor productie
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Route configuratie voor MVC en API controllers
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();