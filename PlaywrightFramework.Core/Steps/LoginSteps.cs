using Microsoft.Playwright;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using System.Threading.Tasks;
using PlaywrightFramework.Core.Pages;

namespace PlaywrightFramework.Core.Steps
{
    public class LoginSteps : BaseSteps
    {
        private readonly LoginPage _elements;

        public LoginSteps(IPage page) : base(page)
        {
            _elements = new LoginPage(page);
        }

        [AllureStep("Navigate to login page")]
        public async Task NavigateToLoginPageAsync()
        {
            await NavigateAsync("/");
        }

        [AllureStep("Login with username {username}")]
        public async Task LoginAsync(string username, string password)
        {
            // Log parameters for debugging
            TestContext.WriteLine($"Username: {username}");
            TestContext.WriteLine($"Password: ***");
            
            await TypeTextAsync(_elements.UsernameInput, username);
            await TypeTextAsync(_elements.PasswordInput, password);
            await ClickElementAsync(_elements.LoginButton);
            await WaitForPageLoadAsync();
        }

        [AllureStep("Check if user is logged in")]
        public async Task<bool> IsLoggedInAsync()
        {
            var isLoggedIn = await IsElementVisibleAsync(_elements.AppLogo);
            return isLoggedIn;
        }

        [AllureStep("Get error message")]
        public async Task<string> GetErrorMessageAsync()
        {
            if (await IsElementVisibleAsync(_elements.ErrorMessage))
            {
                var errorMessage = await GetTextFromElementAsync(_elements.ErrorMessage);
                return errorMessage;
            }
            return null;
        }
    }
}
