using CountryBlockAPI.Core;
using CountryBlockAPI.Core.Base.MiddleWare;
using CountryBlockAPI.Infrastructure;
using CountryBlockAPI.Service;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

try
{
    ConfigureServices(builder.Services);
}
catch (Exception ex)
{
    Console.WriteLine($"Service configuration failed: {ex.Message}");
}

var app = builder.Build();

ConfigureMiddleware(app);

await app.RunAsync();
return;

void ConfigureServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Country Block API", Version = "v1" });

        // Enable Annotations (for [FromBody], [FromQuery], etc.)
        opt.EnableAnnotations();
        // Add Swagger examples
        opt.ExampleFilters();
    });
    // Register Swagger example providers
    services.AddSwaggerExamplesFromAssemblyOf<Program>();

    services.AddControllers()
        .AddControllersAsServices();

    // Enable CORS
    services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
    services
        .AddServiceDependencies()
        .AddCoreDependencies()
        .AddInfrastructureDependencies();
}

void ConfigureMiddleware(WebApplication application)
{
    application.UseHttpsRedirection();
    application.UseCors("AllowAll");

    application.UseSwagger();
    application.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Country Block API v1");
        c.RoutePrefix = string.Empty;
        c.DisplayRequestDuration();
    });
    application.UseMiddleware<ErrorHandlerMiddleware>();

    application.MapControllers();
}