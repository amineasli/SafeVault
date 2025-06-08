using BCrypt.Net;

public static class PasswordHelper
{
    // Hash the password securely with bcrypt
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12); // Higher work factor increases security
    }

    // Verify hashed password against user input
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
