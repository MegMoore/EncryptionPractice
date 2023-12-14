using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EncryptionPractice.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EncryptionPracticeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EncryptionPracticeContext") ?? throw new InvalidOperationException("Connection string 'EncryptionPracticeContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
