using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TransporteEmpresarial.API.Handlers;
using TransporteEmpresarial.Application.Contract;
using TransporteEmpresarial.Application.Service;
using TransporteEmpresarial.Infrastructure.Context;
using TransporteEmpresarial.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Los enums viajan como texto en el JSON ("Programado", "Conductor"...), no como números
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));

// Documento OpenAPI (disponible en /openapi/v1.json)
builder.Services.AddOpenApi();

// Errores con formato ProblemDetails. Los servicios devuelven sus errores en un ServiceResult;
// el manejador queda como respaldo por si una excepción de negocio llega hasta la API.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TransporteExceptionHandler>();

// Capa de infraestructura: DbContext de SQL Server y repositorios
var cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'DefaultConnection' en appsettings.json.");
builder.Services.AddInfrastructure(cadenaConexion);

// Capa de servicios: lógica de negocio y validación de cada campo antes de llegar al repositorio
builder.Services.AddScoped<IRutaService, RutaService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IVehiculoService, VehiculoService>();
builder.Services.AddScoped<IViajeService, ViajeService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Interfaz gráfica para probar los endpoints en /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Transporte Empresarial API v1");
    });

    // Aplica las migraciones pendientes al iniciar (crea la base de datos si no existe)
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TransporteContext>();
    context.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
