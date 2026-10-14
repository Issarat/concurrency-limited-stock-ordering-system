using System.Security.Cryptography;

namespace LimitedStockOrdering.Api.Authentication;

public sealed class PasswordHashService
{
    private const string Algorithm = "pbkdf2-sha256";
    private const int HashSize = 32;

    public bool Verify(string password, string? encodedHash)
    {
        if (string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(encodedHash))
        {
            return false;
        }

        var parts = encodedHash.Split('$', StringSplitOptions.None);
        if (parts.Length != 5
            || parts[0].Length != 0
            || parts[1] != Algorithm
            || !int.TryParse(parts[2], out var iterations)
            || iterations < 100_000)
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(parts[3]);
            var expectedHash = Convert.FromBase64String(parts[4]);
            if (salt.Length < 16 || expectedHash.Length != HashSize)
            {
                return false;
            }

            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return CryptographicOperations.FixedTimeEquals(
                actualHash,
                expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
