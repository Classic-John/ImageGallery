using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models;

namespace Datalayer.Interfaces
{
    public interface IEncryptionAndDecryptionService
    {
        public SecurityItems DeriveKeyAndInitializationVectorFromPassword(string? password, byte[]? salt = null);
        public byte[] EncryptImage(byte[]? image, byte[] key, byte[] iv);
        public byte[] DecryptImage(byte[]? image, byte[] key, byte[] iv);
        public string HashPassword(string? insertedPassword);
        public bool VerifyHashedPassword(string? insertedPassword, string? encryptedPassword);
    }
}
