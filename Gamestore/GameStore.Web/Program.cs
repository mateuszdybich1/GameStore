using GameStore.Infrastructure;
using GameStore.Web;
using GameStore.Web.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("Data Source = localhost, 1433; Initial Catalog = PROJECT; Integrated Security = True; TrustServerCertificate=True;");
});

services.AddControllersWithViews();

services.RegisterServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    PredefinedObjects predefined = new(context);
    predefined.AddGenres();
    predefined.AddPlatforms();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<TotalGamesMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();