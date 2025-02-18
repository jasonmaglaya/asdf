using AspNetCoreRateLimit;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Remy.Gambit.Api.Handlers.Auth.Command;
using Remy.Gambit.Api.Hubs;
using Remy.Gambit.Api.Mappers;
using Remy.Gambit.Api.Validators;
using Remy.Gambit.Api.Web.ActionFilters;
using Remy.Gambit.Core.Concurrency;
using Remy.Gambit.Data;
using Remy.Gambit.Services;
using System.Data;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IDbConnection>(x => new SqlConnection(builder.Configuration["ConnectionStrings:GambitDbSqlConnection"]));
builder.Services.AddTransient<IGambitDbClient, GambitDbClient>();
builder.Services.AddSingleton<IUserLockService, SemaphoreUserLockService>();
builder.Services.AddTransient<IPartnerService, MarvelGamingService>();

// Repositories
builder.Services.Scan(scan =>
{
    scan.FromTypes()
        .FromAssemblyOf<IGambitDbClient>()
        .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase)))
        .AsImplementedInterfaces()
        .WithScopedLifetime();
});

// Action Filters
builder.Services.Scan(scan =>
{
    scan.FromTypes()
        .FromAssemblyOf<FeatureFilterAttribute>()
        .AddClasses(c => c.Where(t => t.Name.EndsWith("FilterAttribute", StringComparison.OrdinalIgnoreCase)))
        .AsImplementedInterfaces()
        .WithScopedLifetime();
});

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<SignUpRequestValidator>();

// Mappers
builder.Services.AddAutoMapper(typeof(DefaultProfile));

// Handlers
builder.Services.Scan(scan =>
{
    scan.FromTypes()
        .FromAssemblyOf<SignUpHandler>()
        .AddClasses(c => c.Where(t => t.Name.EndsWith("Handler", StringComparison.OrdinalIgnoreCase)))
        .AsImplementedInterfaces()
        .WithTransientLifetime();
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:IssuerSigningKey"]!)),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true,
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            // If the request is for the SignalR hub, include the access token
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/events"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var corsPolicy = "CorsPolicy";
builder.Services.AddCors(options => options.AddPolicy(corsPolicy, corsBuilder => {
    var validOrigins = builder.Configuration.GetSection("AllowedOrigins:Default").Get<List<string>>();

    if (builder.Environment.IsDevelopment())
    {
        validOrigins?.AddRange(builder.Configuration.GetSection("AllowedOrigins:Dev").Get<List<string>>()!);
    }

    corsBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithOrigins([.. validOrigins!]);
}));

// Memory Cache
builder.Services.AddMemoryCache(options =>
{
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(builder.Configuration.GetSection("MemoryCache:ExpirationScanFrequencyInMinutes").Get<int>());
});

// Http Clients
builder.Services.AddHttpClient("MarvelGaming", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MG:BaseUrl"]!);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
});

if (Convert.ToBoolean(builder.Configuration["Swagger:Enabled"]))
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Gambit API", Version = "v1" });

        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
    });
}

builder.Services.AddSignalR()
    .AddNewtonsoftJsonProtocol();

// Rate Limiting
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

if (Convert.ToBoolean(builder.Configuration["Swagger:Enabled"]))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIpRateLimiting();

app.UseHttpsRedirection();

app.UseCors(corsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<EventHub>("/hubs/events");    

app.MapControllers();

app.Run();
