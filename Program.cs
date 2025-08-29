using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FluentValidation;
using Serilog;
using FuelTrack.Api.Shared.Data;
using FuelTrack.Api.Shared.Models;
using FuelTrack.Api.Auth.Services;
using FuelTrack.Api.Users.Services;
using FuelTrack.Api.Orders.Services;
using FuelTrack.Api.Payments.Services;
using FuelTrack.Api.Vehicles.Services;
using FuelTrack.Api.Operators.Services;
using FuelTrack.Api.Notifications.Services;
using FuelTrack.Api.Analytics.Services;
using FuelTrack.Api.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ───── Logger: Serilog ─────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// ───── Services ────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ───── Swagger ─────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FuelTrack API",
        Version = "v1",
        Description = "Sistema de gestión de pedidos de combustible"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando esquema Bearer. Ej: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

// ───── Entity Framework con PostgreSQL ─────────────
builder.Services.AddDbContext<FuelTrackDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ───── JWT Auth ─────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(options =>
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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ───── CORS corregido ──────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",                // desarrollo local
                "https://front-end-six-livid.vercel.app" // producción en Vercel
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ───── Validaciones ────────────────────────────────
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ───── Servicios de dominio ────────────────────────
builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IOperatorService, OperatorService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

var app = builder.Build();

// ───── Middlewares ─────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FuelTrack API V1");
    c.RoutePrefix = "swagger";
});

// Redirecciona HTTP a HTTPS
app.UseHttpsRedirection();

// ───── Middleware de manejo de errores global ──────
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var errorMessage = new
        {
            error = "Error interno del servidor",
            detail = ex.Message
        };

        await context.Response.WriteAsJsonAsync(errorMessage);
    }
});

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AccessLogMiddleware>();

app.MapControllers();

// ───── Migraciones y Seed (auto) ───────────────────
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<FuelTrackDbContext>();
    await context.Database.MigrateAsync(); // Aplica migraciones
    await SeedData.Initialize(context);    // Seed inicial
}
catch (Exception ex)
{
    Log.Error(ex, "❌ Error al aplicar migraciones o inicializar datos");
    // Puedes decidir si lanzas una excepción o continúas
}

app.Run();
