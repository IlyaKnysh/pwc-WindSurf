using Microsoft.Playwright;
using PlaywrightFramework.Core.Configuration;
using System;
using System.Threading.Tasks;

namespace PlaywrightFramework.Core.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;
        protected readonly TestConfig Config;

        protected BasePage(IPage page)
        {
            Page = page;
            Config = TestConfig.Instance;
        }

        public virtual async Task NavigateAsync(string url = null, int retryCount = 3, int retryDelayMs = 1000)
        {
            string targetUrl = url ?? Config.BaseUrl;
            Exception lastException = null;
            
            for (int attempt = 0; attempt < retryCount; attempt++)
            {
                try
                {
                    // Add a small random delay for parallel execution to avoid rate limiting
                    if (attempt > 0)
                    {
                        var random = new Random();
                        await Task.Delay(retryDelayMs + random.Next(1000));
                    }
                    
                    // Increase timeout for navigation
                    await Page.GotoAsync(targetUrl, new PageGotoOptions
                    {
                        Timeout = 30000, // 30 seconds
                        WaitUntil = WaitUntilState.NetworkIdle
                    });
                    
                    // If we get here, navigation was successful
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    Console.WriteLine($"Navigation attempt {attempt + 1} failed: {ex.Message}");
                }
            }
            
            // If we've exhausted all retries, throw the last exception
            throw new Exception($"Navigation failed after {retryCount} attempts", lastException);
        }

        public virtual async Task WaitForElementToBeVisibleAsync(ILocator locator, int timeout = 10000)
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });
        }

        public virtual async Task WaitForElementToBeEnabledAsync(ILocator locator, int timeout = 10000)
        {
            // First wait for the element to be visible
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });
            
            // Then poll until the element is enabled
            var startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < timeout)
            {
                if (await locator.IsEnabledAsync())
                    return;
                
                await Task.Delay(100);
            }
            
            throw new Exception($"Element did not become enabled within {timeout}ms");
        }

        public virtual async Task ClickAsync(ILocator locator, int timeout = 10000)
        {
            await WaitForElementToBeVisibleAsync(locator, timeout);
            await WaitForElementToBeEnabledAsync(locator, timeout);
            await locator.ClickAsync();
        }

        public virtual async Task TypeAsync(ILocator locator, string text, int timeout = 10000)
        {
            await WaitForElementToBeVisibleAsync(locator, timeout);
            await WaitForElementToBeEnabledAsync(locator, timeout);
            await locator.FillAsync(text);
        }

        public virtual async Task<string> GetTextAsync(ILocator locator, int timeout = 10000)
        {
            await WaitForElementToBeVisibleAsync(locator, timeout);
            return await locator.TextContentAsync();
        }

        public virtual async Task<bool> IsElementVisibleAsync(ILocator locator, int timeout = 5000)
        {
            try
            {
                await WaitForElementToBeVisibleAsync(locator, timeout);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<byte[]> TakeScreenshotAsync(string path = null)
        {
            var options = new PageScreenshotOptions
            {
                FullPage = true,
                Path = path
            };
            
            return await Page.ScreenshotAsync(options);
        }
    }
}
