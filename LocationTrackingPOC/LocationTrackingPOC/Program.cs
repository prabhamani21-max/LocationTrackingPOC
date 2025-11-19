using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NetTopologySuite;
using StackExchange.Redis;
using LocationTrackingPOC.Hubs;
using LocationTrackingService.Implementation;
using LocationTrackingService.Interface;
using LocationTrackingPOC.Mapper;
using LocationTrackingRepository.Interface;
using LocationTrackingRepository.Implementation;
using LocationTrackingRepository.Data;
using LocationTrackingPOC.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
// Add HttpContext
builder.Services.AddHttpContextAccessor();
//DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
      options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o => o.UseNetTopologySuite()));
Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

// Register Redis ConnectionMultiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    configuration.ResolveDns = true;
    return ConnectionMultiplexer.Connect(configuration);
});

// Make Redis available as IDistributedCache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "waste_mgmt:";
});


// Register IDatabase for direct Redis operations
builder.Services.AddSingleton<StackExchange.Redis.IDatabase>(sp =>
    sp.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>().GetDatabase());

// SignalR
builder.Services.AddSignalR();

// AutoMapper Profiles
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IDriverLocationRepository, DriverLocationRepository>();
builder.Services.AddScoped<ICollectionRequestRepository, CollectionRequestRepository>();
//services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<ICollectionRequestService, CollectionRequestService>();
builder.Services.AddScoped<ILocationTrackingService, LocationTrackingService.Implementation.LocationTrackingService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Background services
builder.Services.AddHostedService<LocationPersistenceService>();

// register a shared GeometryFactory (choose the SRID you need, e.g. 4326)
builder.Services.AddSingleton(
    NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326)
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseMiddleware<CurrentUserMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHub<LocationHub>("/locationHub");

app.Run();
