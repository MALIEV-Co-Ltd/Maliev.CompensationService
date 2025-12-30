using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Maliev.CompensationService.Application.Interfaces;

namespace Maliev.CompensationService.Infrastructure.Data;

/// <summary>
/// EF Core value converter for transparent encryption/decryption of decimal values.
/// </summary>
public class EncryptionValueConverter : ValueConverter<decimal, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptionValueConverter"/> class.
    /// </summary>
    /// <param name="encryptionService">The encryption service.</param>
    public EncryptionValueConverter(IEncryptionService encryptionService)
        : base(
            v => encryptionService.Encrypt(v.ToString("F2")),
            v => decimal.Parse(encryptionService.Decrypt(v)))
    {
    }
}