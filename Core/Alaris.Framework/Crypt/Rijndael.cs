using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Alaris.Framework.Crypt
{
    /// <summary>
    /// Implementation of the secure Rijndael algorithm (also known as the AES encryption)
    /// </summary>
    public class Rijndael : IEncrpytionAlgorithm
    {
        private const string Pasword = "alaris007";
        private const string Salt = "@l@R1sAlT";
        private const string InitVector = "@1A2c5D4r5F4g7H3";

        /// <summary>
        /// Encrpyts the specified string using this algorithm.
        /// </summary>
        /// <param name="plain">The text to encrypt.</param>
        /// <param name="pass">Password used to encrpyt.</param>
        /// <param name="keySize">Size of the key (e.g 128, 256)</param>
        /// <returns>The encrpyted string.</returns>
        public static string EncryptString(string plain, string pass = Pasword, int keySize = 256)
        {
            return (AESEncryption.Encrypt(plain, Pasword, Salt, 2, InitVector, 256));
        }

        #region IEncryptionAlgorithm

        /// <summary>
        /// Encrpyts the specified string using this algorithm.
        /// </summary>
        /// <param name="plain">The text to encrypt.</param>
        /// <param name="pass">Password to encrypt.</param>
        /// <param name="keySize">Key size.</param>
        /// <returns>The encrpyted string.</returns>
        public string EncrpytString(string plain, string pass, int keySize)
        {
            return EncryptString(plain, pass, keySize);
        }

        /// <summary>
        /// Decrypts an encrypted string using this algorithm.
        /// </summary>
        /// <param name="encrypted">The encrypted string</param>
        /// <param name="pass">Password to decrypt.</param>
        /// <param name="keySize">Key Size.
        /// </param>
        /// <returns>The decrypted, original (plain) string.</returns>
        string IEncrpytionAlgorithm.DecryptString(string encrypted, string pass, int keySize)
        {
            return DecryptString(encrypted, pass, keySize);
        }

        #endregion

        /// <summary>
        /// Encrpyts the specified string using this algorithm.
        /// </summary>
        /// <param name="cipherText">
        /// The text to decrypt.
        /// </param>
        /// <param name="pass">Password used to decrypt.</param>
        /// <param name="keySize">Key size of the cipher.</param>
        /// <returns>The encrpyted string.</returns>
        public static string DecryptString(string cipherText, string pass = Pasword, int keySize = 256)
        {
            return (AESEncryption.Decrypt(cipherText, Pasword, Salt, 2, InitVector, 256));
        }
    }
    

        /// <summary>
    /// Utility class that handles encryption
    /// </summary>
    public static class AESEncryption
    {
        #region Static Functions

            /// <summary>
            /// Encrypts a string
            /// </summary>
            /// <param name="plainText">Text to be encrypted</param>
            /// <param name="password">Password to encrypt with</param>
            /// <param name="salt">Salt to encrypt with</param>
            /// <param name="passwordIterations">Number of iterations to do</param>
            /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
            /// <param name="keySize">Can be 128, 192, or 256</param>
            /// <returns>An encrypted string</returns>
            public static string Encrypt(string plainText, string password, string salt, int passwordIterations, string initialVector, int keySize)
            {
                if (string.IsNullOrEmpty(plainText))
                    return "";

                var initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
                var saltValueBytes = Encoding.ASCII.GetBytes(salt);
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                var derivedPassword = new Rfc2898DeriveBytes(password, saltValueBytes, passwordIterations);
                var keyBytes = derivedPassword.GetBytes(keySize / 8);
                var symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};

                byte[] cipherTextBytes;

                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes))
                {
                    using (var memStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            cipherTextBytes = memStream.ToArray();
                            memStream.Close();
                            cryptoStream.Close();
                        }
                    }
                }

                symmetricKey.Clear();
                return Convert.ToBase64String(cipherTextBytes);
            }

            /// <summary>
            /// Decrypts a string
            /// </summary>
            /// <param name="cipherText">Text to be decrypted</param>
            /// <param name="password">Password to decrypt with</param>
            /// <param name="salt">Salt to decrypt with</param>
            /// <param name="passwordIterations">Number of iterations to do</param>
            /// <param name="initialVector">Needs to be 16 ASCII characters long</param>
            /// <param name="keySize">Can be 128, 192, or 256</param>
            /// <returns>A decrypted string</returns>
            public static string Decrypt(string cipherText, string password,
            string salt,
            int passwordIterations, string initialVector,
            int keySize)
        {
            if (string.IsNullOrEmpty(cipherText))
                return "";

            var initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            var saltValueBytes = Encoding.ASCII.GetBytes(salt);
            var cipherTextBytes = Convert.FromBase64String(cipherText);

            var derivedPassword = new Rfc2898DeriveBytes(password, saltValueBytes, passwordIterations);
            var keyBytes = derivedPassword.GetBytes(keySize / 8);

            var symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};

            var plainTextBytes = new byte[cipherTextBytes.Length];

            int byteCount;

            using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes))
            {
                using (var memStream = new MemoryStream(cipherTextBytes))
                {
                    using (var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                    {

                        byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        memStream.Close();
                        cryptoStream.Close();
                    }
                }
            }
            symmetricKey.Clear();
            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }

        #endregion
    }
}
