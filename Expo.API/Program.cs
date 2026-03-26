using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation.AspNetCore;
using Expo.API.Extensions;
using Expo.API.Middleware;
using Expo.API.Middleware.Validations;
using Expo.API.Utils;
using Expo.Infrastructure.Data;
using Expo.Infrastructure.Seeders;


var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// LOGGING & SERVICES
// --------------------------------------------------

builder.Host.UseSerilog();
builder.Services.AddLogging(builder.Configuration);
builder.Services.SetupAuthentication(builder.Configuration);
builder.Services.SetupInfrastructure(builder.Configuration);
builder.Services.AddExpoServices();

// --------------------------------------------------
// DTO MAPPING
// --------------------------------------------------

//builder.Services.AddAutoMapper(cfg => cfg.AddProfile<EntityProfile>());

// ⬇ REGISTRA tutte le tue configurazioni Mapster
MapsterConfig.RegisterMappings();

// ⬇ Se vuoi usare IMapper con DI
var config = TypeAdapterConfig.GlobalSettings;
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();
// --------------------------------------------------
// CONTROLLERS + VALIDATION
// --------------------------------------------------

builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddValidators();

builder.Services.AddFluentValidationAutoValidation(o =>
{
    o.DisableDataAnnotationsValidation = true;
});

builder.Services.AddEndpointsApiExplorer();

// --------------------------------------------------
// API VERSIONING
// --------------------------------------------------

builder.Services
    .AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";       // v1, v2, v3
        options.SubstituteApiVersionInUrl = true;
    });

// --------------------------------------------------
// SWAGGER
// --------------------------------------------------

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddSwaggerGen(options =>
{
    // Include XML documentation
    options.IncludeXmlComments(xmlPath);

    // Add versioning support (populated by ConfigureSwaggerOptions)
    options.OperationFilter<SwaggerDefaultValues>();

    // JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insert JWT token as 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// --------------------------------------------------
// APP
// --------------------------------------------------

var app = builder.Build();

app.UseSerilogRequestLogging();

// DB Migration + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await Seeder.SeedAsync(scope.ServiceProvider);
}

// --------------------------------------------------
// SWAGGER UI
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error500");

    app.UseSwagger();

    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwaggerUI(options =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{desc.GroupName}/swagger.json",
                $"{ApiConstants.APINAME} {desc.GroupName.ToUpperInvariant()}"
            );
        }
    });
}

// --------------------------------------------------
// PIPELINE
// --------------------------------------------------

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();