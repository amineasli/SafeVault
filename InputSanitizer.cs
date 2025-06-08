using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Web;

public static class InputSanitizer
{
    private static ILogger _logger;

    public static void ConfigureLogger(ILogger logger)
    {
        _logger = logger;
    }

    public static string Sanitize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // Check for SQL injection patterns
        if (Regex.IsMatch(input, @"\b(SELECT|INSERT|DELETE|UPDATE|DROP|--)\b", RegexOptions.IgnoreCase))
        {
            _logger?.LogWarning("Possible SQL injection attempt detected: {Input}", input);
        }

        // Check for XSS patterns
        if (Regex.IsMatch(input, @"<script>|onerror|alert\(", RegexOptions.IgnoreCase))
        {
            _logger?.LogWarning("Possible XSS attack detected: {Input}", input);
        }

        // Remove harmful characters
        input = Regex.Replace(input, @"[<>""'/;]", "", RegexOptions.Compiled);
        
        return HttpUtility.HtmlEncode(input);
    }
}
