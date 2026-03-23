using BackpackStoreFS.Data;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Apex Shop API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token into field (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
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

string connectionString = builder.Configuration.GetConnectionString("BackpackStoreDB")
    ?? throw new InvalidOperationException("Connection string 'BackpackStoreDB' not found.");

builder.Services.AddDbContext<BackpackContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<BackpackContext>()
.AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing in appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddScoped<IBackpackService, BackpackService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();