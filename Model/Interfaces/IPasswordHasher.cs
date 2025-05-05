namespace Model.Interfaces;

public interface IPasswordHasher
{
    Task<string> Hash(string plainText);

    Task<bool> Verify(string hashedPassword, string plainTextPassword);
}