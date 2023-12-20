using EncryptionPractice.Models;
using System;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;


public class DataService
{
    private readonly byte[] encryptionKey; // Initialize this key securely

    public DataService(byte[] encryptionKey)
    {
        this.encryptionKey = encryptionKey;
    }

    public Person SaveEncryptedPerson(Person person)
    {
        // Encrypt sensitive properties before saving to the database
        byte[] EmailAddress = Encrypt(person.EmailAddress, encryptionKey);
        
        string encryptedString = Convert.ToBase64String(EmailAddress); // Correct conversion


        return person;
    }


    public Person RetrieveDecryptedPerson(Person person)
    {
        // Decrypt sensitive properties after retrieval from the database
        string decryptedString = Decrypt(person.EmailAddress, encryptionKey);
       

        return person;
    }

    private string Decrypt(string emailAddress, byte[] encryptionKey)
    {
        throw new NotImplementedException();
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


