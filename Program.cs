/*
Developer: Abdullah Alshamrani
FirstBrick Project
VillaCapital
*/



using FirstBrickAPI.Data; //containes entity framwork database
using FirstBrickAPI.Models;
using FirstBrickAPI.Services; // Include the namespace for RabbitMqProducer
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens; //provides needed classes for handling JWT tokens.
using Microsoft.OpenApi.Models;
using System.Text; //tools for encoding and decoding text

var builder = WebApplication.CreateBuilder(args);

// Loading the Configuration
var configuration = builder.Configuration;

// Bind JWT Settings
var jwtSettings = new JwtSettings();
configuration.GetSection("JwtSettings").Bind(jwtSettings);
var key = Encoding.ASCII.GetBytes(jwtSettings.Key ?? throw new InvalidOperationException("JWT Key is missing in configuration."));
// the above line of code Converts the JWT secret key (from appsettings.json) into a byte array for use in token signing. Throws an exception if the key is not provided.

// Database Context and configration to connect with the databse information in appsettings.json
builder.Services.AddDbContext<FirstBrickContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Database connection string is missing!")));

// Adding JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true; //saving the token in the request for reuse.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ClockSkew = TimeSpan.Zero
    };
});

// Adding RabbitMqProducer Service
builder.Services.AddSingleton<RabbitMqProducer>();

// Adding Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Adds support for controllers, enabling API endpoints.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FirstBrickAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
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
            new string[] { }
        }
    });
});

// Build
var app = builder.Build();

// Middleware configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); //redirect to https for more security 
app.UseAuthentication(); // Adds authentication middleware to validate tokens for incoming requests.
app.UseAuthorization(); // Adds authorization middleware to enforce role-based or policy-based access control.
app.MapControllers();// Maps API controller endpoints to the request pipeline.

// Run Application
app.Run();
