using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Maliev.CompensationService.Application.Interfaces;

namespace Maliev.CompensationService.Infrastructure.Data;

/// <summary>
/// EF Core value converter for transparent encryption/decryption of string values.
/// </summary>
public class StringEncryptionValueConverter : ValueConverter<string?, string?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringEncryptionValueConverter"/> class.
    /// </summary>
    /// <param name="encryptionService">The encryption service.</param>
    public StringEncryptionValueConverter(IEncryptionService encryptionService)
        : base(
            v => string.IsNullOrEmpty(v) ? v : encryptionService.Encrypt(v),
            v => string.IsNullOrEmpty(v) ? v : encryptionService.Decrypt(v))
    {
    }
}