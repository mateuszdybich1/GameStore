using System.Text;
using GameStore.Domain.UserEntities;
using GameStore.Infrastructure;
using GameStore.Infrastructure.MongoRepositories;
using GameStore.Users;
using GameStore.Web;
using GameStore.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

services.Configure<BlobStorageConfiguration>(builder.Configuration.GetSection("BlobStorage"));

services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("GameStoreDatabase"));
});

services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("GameStoreUsers"));
});

services.AddIdentity<PersonModel, RoleModel>(options =>
{
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<IdentityDbContext>();

services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

services.AddHttpClient("PaymentMicroservice", client =>
{
    var connString = builder.Configuration.GetConnectionString("PaymentApiBaseUrl").ToString();
    client.BaseAddress = new Uri(connString);
});

services.AddHttpClient("AuthMicroservice", client =>
{
    var connString = builder.Configuration.GetConnectionString("AuthApiBaseUrl").ToString();
    client.BaseAddress = new Uri(connString);
});

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings:SecretKey").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
    };
});

services.RegisterServices();

services.AddHostedService(provider =>
{
    return new UserUnbanService(builder.Configuration.GetConnectionString("GameStoreUsers")!);
});

services.AddMemoryCache();

services.AddCors();
services.AddControllers().AddNewtonsoftJson();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

QuestPDF.Settings.License = LicenseType.Community;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    string infoLogsPath = builder.Configuration.GetValue("FilePaths:InfoLogs", "default/path.log");
    string errorLogsPath = builder.Configuration.GetValue("FilePaths:ErrorLogs", "default/error.log");
    app.UseMiddleware<LoggingMiddleware>(infoLogsPath, errorLogsPath);
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    PredefinedObjects predefined = new(dbContext);
    predefined.AddGenres();
    predefined.AddPlatforms();

    var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    identityDbContext.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleModel>>();
    PredefinedUserRoles predefinedUserRoles = new(roleManager);
    await predefinedUserRoles.AddDefaultUserRoles();
}

app.UseDeveloperExceptionPage();

app.UseMiddleware<TotalGamesMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
