using CoreApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        builder => builder.WithOrigins("https://localhost:7074")  // Allow Blazor app URL
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, //  Notes : Ensures the token was issued by a trusted issuer.
            ValidateAudience = true, // This ensures that the token is intended for the audience that it was issued to.
            ValidateLifetime = true, // This ensures that the token is not expired.
            ValidateIssuerSigningKey = true, // This ensures that the key used to sign the token is valid.
            ValidIssuer = "https://localhost:7077", // The issuer is the authority that issues the token.
            ValidAudience = "https://localhost:7074",  // The audience is the intended recipient of the token.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345_super_secret_256bit_key")) // The key used to sign the token.
        };
    });

// Register DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Use CORS
app.UseCors("AllowBlazorApp");

app.UseHttpsRedirection();

// ************  Add Authentication and Authorization middleware  ******************
app.UseAuthentication();  // This enables JWT authentication
app.UseAuthorization();

app.MapControllers();

app.Run();
