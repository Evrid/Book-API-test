using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using RedRiverTestKevin.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RedRiverTestKevin;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
}).CreateLogger("MyLogger");



// Add services to the container.
builder.Services.AddControllers();


//builder.Services.AddSingleton(users);

byte[] HexStringToByteArray(string hexString)
{
    int numChars = hexString.Length;
    byte[] bytes = new byte[numChars / 2];
    for (int i = 0; i < numChars; i += 2)
    {
        bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
    }
    return bytes;
}

// Configure JWT authentication
var key = HexStringToByteArray("E7F86A0A49A1C86EAC378F1D93A57C9E71B3C6CFA770A1D33DF9873F0F4C45C9");



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Set to false to allow any issuer
            ValidateAudience = false, // Set to false to allow any audience
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
         
            ClockSkew = TimeSpan.Zero // Remove any clock skew (optional)

           
    };
        logger.LogInformation($"IssuerSigningKey: {Convert.ToBase64String(key)}");
    });
//builder.Logging.LogInformation($"IssuerSigningKey: {IssuerSigningKey}");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the database context to the DI container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable authentication middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();