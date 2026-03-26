using Expo.API.Services;
using Expo.API.Services.DbServices;
using Expo.API.Utils;
using Expo.Domain.Constants;
using Expo.Domain.Interfaces.Repo;
using Expo.Domain.Interfaces.Services;
using Expo.Infrastructure.Data;
using Expo.Infrastructure.Repositories;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Expo.API.Extensions;

/// <summary>
/// Extension method for IService collection
/// </summary>
internal static class ServicesExtension
{
    /// <summary>
    /// Setup infrastructure
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    internal static void SetupInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStrign = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionStrign)
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPavilionRepository, PavilionRepository>();
        services.AddScoped<IExhibitionHallRepository, ExhibitionHallRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IStandRepository, StandRepository>();

        services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionStrign)));

        services.AddHangfireServer();
    }
    /// <summary>
    /// Setup Authentication
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    internal static void SetupAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        var jwtSection = configuration.GetSection("Jwt");
        var keyBytes = Convert.FromBase64String(jwtSection["Key"]);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateLifetime = true
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.Users.CanCreateUser, policy => policy.RequireClaim(Permissions.Users.Create));
            options.AddPolicy(Policy.Users.CanPromoteUser, policy => policy.RequireClaim(Permissions.Users.Promote));
            options.AddPolicy(Policy.Users.CanDemoteUser, policy => policy.RequireClaim(Permissions.Users.Demote));
            options.AddPolicy(Policy.Users.CanReadUser, policy => policy.RequireClaim(Permissions.Users.Read));
            options.AddPolicy(Policy.Entity.CanCreateEntity, policy => policy.RequireClaim(Permissions.Entities.Create));
            options.AddPolicy(Policy.Entity.CanUpdateEntity, policy => policy.RequireClaim(Permissions.Entities.Update));
            options.AddPolicy(Policy.Entity.CanDeleteEntity, policy => policy.RequireClaim(Permissions.Entities.Delete));
        });

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();

        if (emailSettings.Enabled)
        {
            services.AddSingleton<IAPIEmailSender, RegistationService>();
        }
        else
        {
            services.AddSingleton<IAPIEmailSender, DebugHtmlEmailSender>();
        }
    }
    /// <summary>
    /// Add application services
    /// </summary>
    /// <param name="services"></param>
    internal static void AddExpoServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped<ICategoriaService, CategoryService>();
        services.AddScoped<IPavilionService, PavilionService>();
        services.AddScoped<IExhibitionHallService, ExhibitionHallService>();
        services.AddScoped<IStandService, StandService>();
    }
    /// <summary>
    /// Setup login application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    internal static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        return services;
    }
    /// <summary>
    /// Setup validator
    /// </summary>
    /// <param name="serviceCollection"></param>
    internal static void AddValidators(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddValidatorsFromAssemblyContaining<Program>();
    }
}