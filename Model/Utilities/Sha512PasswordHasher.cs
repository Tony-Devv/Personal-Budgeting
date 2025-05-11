using System.Security.Cryptography;
using Model.Interfaces;

namespace Model.Utilities;

/// <summary>
/// Implements a password hashing and verification mechanism using PBKDF2 and SHA-512.
/// This provides a secure way to store and verify passwords.
/// </summary>
public class Sha512PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// The size (in bytes) of the final derived key before hashing.
    /// </summary>
    private const int HASHSIZE = 32;

    /// <summary>
    /// The size (in bytes) of the randomly generated salt.
    /// </summary>
    private const int SALTSIZE = 16;

    /// <summary>
    /// The number of iterations for the PBKDF2 function.
    /// A higher number increases computational cost, improving resistance to brute-force attacks.
    /// </summary>
    private const int ITERATIONS = 100000;

    /// <summary>
    /// Asynchronously generates a secure hash for the given plain text password.
    /// The process includes generating a random salt, deriving a key using PBKDF2,
    /// then hashing it with SHA-512.
    /// </summary>
    /// <param name="plainText">The plaintext password to hash.</param>
    /// <returns>
    /// A task that returns a string containing the Base64-encoded SHA-512 hash and salt, separated by a hyphen.
    /// </returns>
    public async Task<string> Hash(string plainText)
    {
        // Generate a random salt
        byte[] salt = new byte[SALTSIZE];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        // Derive a key from the password and salt using PBKDF2
        using (var pbdkf2 = new Rfc2898DeriveBytes(plainText, salt, ITERATIONS))
        {
            byte[] key = pbdkf2.GetBytes(HASHSIZE);

            // Hash the derived key using SHA-512
            using (var sha512 = SHA512.Create())
            {
                byte[] hash = sha512.ComputeHash(key);

                // Combine the hash and salt for storage
                return $"{Convert.ToBase64String(hash)}-{Convert.ToBase64String(salt)}";
            }
        }
    }

    /// <summary>
    /// Asynchronously verifies whether a plaintext password matches the given hashed password.
    /// </summary>
    /// <param name="hashedPassword">The hashed password with salt, in the format 'hash-salt'.</param>
    /// <param name="plainTextPassword">The plaintext password to verify.</param>
    /// <returns>
    /// A task that returns true if the password matches the hash; otherwise, false.
    /// </returns>
    public Task<bool> Verify(string hashedPassword, string plainTextPassword)
    {
        string[] parts = hashedPassword.Split('-');
        if (parts.Length != 2)
            return Task.FromResult(false);

        byte[] hash = Convert.FromBase64String(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);

        // Derive key using the same salt and compare the resulting hash
        using (var pbdkf2 = new Rfc2898DeriveBytes(plainTextPassword, salt, ITERATIONS))
        {
            byte[] key = pbdkf2.GetBytes(HASHSIZE);
            using (var sha512 = SHA512.Create())
            {
                byte[] enteredHash = sha512.ComputeHash(key);
                bool isMatch = AreHashesEqual(enteredHash, hash);
                return Task.FromResult(isMatch);
            }
        }
    }

    /// <summary>
    /// Compares two byte arrays in constant time to prevent timing attacks.
    /// </summary>
    /// <param name="a">The first byte array.</param>
    /// <param name="b">The second byte array.</param>
    /// <returns>True if both arrays are equal; otherwise, false.</returns>
    private static bool AreHashesEqual(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
                return false;
        }

        return true;
    }
}
