using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Interfaces;
using System.Security.Cryptography;
using BCrypt.Net;

namespace Core.Services
{
    public class EncryptionAndDecryptionService : IEncryptionAndDecryptionService
    {
        public byte[] DecryptImage(byte[]? image, byte[] key, byte[] iv)
        {
            byte[] decryptedImage;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor = aes.CreateDecryptor();
                using (MemoryStream memoryStream = new(image))
                {
                    using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream output = new())
                        {
                            cryptoStream.CopyTo(output);
                            decryptedImage = output.ToArray();
                        }
                    }
                }
            }
            return decryptedImage;
        }

        public byte[] EncryptImage(byte[] image, byte[] key, byte[] iv)
        {
            byte[] encryptedImage;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream memoryStream = new())
                {
                    using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(image, 0, image.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    encryptedImage = memoryStream.ToArray();
                }
            }
            return encryptedImage;
        }
        public (byte[], byte[]) GenerateKeyAndInitializationVector()
        {
            byte[] iv = new byte[16];
            byte[] key = new byte[16];

            using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(key);
                generator.GetBytes(iv);
            }
            return (key, iv);
        }

        public string HashPassword(string? insertedPassword)
            => BCrypt.Net.BCrypt.HashPassword(insertedPassword, workFactor: 16);


        public bool VerifyHashedPassword(string? insertedPassword, string? encryptedPassword)
            => BCrypt.Net.BCrypt.Verify(insertedPassword, encryptedPassword);

    }
}
