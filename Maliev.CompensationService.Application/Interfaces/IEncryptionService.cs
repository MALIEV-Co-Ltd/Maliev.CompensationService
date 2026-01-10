namespace Maliev.CompensationService.Application.Interfaces;

/// <summary>
/// Service for encrypting and decrypting sensitive data
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the provided plain text
    /// </summary>
    /// <param name="plainText">The text to encrypt</param>
    /// <returns>The encrypted base64 string</returns>
    string Encrypt(string? plainText);

    /// <summary>
    /// Decrypts the provided cipher text
    /// </summary>
    /// <param name="cipherText">The base64 encrypted string</param>
    /// <returns>The decrypted plain text</returns>
    string Decrypt(string? cipherText);
}
