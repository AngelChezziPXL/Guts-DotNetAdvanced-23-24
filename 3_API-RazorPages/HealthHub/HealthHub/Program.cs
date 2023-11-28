
using HealthHub.AppLogic;
using HealthHub.Infrastructure;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HealthHubDbContext>(options =>
{
    string connectionString = builder.Configuration["ConnectionStrings:HealthHubDbConnection"];
    object value = options.UseSqlServer(connectionString);

});
builder.Services.AddScoped<IDoctorsRepository, DoctorsRepository>();
IServiceCollection serviceCollection = builder.Services.AddScoped<IDoctorsRepository,DoctorsRepository>();

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
