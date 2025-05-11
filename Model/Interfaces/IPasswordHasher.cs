namespace Model.Interfaces
{
    /// <summary>
    /// Defines methods for hashing and verifying passwords.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes the provided plain text password.
        /// </summary>
        /// <param name="plainText">The plain text password to hash.</param>
        /// <returns>A task that returns the hashed password as a string.</returns>
        Task<string> Hash(string plainText);

        /// <summary>
        /// Verifies if the provided plain text password matches the hashed password.
        /// </summary>
        /// <param name="hashedPassword">The hashed password to compare against.</param>
        /// <param name="plainTextPassword">The plain text password to verify.</param>
        /// <returns>A task that returns true if the passwords match; otherwise, false.</returns>
        Task<bool> Verify(string hashedPassword, string plainTextPassword);
    }
}