using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class AppEndpoints
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapPost("/register", async (UserManager<AppUser> userManager, AppDbContext dbContext, HttpContext context) =>
        {
            var request = await context.Request.ReadFromJsonAsync<UserModel>();
            if (request is null || string.IsNullOrWhiteSpace(request.Password))
                return Results.BadRequest("Invalid input");

            // Sanitize input
            var sanitizedUsername = InputSanitizer.Sanitize(request.Username);
            var sanitizedPassword = InputSanitizer.Sanitize(request.Password);

            var user = new AppUser { UserName = sanitizedUsername };
            var result = await userManager.CreateAsync(user, sanitizedPassword);

            await dbContext.SaveChangesAsync(); // Ensure safe database interaction

            return result.Succeeded ? Results.Ok("User registered successfully") : Results.BadRequest(result.Errors);
        });

        app.MapPost("/login", async (SignInManager<AppUser> signInManager, AppDbContext dbContext, HttpContext context) =>
        {
            var request = await context.Request.ReadFromJsonAsync<UserModel>();
            if (request is null || string.IsNullOrWhiteSpace(request.Password))
                return Results.BadRequest("Invalid input");

            // Sanitize input
            var sanitizedUsername = InputSanitizer.Sanitize(request.Username);
            var sanitizedPassword = InputSanitizer.Sanitize(request.Password);

            var userExists = await dbContext.Users
                .Where(u => u.UserName == sanitizedUsername)
                .FirstOrDefaultAsync(); // Parameterized query

            if (userExists == null)
                return Results.Unauthorized();

            var result = await signInManager.PasswordSignInAsync(sanitizedUsername, sanitizedPassword, false, false);

            return result.Succeeded ? Results.Ok("Login successful") : Results.Unauthorized();
        });

        // Secure Admin Endpoint
        app.MapGet("/admin", [Authorize(Policy = "AdminOnly")] () =>
            "Welcome, Admin! This is a restricted endpoint.");

        app.MapGet("/", () =>
        "Welcome to SafeVault API! Authentication and role-based access control are enabled.");

    }
}
