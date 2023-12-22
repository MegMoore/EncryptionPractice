using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EncryptionPractice.Data;
using System.Security.Cryptography;
using EncryptionPractice.Controllers;
using static EncryptionPractice.Controllers.KidsController;
using System.Text;
using EncryptionPractice.Models;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EncryptionPracticeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevDb"))

            .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

