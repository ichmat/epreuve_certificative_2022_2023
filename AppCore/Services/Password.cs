using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public static class Password
    {
        private const int keySize = 32;
        private const int iterations = 350000;
        private static readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA256;

        public static string HashPasword(string password, out string str_salt)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);
            str_salt = Convert.ToHexString(salt);
            return Convert.ToHexString(hash);
        }

        public static bool VerifyPassword(string password, string hash, string str_salt)
        {
            byte[] salt = Convert.FromHexString(str_salt);
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        }
    }
}
