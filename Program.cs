// --> make it global so we only have to reference it once if we are using it in multiple placess of the app
global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
using dotnet_rpg.Data;
using dotnet_rpg.Services.Auth;
using dotnet_rpg.Services.CharacterService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Encoding = System.Text.Encoding;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(configs =>
{
    configs.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Description = """Standard Auth header using the Bearer Scheme. Example "bearer {token}" """,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    
    configs.OperationFilter<SecurityRequirementsOperationFilter>();
});

// --> Add Automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Adds a scoped service of the type specified in ICharacterService with an implementation type specified in CharacterService to the specified IServiceCollection.<Interface, Service>
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// auth

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var appSettingsToken = builder.Configuration.GetSection("AppSettings:Token").Value;
        var encodedToken = Encoding.UTF8.GetBytes(appSettingsToken!);
        SymmetricSecurityKey key = new SymmetricSecurityKey(encodedToken);

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// must be above UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
