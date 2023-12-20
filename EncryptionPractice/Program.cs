using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EncryptionPractice.Data;
using System.Security.Cryptography;
using EncryptionPractice.Controllers;
using static EncryptionPractice.Controllers.KidsController;
using System.Text;
using EncryptionPractice.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EncryptionPracticeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevDb") ?? throw new InvalidOperationException("Connection string 'EncryptionPracticeContext' not found.")));



// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

//Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
