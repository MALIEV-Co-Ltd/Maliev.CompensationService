namespace Maliev.CompensationService.Application.Interfaces;

/// <summary>
/// Service for encrypting and decrypting sensitive data
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the specified plain text
    /// </summary>
    /// <param name="plainText">The text to encrypt</param>
    /// <returns>The encrypted ciphertext in Base64 format</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts the specified ciphertext
    /// </summary>
    /// <param name="cipherText">The encrypted text in Base64 format</param>
    /// <returns>The decrypted plain text</returns>
    string Decrypt(string cipherText);
}