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
using Expo.Infrastructure.Seeders;
using Expo.Infrastructure.Persistence;
using Expo.Application;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// LOGGING
// --------------------------------------------------
builder.Host.UseSerilog();
builder.Services.AddLogging(builder.Configuration);

// --------------------------------------------------
// AUTHENTICATION & INFRASTRUCTURE
// --------------------------------------------------
builder.Services.SetupAuthentication(builder.Configuration);
builder.Services.SetupInfrastructure(builder.Configuration); // DbContext, Repositories, Services
builder.Services.AddExpoServices(); // I tuoi servizi applicativi

// --------------------------------------------------
// APPLICATION LAYER
// --------------------------------------------------
builder.Services.AddApplication();
// --------------------------------------------------
// CONTROLLERS + VALIDATION
// --------------------------------------------------
builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddValidators(); // Aggiunge tutti i validator
builder.Services.AddFluentValidationAutoValidation(o => o.DisableDataAnnotationsValidation = true);

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
        options.GroupNameFormat = "'v'VVV"; // v1, v2, v3
        options.SubstituteApiVersionInUrl = true;
    });

// --------------------------------------------------
// SWAGGER
// --------------------------------------------------
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(xmlPath); // Documentazione XML
    options.OperationFilter<SwaggerDefaultValues>();

    // JWT Bearer Authorization
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
// BUILD APP
// --------------------------------------------------
var app = builder.Build();

// --------------------------------------------------
// LOG REQUESTS
// --------------------------------------------------
app.UseSerilogRequestLogging();

// --------------------------------------------------
// DATABASE MIGRATION + SEED
// --------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await Seeder.SeedAsync(scope.ServiceProvider);
}

// --------------------------------------------------
// SWAGGER UI (solo dev)
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
                $"Expo API {desc.GroupName.ToUpperInvariant()}"
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