namespace Alaris.API.Crypt
{
    /// <summary>
    /// An interface which should be implemented by all encryption
    /// algorithm implementations in Alaris.
    /// </summary>
    public interface IEncrpytionAlgorithm
    {
        /// <summary>
        /// Encrpyts the specified string using this algorithm.
        /// </summary>
        /// <param name="plain">The text to encrypt.</param>
        /// <param name="pass">Password to encrypt.</param>
        /// <param name="keySize">Key size.</param>
        /// <returns>The encrpyted string.</returns>
        string EncrpytString(string plain, string pass, int keySize);

        /// <summary>
        /// Decrypts an encrypted string using this algorithm.
        /// </summary>
        /// <param name="encrypted">The encrypted string</param>
        /// <param name="pass">Password to decrypt.</param>
        /// <param name="keySize">Key Size.
        /// </param>
        /// <returns>The decrypted, original (plain) string.</returns>
        string DecryptString(string encrypted, string pass, int keySize);
    }
}
