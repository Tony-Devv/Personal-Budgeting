using System.Security.Cryptography;
using Model.Interfaces;

namespace Model.Utilities;

public class Sha512PasswordHasher : IPasswordHasher
{
    private const int HASHSIZE = 32;
    private const int SALTSIZE = 16;
    private const int ITERATIONS = 100000;

    public async Task<string> Hash(string plainText)
    {
        byte[] salt = new byte[SALTSIZE];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        using (var pbdkf2 = new Rfc2898DeriveBytes(plainText, salt, ITERATIONS))
        {
            byte[] key = pbdkf2.GetBytes(HASHSIZE);
            using (var sha512 = SHA512.Create())
            {
                byte[] hash = sha512.ComputeHash(key);
                return $"{Convert.ToBase64String(hash)}-{Convert.ToBase64String(salt)}";
            }
        }

    }

    public Task<bool> Verify(string hashedPassword, string plainTextPassword)
    {
        string[] parts = hashedPassword.Split('-');
        if (parts.Length != 2)
            return Task.FromResult(false);

        byte[] hash = Convert.FromBase64String(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);

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