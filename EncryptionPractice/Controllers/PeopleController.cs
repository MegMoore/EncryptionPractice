using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EncryptionPractice.Data;
using EncryptionPractice.Models;
using System.Text;
using System.Security.Cryptography;


namespace EncryptionPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly EncryptionPracticeContext _dbContext;
        private readonly byte[] encryptionKey;

        public PeopleController(EncryptionPracticeContext dbContext)
        {
            _dbContext = dbContext;

            // Initialize the encryption key securely (this is just an example, use a secure method for production)
            encryptionKey = Encoding.UTF8.GetBytes("ThisIsASecretKey1234567890123456");
        }

        [HttpGet("getall")]
        public ActionResult<IEnumerable<Person>> GetAllPersons()
        {
            try
            {
                // Retrieve all persons from the database
                List<Person> persons = _dbContext.Persons.ToList();

                // Decrypt email addresses before returning them
                List<Person> decryptedPersons = persons.Select(DecryptPerson).ToList();
                return Ok(decryptedPersons);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving persons: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Person> GetPersonById(int id)
        {
            try
            {
                // Retrieve the person from the database
                Person person = _dbContext.Persons.FirstOrDefault(p => p.Id == id);

                if (person == null)
                {
                    return NotFound("Person not found");
                }

                // Decrypt the person before returning it
                Person decryptedPerson = DecryptPerson(person);
                return Ok(decryptedPerson);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving person: {ex.Message}");
            }
        }

        [HttpPost("create")]
        public ActionResult<Person> CreatePerson([FromBody] Person person)
        {
            try
            {
                // Encrypt the person's data before storing it in the database
                Person encryptedPerson = EncryptPerson(person);
                _dbContext.Persons.Add(encryptedPerson);
                _dbContext.SaveChanges();

                return CreatedAtAction(nameof(GetPersonById), new { id = encryptedPerson.Id }, encryptedPerson);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating person: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Person> UpdatePerson(int id, [FromBody] Person updatedPerson)
        {
            try
            {
                // Find the person in the database
                Person existingPerson = _dbContext.Persons.FirstOrDefault(p => p.Id == id);

                if (existingPerson == null)
                {
                    return NotFound("Person not found");
                }

                // Update the person's data
                existingPerson.FirstName = updatedPerson.FirstName;
                existingPerson.LastName = updatedPerson.LastName;

                // Encrypt the updated email address
                byte[] encryptedEmail = Encrypt(updatedPerson.EmailAddress, encryptionKey);

                // Convert the encrypted byte array to a string
                existingPerson.EmailAddress = Convert.ToBase64String(encryptedEmail);

                // Save changes to the database
                _dbContext.SaveChanges();

                return Ok(existingPerson);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating person: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePerson(int id)
        {
            try
            {
                // Find and remove the person from the database
                Person personToRemove = _dbContext.Persons.FirstOrDefault(p => p.Id == id);

                if (personToRemove == null)
                {
                    return NotFound("Person not found");
                }

                _dbContext.Persons.Remove(personToRemove);
                _dbContext.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting person: {ex.Message}");
            }
        }

        // Encryption logic
        private Person EncryptPerson(Person person)
        {
            byte[] encryptedEmail = Encrypt(person.EmailAddress, encryptionKey);

            // Convert the encrypted byte[] to a Base64-encoded string (or use another suitable encoding)
            person.EmailAddress = Convert.ToBase64String(encryptedEmail);

            return person;
        }

        private Person DecryptPerson(Person person)
        {
            // Convert the Base64-encoded string to a byte[] before decrypting
            byte[] encryptedEmail = Convert.FromBase64String(person.EmailAddress);

            // Decrypt the byte[] and store the result in person.EmailAddress
            person.EmailAddress = Decrypt(encryptedEmail, encryptionKey);
            return person;
        }

        private byte[] Encrypt(string data, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(data);
                            }
                        }
                    }

                    return msEncrypt.ToArray();
                }
            }
        }

        private string Decrypt(byte[] encryptedData, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                byte[] iv = new byte[aesAlg.BlockSize / 8];
                Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
                aesAlg.IV = iv;

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                        {
                            csDecrypt.Write(encryptedData, iv.Length, encryptedData.Length - iv.Length);
                        }
                    }

                    return Encoding.UTF8.GetString(msDecrypt.ToArray());
                }
            }
        }
    }
}


