using NUnit.Framework;
using PlaywrightFramework.Core.Steps;
using System.Threading.Tasks;
using Allure.NUnit.Attributes;

namespace PlaywrightFramework.Tests.Tests
{
    [TestFixture]
    [Description("Login functionality tests")]
    [AllureSuite("Login")]
    [AllureEpic("Authentication")]
    [Parallelizable(ParallelScope.All)]
    public class LoginTests : BaseTest
    {
        private LoginSteps _loginSteps;

        [SetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            _loginSteps = new LoginSteps(Page);
        }

        [Test]
        [Description("Verify successful login with valid credentials")]
        [AllureFeature("Login")]
        [AllureStory("User can login with valid credentials")]
        [AllureSeverity(Allure.Net.Commons.SeverityLevel.critical)]
        public async Task SuccessfulLogin_WithValidCredentials()
        {
            // Arrange
            await _loginSteps.NavigateToLoginPageAsync();
            
            // Act
            await _loginSteps.LoginAsync(Config.Username, Config.Password);
            
            // Assert
            bool isLoggedIn = await _loginSteps.IsLoggedInAsync();
            Assert.IsTrue(isLoggedIn, "User should be logged in successfully");
        }

        [Test]
        [Description("Verify login fails with invalid credentials")]
        [AllureFeature("Login")]
        [AllureStory("User cannot login with invalid credentials")]
        [AllureSeverity(Allure.Net.Commons.SeverityLevel.critical)]
        public async Task FailedLogin_WithInvalidCredentials()
        {
            // Arrange
            await _loginSteps.NavigateToLoginPageAsync();

            // Act
            await _loginSteps.LoginAsync("invalid_user", "invalid_password");

            // Assert
            bool isLoggedIn = await _loginSteps.IsLoggedInAsync();
            Assert.IsFalse(isLoggedIn, "User should not be logged in with invalid credentials");

            string errorMessage = await _loginSteps.GetErrorMessageAsync();
            Assert.IsNotNull(errorMessage, "Error message should be displayed");
        }
    }
}
