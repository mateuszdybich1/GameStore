using GameStore.Infrastructure;
using GameStore.Web;
using GameStore.Web.Middlewares;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("GameStoreDatabase"));
});

services.AddHttpClient("PaymentMicroservice", client =>
{
    var connString = builder.Configuration.GetConnectionString("PaymentApiBaseUrl").ToString();
    client.BaseAddress = new Uri(connString);
});

services.RegisterServices();
services.AddCors();
services.AddControllers().AddNewtonsoftJson();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

QuestPDF.Settings.License = LicenseType.Community;

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

app.UseDeveloperExceptionPage();

string infoLogsPath = builder.Configuration.GetValue("FilePaths:InfoLogs", "default/path.log");
string errorLogsPath = builder.Configuration.GetValue("FilePaths:ErrorLogs", "default/error.log");

app.UseMiddleware<LoggingMiddleware>(infoLogsPath, errorLogsPath);

app.UseMiddleware<TotalGamesMiddleware>();

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials());

app.UseAuthorization();

app.MapControllers();

app.Run();
