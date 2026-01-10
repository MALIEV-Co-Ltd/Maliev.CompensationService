using Maliev.CompensationService.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Maliev.CompensationService.Infrastructure.Data;

/// <summary>
/// EF Core Value Converter for encrypting and decrypting fields at rest
/// </summary>
public class EncryptionValueConverter : ValueConverter<string?, string?>
{
    public EncryptionValueConverter(IEncryptionService encryptionService, ConverterMappingHints? mappingHints = null)
        : base(
            v => encryptionService.Encrypt(v),
            v => encryptionService.Decrypt(v),
            mappingHints)
    {
    }
}

/// <summary>
/// EF Core Value Converter for encrypting and decrypting decimal fields at rest
/// </summary>
public class DecimalEncryptionValueConverter : ValueConverter<decimal, string?>
{
    public DecimalEncryptionValueConverter(IEncryptionService encryptionService, ConverterMappingHints? mappingHints = null)
        : base(
            v => encryptionService.Encrypt(v.ToString()),
            v => decimal.Parse(encryptionService.Decrypt(v) ?? "0"),
            mappingHints)
    {
    }
}