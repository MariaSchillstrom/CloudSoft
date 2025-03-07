using CloudSoft.Models;
using CloudSoft.Configurations;
using MongoDB.Driver;
using CloudSoft.Repositories;  // För MongoDbSubscriberRepository
using CloudSoft.Services;  // För INewsletterService och NewsletterService
using Microsoft.Extensions.Options;  // För att hantera MongoDBOptions

var builder = WebApplication.CreateBuilder(args);

// Lägg till tjänster till containern
builder.Services.AddControllersWithViews();

// Läs från appsettings.json för att hämta MongoDB-inställningar
builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var mongoDbOptions = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
    return new MongoClient(mongoDbOptions.ConnectionString);  // Använder connection string från appsettings.json
});

// Lägg till MongoDB-kollektion och repository
builder.Services.AddSingleton<IMongoCollection<Subscriber>>(serviceProvider =>
{
    var mongoDbOptions = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
    var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
    var database = mongoClient.GetDatabase(mongoDbOptions.DatabaseName);
    return database.GetCollection<Subscriber>(mongoDbOptions.SubscribersCollectionName);  // Använder databas- och kollektionsnamn från appsettings.json
});

// Registrera MongoDB-repository
builder.Services.AddSingleton<ISubscriberRepository, MongoDbSubscriberRepository>();

// Registrera NewsletterService (som beror på repository)
builder.Services.AddScoped<INewsletterService, NewsletterService>();

var app = builder.Build();

// Konfigurera HTTP-pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
