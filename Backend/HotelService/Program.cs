using HotelService.Data;
using HotelService.Repositories;
using HotelService.Services;
using HotelService.Services.Http;
using HotelService.Common.Middleware;
using HotelService.Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OHMS - Hotel Service API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your JWT token (you can paste just the token, or 'Bearer <token>')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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
            Array.Empty<string>()
        }
    });
});

// Configure Database
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<HotelDbContext>(options =>
{
    if (useInMemory || string.IsNullOrEmpty(connectionString))
    {
        options.UseInMemoryDatabase("HotelDB");
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

// Repositories and Services
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();

// HTTP Client for Booking Service
var bookingServiceUrl = builder.Configuration["Services:BookingServiceUrl"] ?? "http://localhost:5003";
builder.Services.AddHttpClient<IBookingServiceClient, BookingServiceClient>(client =>
{
    client.BaseAddress = new Uri(bookingServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(5);
});

// Shared JWT Authentication
builder.Services.AddSharedJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Ensure Database Created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    db.Database.EnsureCreated();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

//public partial class Program { }
