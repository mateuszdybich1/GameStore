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

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    PredefinedObjects predefined = new(context);
    predefined.AddGenres();
    predefined.AddPlatforms();
}

app.UseMiddleware<TotalGamesMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
