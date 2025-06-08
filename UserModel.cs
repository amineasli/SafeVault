public record UserModel(string Username, string Password, string? Role = null)
{
    // Optional role parameter, defaults to null
    // If not provided, the user will be assigned the default "User" role
};
