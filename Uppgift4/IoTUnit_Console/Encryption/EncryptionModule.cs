using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IoTUnit_Console.Encryption
{
    internal class EncryptionModule
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("ThisIsASecretKey"); // 16 byte
        private static readonly byte[] Iv = Encoding.UTF8.GetBytes("ThisIsSecretIV12"); // 16 byte

        public static string Encrypt(string plainText)
        {
            using (var aes = new AesManaged())
            {
                var encryptor = aes.CreateEncryptor(Key, Iv);

                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                using var sw = new StreamWriter(cs);

                sw.Write(plainText);
                sw.Close();

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (var aes = new AesManaged())
            {
                var decryptor = aes.CreateDecryptor(Key, Iv);

                using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
        }
    }
}
