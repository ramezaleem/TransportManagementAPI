using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using TransportManagement.API.Services;
using TransportManagement.Application.Services;
using TransportManagement.Infrastructure.Data;

namespace TransportManagement.API;

public class Program
{
    public static void Main ( string[] args )
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Transport Management API", Version = "v1" });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by a space and your token. Example: Bearer 12345abcdef"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        // ====== ≈÷«›… ≈⁄œ«œ«  «·‹ CORS Â‰« ======
        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:3000", // ÷Ì› Â‰« √Ì origin  «‰Ì „Õ «ÃÂ
                        "https://localhost:3000"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
        });

        builder.Services.AddDbContext<TransportDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)));

        builder.Services.AddHealthChecks()
            .AddSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                tags: new[] { "database" });

        // Application Services
        builder.Services.AddScoped<ITransporterService, TransporterService>();
        builder.Services.AddScoped<IVehicleService, VehicleService>();
        builder.Services.AddScoped<IDirectionsService, DirectionsService>();
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<ITripService, TripService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddSingleton<JwtTokenHelper>();
        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddSingleton<IWhatsappService, WhatsappService>();

        // JWT Settings
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = builder.Configuration["JwtSettings:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new Exception("JWT SecretKey is not set in appsettings.json!");
        }

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapGet("/", () => Results.Redirect("/swagger"));
        app.UseHttpsRedirection();

        // ================ «” Œœ„ «·‹ CORS Â‰« ================
        app.UseCors(MyAllowSpecificOrigins);

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.UseMiddleware<TransportManagement.API.Middleware.ErrorHandlingMiddleware>();
        app.MapControllers();
        app.Run();
    }
}
