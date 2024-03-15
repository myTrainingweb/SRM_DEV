using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IP.Classess
{
    /// <summary>
    /// A very basic encryption method
    /// </summary>
    public class BasicEncrypt
    {
        private static readonly Lazy<BasicEncrypt> lazy =
            new Lazy<BasicEncrypt>(() => new BasicEncrypt());

        public static BasicEncrypt Instance { get { return lazy.Value; } }

        // random bytes
        private static byte[] key = { 138, 117, 29, 21, 124, 36, 185, 155, 214, 47, 73, 12, 93, 1, 130, 59, 71, 92, 175, 144, 127, 168, 87, 76, 45, 22, 54, 45, 131, 236, 53, 92 };
        private static byte[] vector = { 158, 43, 16, 212, 15, 224, 238, 62, 112, 15, 67, 23, 14, 55, 23, 45 };

        private readonly ICryptoTransform encryptor;
        private readonly ICryptoTransform decryptor;
        private UTF8Encoding stringEncoder;


        public BasicEncrypt()
        {
            RijndaelManaged rm = new RijndaelManaged();
            encryptor = rm.CreateEncryptor(key, vector);
            decryptor = rm.CreateDecryptor(key, vector);
            stringEncoder = new UTF8Encoding();
        }

        public string Encrypt(string unencrypted) { return Convert.ToBase64String(Encrypt(stringEncoder.GetBytes(unencrypted))); }

        public string Decrypt(string encrypted) { return stringEncoder.GetString(Decrypt(Convert.FromBase64String(encrypted))); }

        public byte[] Encrypt(byte[] buffer) { return Transform(buffer, encryptor); }

        public byte[] Decrypt(byte[] buffer) { return Transform(buffer, decryptor); }

        protected byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return ms.ToArray();
        }
    }
}