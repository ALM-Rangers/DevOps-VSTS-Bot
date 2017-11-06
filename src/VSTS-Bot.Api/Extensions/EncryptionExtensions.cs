// ———————————————————————————————
// <copyright file="EncryptionExtensions.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains extensions for encrypting and decrypting an object.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains extensions for encrypting and decrypting an object.
    /// </summary>
    public static class EncryptionExtensions
    {
        /// <summary>
        /// Encrypts the target object.
        /// </summary>
        /// <typeparam name="T">The target.</typeparam>
        /// <param name="target">The target object.</param>
        /// <param name="securityKey">The security string.</param>
        /// <returns>Returns the encrypted string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Being closed by disposing the StreamWriter.")]
        public static string Encrypt<T>(this T target, string securityKey)
        {
            var salt = GenerateRandomSalt();
            var toEncrypt = JsonConvert.SerializeObject(target);

            var memStream = new MemoryStream();
            memStream.Write(salt, 0, salt.Length);

            using (var key = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(securityKey), salt, 50000))
            {
                using (var provider = CreateCipher(key))
                {
                    var encryptor = provider.CreateEncryptor();

                    var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);
                    using (var sw = new StreamWriter(cryptoStream))
                    {
                        sw.Write(toEncrypt);
                    }

                    return Convert.ToBase64String(memStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypts the target string.
        /// </summary>
        /// <typeparam name="T">The object</typeparam>
        /// <param name="target">The string to decrypt.</param>
        /// <param name="securityKey">The security string</param>
        /// <returns>An decrypted object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Being closed by disposing the StreamReader.")]
        public static T Decrypt<T>(this string target, string securityKey)
        {
            var salt = new byte[32];
            var toDecrypt = Convert.FromBase64String(target);

            var memStream = new MemoryStream(toDecrypt);
            memStream.Read(salt, 0, salt.Length);

            using (var key = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(securityKey), salt, 50000))
            {
                using (var provider = CreateCipher(key))
                {
                    var decryptor = provider.CreateDecryptor();

                    var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
                    using (var sr = new StreamReader(cryptoStream))
                    {
                        var result = sr.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(result);
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "RundaelManaged is disposed in calling method(s).")]
        private static RijndaelManaged CreateCipher(DeriveBytes key)
        {
            var cipher = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CFB,
                Padding = PaddingMode.PKCS7
            };
            cipher.Key = key.GetBytes(cipher.KeySize / 8);
            cipher.IV = key.GetBytes(cipher.BlockSize / 8);

            return cipher;
        }

        private static byte[] GenerateRandomSalt()
        {
            var data = new byte[32];

            using (var rng = new RNGCryptoServiceProvider())
            {
                for (var i = 0; i < 10; i++)
                {
                    // Fill the buffer with the generated data
                    rng.GetBytes(data);
                }
            }

            return data;
        }
    }
}