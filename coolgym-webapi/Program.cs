using coolgym_webapi.Contexts.Shared.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;
using coolgym_webapi.Contexts.Equipments.Application.CommandServices;
using coolgym_webapi.Contexts.Equipments.Application.QueryServices;
using coolgym_webapi.Contexts.Equipments.Domain.Repositories;
using coolgym_webapi.Contexts.Equipments.Domain.Services;
using coolgym_webapi.Contexts.Equipments.Infrastructure.Persistence.Repositories;  
using Microsoft.EntityFrameworkCore;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; // <--- agregar para probar el back en el front
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            // ESTA ES LA URL DEl FRONTEND 
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
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
builder.Services.AddScoped<IEquipmentCommandService, EquipmentCommandService>();
builder.Services.AddScoped<IEquipmentQueryService, EquipmentQueryService>();


var app = builder.Build();


// Ensure DB is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CoolgymContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

// Usar el middleware de CORS ANTES de UseAuthorization()
app.UseCors(MyAllowSpecificOrigins); 

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();