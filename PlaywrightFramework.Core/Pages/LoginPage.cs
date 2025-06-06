using Microsoft.Playwright;

namespace PlaywrightFramework.Core.Pages
{
    // Elements layer - contains only locators and selectors (no methods)
    public class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page)
        {
            _page = page;
        }

        // Login form elements
        public ILocator UsernameInput => _page.Locator("#user-name");
        public ILocator PasswordInput => _page.Locator("#password");
        public ILocator LoginButton => _page.Locator("#login-button");
        
        // Error message elements
        public ILocator ErrorMessage => _page.Locator("[data-test='error']");
        
        // Header elements after successful login
        public ILocator AppLogo => _page.Locator(".app_logo");
        public ILocator BurgerMenu => _page.Locator("#react-burger-menu-btn");
    }
}
