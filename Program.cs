// --> make it global so we only have to reference it once if we are using it in multiple placess of the app
global using AutoMapper;
using dotnet_rpg.Services.CharacterService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --> Add Automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Adds a scoped service of the type specified in ICharacterService with an implementation type specified in CharacterService to the specified IServiceCollection.<Interface, Service>
builder.Services.AddScoped<ICharacterService, CharacterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
