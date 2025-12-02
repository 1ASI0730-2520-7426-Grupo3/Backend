using System.Globalization;
using System.Reflection;
using coolgym_webapi.Contexts.BillingInvoices.Application.CommandServices;
using coolgym_webapi.Contexts.BillingInvoices.Application.QueryServices;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Repositories;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Services;
using coolgym_webapi.Contexts.BillingInvoices.Infrastructure.Persistence.Repositories;
using coolgym_webapi.Contexts.ClientPlans.Application.QueryServices;
using coolgym_webapi.Contexts.ClientPlans.Domain.Model.Entities;
using coolgym_webapi.Contexts.ClientPlans.Domain.Repositories;
using coolgym_webapi.Contexts.ClientPlans.Domain.Services;
using coolgym_webapi.Contexts.ClientPlans.Infrastructure.Persistence.Repositories;
using coolgym_webapi.Contexts.Equipments.Application.CommandServices;
using coolgym_webapi.Contexts.Equipments.Application.QueryServices;
using coolgym_webapi.Contexts.Equipments.Domain.Repositories;
using coolgym_webapi.Contexts.Equipments.Domain.Services;
using coolgym_webapi.Contexts.Equipments.Infrastructure.Persistence.Repositories;
using coolgym_webapi.Contexts.maintenance.Application.CommandServices;
using coolgym_webapi.Contexts.maintenance.Application.QueryServices;
using coolgym_webapi.Contexts.maintenance.Domain.Repositories;
using coolgym_webapi.Contexts.maintenance.Domain.Services;
using coolgym_webapi.Contexts.maintenance.Infrastructure.Persistence.Repositories;
using coolgym_webapi.Contexts.Rentals.Application.CommandServices;
using coolgym_webapi.Contexts.Rentals.Application.QueryServices;
using coolgym_webapi.Contexts.Rentals.Domain.Repositories;
using coolgym_webapi.Contexts.Rentals.Domain.Services;
using coolgym_webapi.Contexts.Rentals.Infrastructure.Persistence.Repositories;
using coolgym_webapi.Contexts.Security.Application.CommandServices;
using coolgym_webapi.Contexts.Security.Application.QueryServices;
using coolgym_webapi.Contexts.Security.Domain.Infrastructure;
using coolgym_webapi.Contexts.Security.Domain.Middleware;
using coolgym_webapi.Contexts.Security.Domain.Services;
using coolgym_webapi.Contexts.Security.Infrastructure;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:5173",
                    "https://localhost:5173",
                    "https://frontend-coolgym-tf.vercel.app",
                    "https://frontend-coolgym-7ii3o8kgq-and12326s-projects.vercel.app")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("coolgymCenter")
                       ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'coolgymCenter'.");

builder.Services.AddDbContext<CoolgymContext>(options =>
{
    options.UseMySQL(connectionString);

    if (builder.Environment.IsDevelopment())
        options.LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    else if (builder.Environment.IsProduction())
        options.LogTo(Console.WriteLine, LogLevel.Error)
            .EnableDetailedErrors();
});

//Dependency injection

// Shared
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// Equipment Context
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddTransient<IEquipmentCommandService, EquipmentCommandService>();
builder.Services.AddTransient<IEquipmentQueryService, EquipmentQueryService>();

//Maintenance Request Context
builder.Services.AddScoped<IMaintenanceRequestRepository, MaintenanceRequestRepository>();
builder.Services.AddScoped<IMaintenanceRequestCommandService, MaintenanceRequestCommandService>();
builder.Services.AddScoped<IMaintenanceRequestQueryService, MaintenanceRequestQueryService>();
// Billing Invoices Context
builder.Services.AddScoped<IBillingInvoiceRepository, BillingInvoiceRepository>();
builder.Services.AddTransient<IInvoiceQueryService, InvoiceQueryService>();
builder.Services.AddTransient<IInvoiceCommandService, InvoiceCommandService>();

// Rental Requests Context
builder.Services.AddScoped<IRentalRequestRepository, RentalRequestRepository>();
builder.Services.AddTransient<IRentalRequestCommandService, RentalRequestCommandService>();
builder.Services.AddTransient<IRentalRequestQueryService, RentalRequestQueryService>();

// Security Context
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IPasswordHashService, PasswordHashService>();
builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();
builder.Services.AddTransient<IUserCommandService, UserCommandService>();
builder.Services.AddTransient<IUserQueryService, UserQueryService>();

// Client Plans Context
builder.Services.AddScoped<IClientPlanRepository, ClientPlanRepository>();
builder.Services.AddTransient<IClientPlanQueryService, ClientPlanQueryService>();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Información general de la API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "CoolGym Equipment Management API",
        Description = "An ASP.NET Core Web API for managing fitness equipment with real-time monitoring capabilities",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "CoolGym Support Team",
            Email = "support@coolgym.com",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "CoolGym License",
            Url = new Uri("https://example.com/license")
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.UseInlineDefinitionsForEnums();
    // options.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
});


var app = builder.Build();

//swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CoolGym API v1");
    options.RoutePrefix = "swagger";

    options.DocumentTitle = "CoolGym API Documentation";
    options.DisplayRequestDuration();
    options.EnableDeepLinking();
    options.EnableFilter();
    options.ShowExtensions();
});

// Ensure DB is created and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CoolgymContext>();
    context.Database.EnsureCreated();
    
    if (!context.ClientPlans.Any())
    {
        var plans = new[]
        {
            // Client Plans (Individual users)
            new ClientPlan(
                "Basic",
                "Perfect for individual users. Access to up to 6 machines.",
                18.99m,
                6,
                false,
                false
            ),
            new ClientPlan(
                "Standard",
                "For active users. Access to up to 12 machines with maintenance support.",
                35.13m,
                12,
                true,
                false
            ),
            new ClientPlan(
                "Premium",
                "For power users. Access to up to 24 machines with full support.",
                67.56m,
                24,
                true,
                true
            ),
            // Provider Plans (Company/Business users)
            new ClientPlan(
                "Small Company",
                "Perfect for small businesses. Manage up to 10 clients.",
                40.51m,
                10,
                false,
                false
            ),
            new ClientPlan(
                "Medium Company",
                "Ideal for growing companies. Manage up to 30 clients with maintenance support.",
                81.08m,
                30,
                true,
                false
            ),
            new ClientPlan(
                "Enterprise Premium",
                "Ultimate solution for large enterprises. Unlimited clients with priority support.",
                162.16m,
                999,
                true,
                true
            )
        };

        context.ClientPlans.AddRange(plans);
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();


var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("es")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(localizationOptions);

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseMiddleware<AuthMiddleware>();

app.MapControllers();

app.Run();