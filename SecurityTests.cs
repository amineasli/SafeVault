using NUnit.Framework;

[TestFixture]
public class SecurityTests
{
    [TestCase("SELECT * FROM users", "")]
    [TestCase("DROP TABLE users", "")]
    [TestCase("'; DELETE FROM users --", "")]
    public void Should_Remove_SQL_Injection_Attempts(string input, string expectedOutput)
    {
        var sanitizedInput = InputSanitizer.Sanitize(input);
        Assert.That(sanitizedInput, Is.EqualTo(expectedOutput));
    }

    [TestCase("<script>alert('XSS')</script>", "&lt;script&gt;alert('XSS')&lt;/script&gt;")]
    [TestCase("<img src=x onerror=alert('XSS')>", "&lt;img src=x onerror=alert('XSS')&gt;")]
    public void Should_Encode_HTML_To_Prevent_XSS(string input, string expectedOutput)
    {
        var sanitizedInput = InputSanitizer.Sanitize(input);
        Assert.That(sanitizedInput, Is.EqualTo(expectedOutput));
    }
}
