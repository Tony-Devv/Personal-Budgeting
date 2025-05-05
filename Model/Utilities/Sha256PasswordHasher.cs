using Model.Interfaces;

namespace Model.Utilities;

public class Sha256PasswordHasher : IPasswordHasher
{
    public Task<string> Hash(string plainText)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Verify(string hashedPassword, string plainTextPassword)
    {
        throw new NotImplementedException();
    }
}