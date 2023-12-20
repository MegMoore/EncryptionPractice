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


    static void Main()
    {
        // Initialize a secure encryption key (in a real scenario, this should be handled securely)
        byte[] encryptionKey = Encoding.UTF8.GetBytes("ThisIsASecretKey123");

        // Create an instance of the DatabaseService
        DataService dataService = new DataService(encryptionKey);

        // Example: Save encrypted person to the database
        Person personToSave = new Person
        {
            FirstName = "Alice",
            LastName = "Smith",
            EmailAddress = "alice.smith@example.com"
        };
        dataService.SaveEncryptedPerson(personToSave);

        // Example: Retrieve and decrypt person from the database
        Person retrievedPerson = dataService.RetrieveDecryptedPerson(personToSave);
        if (retrievedPerson != null)
        {
            Console.WriteLine($"Retrieved person: {retrievedPerson.FirstName} {retrievedPerson.LastName} ({retrievedPerson.EmailAddress})");
        }
        else
        {
            Console.WriteLine("No person retrieved from the database.");
        }
        Console.WriteLine($"Retrieved person: {retrievedPerson?.FirstName} {retrievedPerson?.LastName} ({retrievedPerson?.EmailAddress})");
        }


//Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
