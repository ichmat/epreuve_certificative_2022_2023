using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class SecurityManager
    {
        private string? _private_unsafe_key = null;
        private string? _public_unsafe_key = null;

        private static readonly byte[] IV = Encoding.ASCII.GetBytes("abcdef0123456789");

        private byte[]? _sync_key = null;

        public SecurityManager()
        {
            
        }

        public string GenerateUnsafeAsyncKeysAndReturnPublicKey()
        {
            var rsa = new RSACryptoServiceProvider(2048);
            rsa.ToXmlString(true);
            
            _private_unsafe_key = rsa.ToXmlString(true);
            _public_unsafe_key = rsa.ToXmlString(false);
            return _public_unsafe_key;
        }

        public void SetUnsafeAsyncKey(string public_unsafe_key)
        {
            _public_unsafe_key = public_unsafe_key;
        }

        public byte[] EncryptUnsafeKey(string sync_key)
        {
            return EncryptUnsafeKey(Encoding.UTF8.GetBytes(sync_key));
        }

        private byte[] EncryptUnsafeKey(byte[] sync_key)
        {
            if (_public_unsafe_key == null) throw new ArgumentException("no public key setted");
            byte[] result;
            using(var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(_public_unsafe_key);
                result = rsa.Encrypt(sync_key, false);
            }
            return result;
        }

        public string DecryptUnsafeKey(byte[] sync_key)
        {
            if (_private_unsafe_key == null) throw new ArgumentException("no private key setted");
            string result;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(_private_unsafe_key);
                result = Encoding.UTF8.GetString(rsa.Decrypt(sync_key,false));
            }
            return result;
        }


        public string GenerateAndGetSyncKey()
        {
            byte[] key;
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                _sync_key = aes.Key;
                key = aes.Key;
            }
            return Convert.ToBase64String(key);
        }

        public void SetSyncKey(string sync_key)
        {
            _sync_key = Convert.FromBase64String(sync_key);
        }

        public string Encrypt(string data)
        {
            if (_sync_key == null) throw new ArgumentException("no sync key setted");
            using (Aes aes = Aes.Create())
            {
                aes.Key = _sync_key;
                aes.IV = IV;
                var symmetricEncryptor = aes.CreateEncryptor(aes.Key, IV);
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream as Stream, symmetricEncryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream as Stream))
                        {
                            streamWriter.Write(data);
                        }
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        public string Decrypt(string encryptedData)
        {
            if (_sync_key == null) throw new ArgumentException("no sync key setted");
            byte[] buffer = Convert.FromBase64String(encryptedData);
            using (Aes aes = Aes.Create())
            {
                aes.Key = _sync_key;
                aes.IV = IV;
                var decryptor = aes.CreateDecryptor(aes.Key, IV);
                using (var memoryStream = new MemoryStream(buffer))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream as Stream,decryptor, CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream as Stream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
