using Top250Movies.Interfaces;
using Top250Movies.Repository;
using Top250Movies.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHostedService<MyHostedService>();
builder.Services.AddHostedService<MyBackgroundService>();
builder.Services.AddScoped<IMovieDB, ConnectionStringAndAPIKeyService>();
builder.Services.AddHttpClient<IDBTables, CreateDBTablesRepository>();
builder.Services.AddScoped<IMovies, GetIMDBDataRepository>();
builder.Services.AddScoped<IUpdateData, UpdateDataRepository>();

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
