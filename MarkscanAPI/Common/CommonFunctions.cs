using Dapper;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Web;



namespace MarkscanAPI.Common
{
    public class CommonFunctions : BaseFields
    {
        public static async Task<DateTime> ConvertUtcToIst(DateTime? dt)
        {
            DateTime utcdate = (dt == null) ? DateTime.UtcNow : dt.Value;
            var istdate = TimeZoneInfo.ConvertTimeFromUtc(utcdate,
            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            return istdate;
        }
        public static string EncryptString(string text, string keyString = "F326C8D3278C45431069B5727695D4F1")
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aesAlg = Aes.Create();
            using var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);

            }

            var iv = aesAlg.IV;
            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            return Convert.ToBase64String(result);
        }

        public static string DecryptString(string cipherText, string keyString = "F326C8D3278C45431069B5727695D4F1")
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aesAlg = Aes.Create();
            if (cipherText.Length == 64)
            {
                aesAlg.Padding = PaddingMode.Zeros;
            }
            else
            {
                aesAlg.Padding = PaddingMode.ISO10126;
            }
            using var decryptor = aesAlg.CreateDecryptor(key, iv);
            string result;
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                result = srDecrypt.ReadToEnd();
            }
            return result;
        }

    }
}
