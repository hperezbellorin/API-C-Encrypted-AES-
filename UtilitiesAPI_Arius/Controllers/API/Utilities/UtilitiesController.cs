using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

namespace UtilitiesAPI_Arius.Controllers.API.Utilities
{
    public class UtilitiesController : ApiController
    {
       
     
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/Utilities/Babylon")]
        [System.Web.Http.HttpPost]
        public string Babylon(strOcbjet eObject)
        { string str_encrypted = string.Empty;
            try
            {
            str_encrypted = Str_Encrypted(eObject.MyString,eObject.MyKey);
            }
          
        catch (Exception ex)
            {

             //   throw;
            }
            return str_encrypted;
         }

      
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("api/Utilities/Eden")]
        [System.Web.Http.HttpPost]
      
        public string Eden(strOcbjet eObject)
        {
            string Str_Decrypted = string.Empty;
            try
            {
            Str_Decrypted = DecryptAES(eObject.MyString,eObject.MyKey);

            }
            catch (Exception)
            {

                throw;
            }
            return Str_Decrypted;
           
        }

        public class strOcbjet
        {
            public string MyString { get; set; }
            public string MyKey { get; set; }
        }

        static string aes_key = "AXe8YwuIn1zxt3FPWTZFlAa14EHdPAdN9FaZ9RQWihc=";
        static string aes_iv = "bsxnWolsAyO7kCfWuyrnqg==";
        public string Str_Encrypted(string original,string MyKey)
        {

            string AriusKey = "6989f605107b8c407843579fe71ec351";
            string str_encrypted = string.Empty;

            if (AriusKey.Equals(MyKey))
            {

                try
                {
                    // Encrypt the string to an array of bytes.
                    byte[] encrypted = EncryptStringToBytes(original, Convert.FromBase64String(aes_key), Convert.FromBase64String(aes_iv));

                    // Decrypt the bytes to a string.
                    //  string roundtrip = DecryptStringFromBytes(encrypted, Convert.FromBase64String(aes_key), Convert.FromBase64String(aes_iv));

                    // Encrypt the string to an array of bytes.
                    str_encrypted = EncryptAES(original, MyKey);

                    // Decrypt the bytes to a string.
                    // string str_roundtrip = DecryptAES(str_encrypted);

                    return str_encrypted;

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
            return str_encrypted;
        }

        public static string EncryptAES(string plainText,string MyKey)
        {
         
            byte[] encrypted;

          
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = Convert.FromBase64String(aes_key);
                aes.IV = Convert.FromBase64String(aes_iv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
              //  aes.BlockSize = 128;

                ICryptoTransform enc = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }

                        encrypted = ms.ToArray();
                    }
                }
            }
                return Convert.ToBase64String(encrypted);
            
        }

        public static string DecryptAES(string encryptedText,string Key)
        {
            string decrypted = null;
            string AriusKey = "6989f605107b8c407843579fe71ec351";
            if (AriusKey.Equals(Key))
            {
                // Decrypt the bytes to a string.
                //  string roundtrip = DecryptStringFromBytes(encrypted, Convert.FromBase64String(aes_key), Convert.FromBase64String(aes_iv));
                byte[] cipher = Convert.FromBase64String(encryptedText);
                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    aes.Key = Convert.FromBase64String(aes_key);
                    aes.IV = Convert.FromBase64String(aes_iv);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    //   aes.BlockSize = 128;

                    ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream(cipher))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                decrypted = sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            return decrypted;
        }
        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

        private string _stringToHex(string s)
        {
            var sb = new StringBuilder();
            foreach (var t in s)
            {
                sb.Append(Convert.ToInt32(t).ToString("x") + " ");
            }
            return sb.ToString();
        }
     
      
    }
}