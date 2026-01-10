using System.Security.Cryptography;
using System.Text;
using Maliev.CompensationService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Maliev.CompensationService.Infrastructure.Services;

/// <summary>
/// Implementation of IEncryptionService using AES-256-GCM
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private const int NonceSize = 12; // 96 bits
    private const int TagSize = 16;   // 128 bits

    public EncryptionService(IConfiguration configuration)
    {
        var keyString = configuration["Encryption:Key"] ?? throw new InvalidOperationException("Encryption key not configured.");
        _key = Convert.FromBase64String(keyString);

        if (_key.Length != 32)
        {
            throw new InvalidOperationException("Encryption key must be 32 bytes (256 bits).");
        }
    }

    public string Encrypt(string? plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return string.Empty;

        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherText = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        using var aesGcm = new AesGcm(_key, TagSize);
        aesGcm.Encrypt(nonce, plainBytes, cipherText, tag);

        var result = new byte[NonceSize + cipherText.Length + TagSize];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(cipherText, 0, result, NonceSize, cipherText.Length);
        Buffer.BlockCopy(tag, 0, result, NonceSize + cipherText.Length, TagSize);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string? cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return string.Empty;

        var fullCipher = Convert.FromBase64String(cipherText);
        
        if (fullCipher.Length < NonceSize + TagSize)
        {
            return string.Empty;
        }

        var nonce = new byte[NonceSize];
        var tag = new byte[TagSize];
        var cipherBytes = new byte[fullCipher.Length - NonceSize - TagSize];

        Buffer.BlockCopy(fullCipher, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(fullCipher, NonceSize, cipherBytes, 0, cipherBytes.Length);
        Buffer.BlockCopy(fullCipher, NonceSize + cipherBytes.Length, tag, 0, TagSize);

        var plainBytes = new byte[cipherBytes.Length];

        using var aesGcm = new AesGcm(_key, TagSize);
        aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }
}