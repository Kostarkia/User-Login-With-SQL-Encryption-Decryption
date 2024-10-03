using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace User_Login_With_SQL.Util
{
   public class EncryptionDecryptionUtil
    {

        public static string Encrypt(string source)
        {
            byte[] keyArray;

            byte[] data = Encoding.UTF8.GetBytes(source);

            string key = "SecurityKey";

            var hashMD5Provider = new MD5CryptoServiceProvider();

            keyArray = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));

            hashMD5Provider.Clear();

            var tDESC = new TripleDESCryptoServiceProvider()
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var transform = tDESC.CreateEncryptor();

            tDESC.Clear();

            return Convert.ToBase64String(transform.TransformFinalBlock(data, 0, data.Length));

        }

        public static string Decrypt(string encrypt)
        {
            byte[] keyArray;

            string key = "SecurityKey";

            var hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteBuff = Convert.FromBase64String(encrypt);

            keyArray = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));

            hashMD5Provider.Clear();

            var tDESC = new TripleDESCryptoServiceProvider()
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var decryption = tDESC.CreateDecryptor();

            tDESC.Clear();

            return Encoding.UTF8.GetString(decryption.TransformFinalBlock(byteBuff, 0, byteBuff.Length));
        }

    }
}
