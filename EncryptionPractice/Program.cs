using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EncryptionPractice.Data;
using System.Security.Cryptography;
using EncryptionPractice.Controllers;
using static EncryptionPractice.Controllers.KidsController;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EncryptionPracticeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevDb") ?? throw new InvalidOperationException("Connection string 'EncryptionPracticeContext' not found.")));



// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

//Encryption
static byte[] Encrypt(string simpletext, byte[] key, byte[] iv)
{
    byte[] cipheredtext;
    using (Aes aes = Aes.Create())
    {
        ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(simpletext);
                }

                cipheredtext = memoryStream.ToArray();
            }
        }
    }
    return cipheredtext;
}
//Decription
static string Decrypt(byte[] cipheredtext, byte[] key, byte[] iv)
{
    string simpletext = String.Empty;
    using (Aes aes = Aes.Create())
    {
        ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
        using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                {
                    simpletext = streamReader.ReadToEnd();
                }
            }
        }
    }
    return simpletext;
}
//Main Method
static void Main(string[] args)
{
    Console.WriteLine("Please enter a username:");
    string username = Console.ReadLine();

    Console.WriteLine("Please enter a password:");
    string HashPassword = Console.ReadLine();

    //Generate the key and IV
    byte[] key = new byte[16];

    byte[] iv = new byte[16];

    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(key);
        rng.GetBytes(iv);
    }

    //Encrypt the password
    byte[] encryptedPassword = Encrypt(HashPassword, key, iv);
    string encryptedPasswordString = Convert.ToBase64String(encryptedPassword);
    Console.WriteLine("Encrypted password: " + encryptedPasswordString);

    //Decrypt the password
    string decryptedPassword = Decrypt(encryptedPassword, key, iv);
    Console.WriteLine("Decrypted password: " + decryptedPassword);
    Console.ReadLine();
}


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
