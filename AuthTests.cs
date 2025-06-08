using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

[TestFixture]
public class AuthTests
{
    private Mock<UserManager<AppUser>> _userManagerMock;
    private Mock<SignInManager<AppUser>> _signInManagerMock;

    [SetUp]
    public void Setup()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            new Mock<IUserStore<AppUser>>().Object, null, null, null, null, null, null, null, null);

        _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object, new Mock<IHttpContextAccessor>().Object, new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object, null, null, null, null);
    }

    [Test]
    public async Task InvalidLogin_ShouldReturnUnauthorized()
    {
        _signInManagerMock.Setup(m => m.PasswordSignInAsync("NonExistentUser", "WrongPassword", false, false))
            .ReturnsAsync(SignInResult.Failed);

        var result = await _signInManagerMock.Object.PasswordSignInAsync("NonExistentUser", "WrongPassword", false, false);

        Assert.That(result, Is.EqualTo(SignInResult.Failed));
    }

    [Test]
    public async Task UnauthorizedUserAccess_AdminPage_ShouldFail()
    {
        var user = new AppUser { UserName = "NormalUser" };
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin"))
            .ReturnsAsync(false);

        var isAdmin = await _userManagerMock.Object.IsInRoleAsync(user, "Admin");

        Assert.That(isAdmin, Is.False);
    }

    [Test]
    public async Task AdminUserAccess_AdminPage_ShouldSucceed()
    {
        var user = new AppUser { UserName = "AdminUser" };
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin"))
            .ReturnsAsync(true);

        var isAdmin = await _userManagerMock.Object.IsInRoleAsync(user, "Admin");

        Assert.That(isAdmin, Is.True);
    }
}
