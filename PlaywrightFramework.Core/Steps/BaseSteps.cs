using Microsoft.Playwright;
using PlaywrightFramework.Core.Configuration;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace PlaywrightFramework.Core.Steps
{
    // Base class for all step classes
    public abstract class BaseSteps
    {
        protected readonly IPage Page;
        protected readonly TestConfig Config;

        protected BaseSteps(IPage page)
        {
            Page = page;
            Config = TestConfig.Instance;
        }

        // Step: Navigate to URL with retry logic
        protected async Task NavigateAsync(string url, int retryCount = 3, int retryDelayMs = 1000)
        {
            string fullUrl = url.StartsWith("http") ? url : $"{Config.BaseUrl.TrimEnd('/')}/{url.TrimStart('/')}";
            await Page.GotoAsync(fullUrl, new PageGotoOptions
                    {
                        Timeout = 30000, // 30 seconds
                        WaitUntil = WaitUntilState.NetworkIdle
                    });
        }

        // Step: Wait for page to load
        protected async Task WaitForPageLoadAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Step: Take screenshot
        protected async Task<byte[]> TakeScreenshotAsync(string path = null)
        {
            var screenshotOptions = new PageScreenshotOptions
            {
                FullPage = true,
                Path = path
            };

            var screenshot = await Page.ScreenshotAsync(screenshotOptions);
            return screenshot;
        }

        // Step: Check if element is visible
        protected async Task<bool> IsElementVisibleAsync(ILocator locator, int timeout = 5000)
        {
            try
            {
                await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = timeout });
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Step: Wait for element to be visible - internal helper method
        protected async Task WaitForElementVisibleAsync(ILocator locator, int timeout = 10000)
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });
        }

        // Step: Click on element
        protected async Task ClickElementAsync(ILocator locator, int timeout = 10000)
        {
            // Don't call WaitForElementVisibleAsync to avoid potential recursion
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });
            await locator.ClickAsync();
        }

        // Step: Type text into element
        protected async Task TypeTextAsync(ILocator locator, string text, int timeout = 10000)
        {
            // Don't call WaitForElementVisibleAsync to avoid potential recursion
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });
            await locator.FillAsync(text);
        }

        // Step: Get text from element
        protected async Task<string> GetTextFromElementAsync(ILocator locator, int timeout = 10000)
        {
            // Don't call WaitForElementVisibleAsync to avoid potential recursion
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });
            var text = await locator.TextContentAsync();
            return text;
        }
    }
}
