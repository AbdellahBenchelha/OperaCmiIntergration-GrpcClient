using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CMI_Client
{
    public class AesOperation
    {
        public static Hashtable EncryptString(string key,string plainText)
        {
            Hashtable data = new Hashtable();
            byte[] array;
            Aes AesCrypt = new AesCryptoServiceProvider();

            AesCrypt.Mode = CipherMode.CBC;
            AesCrypt.BlockSize = 128;
            AesCrypt.Padding = PaddingMode.PKCS7;
            AesCrypt.KeySize = 128;
            AesCrypt.GenerateIV();
            ICryptoTransform transform = AesCrypt.CreateEncryptor(ASCIIEncoding.ASCII.GetBytes(key), AesCrypt.IV);

            byte[] encreypted_bytes = transform.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(plainText), 0, plainText.Length);

            data.Add("LS_Date", Convert.ToBase64String(encreypted_bytes));
            data.Add("IV", Convert.ToBase64String(AesCrypt.IV));

            return data;
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
