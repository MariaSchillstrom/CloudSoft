using CloudSoft.Services;
using CloudSoft.Repositories;  // Lägg till detta om det saknas

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//// Register repository
builder.Services.AddSingleton<ISubscriberRepository, InMemorySubscriberRepository>();

// Register service (depends on repository)
builder.Services.AddScoped<INewsletterService, NewsletterService>();

// 🔹 Registrera NewsletterService i DI-container
builder.Services.AddScoped<INewsletterService, NewsletterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
