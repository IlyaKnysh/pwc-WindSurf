using Microsoft.Playwright;
using NUnit.Framework.Interfaces;
using PlaywrightFramework.Core.Assertions;
using PlaywrightFramework.Core.Browser;
using PlaywrightFramework.Core.Configuration;
using Allure.NUnit;
using Allure.Net.Commons;

namespace PlaywrightFramework.Tests.Tests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)] // Ensure a new instance for each test
    [AllureNUnit]
    public abstract class BaseTest
    {
        protected IPlaywright Playwright;
        protected IBrowser Browser;
        protected IBrowserContext Context;
        protected IPage Page;
        protected CustomAssert Assert;
        protected TestConfig Config;

        [SetUp]
        public virtual async Task SetUp()
        {
            LogTestStart();
            InitializeTestDependencies();
            await InitializePlaywright();
        }

        [TearDown]
        public virtual async Task TearDown()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            
            LogTestCleanupStart(testName, threadId);
            
            if (testStatus == TestStatus.Failed)
            {
                await CaptureFailureEvidence(testName);
            }
            
            await CleanupResources(testName);
            
            LogTestCompletion(testName, threadId);
        }
        
        // Can be overridden in derived test classes to provide custom browser configuration
        protected virtual BrowserConfig GetBrowserConfig()
        {
            return new BrowserConfig();
        }
        
        #region Setup Helper Methods
        
        private void LogTestStart()
        {
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Starting test '{TestContext.CurrentContext.Test.Name}' on thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }
        
        private void InitializeTestDependencies()
        {
            Config = TestConfig.Instance;
            Assert = new CustomAssert();
        }
        
        private async Task InitializePlaywright()
        {
            try
            {
                await CreatePlaywrightInstance();
                await LaunchBrowser();
                await CreateBrowserContext();
                await CreatePage();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error during browser setup: {ex.Message}");
                throw;
            }
        }
        
        private async Task CreatePlaywrightInstance()
        {
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Created Playwright instance for test '{TestContext.CurrentContext.Test.Name}'");
        }
        
        private async Task LaunchBrowser()
        {
            Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = Config.Headless,
                Timeout = 30000 // Increase timeout for browser launch
            });
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Launched browser for test '{TestContext.CurrentContext.Test.Name}'");
        }
        
        private async Task CreateBrowserContext()
        {
            var browserConfig = GetBrowserConfig();
            
            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize
                {
                    Width = browserConfig.ViewportWidth,
                    Height = browserConfig.ViewportHeight
                }
            });
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Created browser context for test '{TestContext.CurrentContext.Test.Name}'");

            // Start tracing if enabled
            if (Config.CaptureTraceOnFailure)
            {
                await Context.Tracing.StartAsync(new TracingStartOptions
                {
                    Screenshots = true,
                    Snapshots = true
                });
            }
        }
        
        private async Task CreatePage()
        {
            Page = await Context.NewPageAsync();
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Created page for test '{TestContext.CurrentContext.Test.Name}'");
        }
        
        #endregion
        
        #region Teardown Helper Methods
        
        private void LogTestCleanupStart(string testName, int threadId)
        {
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Starting cleanup for test '{testName}' on thread {threadId}");
        }
        
        private async Task CaptureFailureEvidence(string testName)
        {
            try
            {
                await CaptureScreenshot(testName);
                await CaptureTrace(testName);
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error during failure capture: {ex.Message}");
            }
        }
        
        private async Task CaptureScreenshot(string testName)
        {
            if (Page == null) return;
            
            var screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "screenshots", $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath));
            
            var screenshotBytes = await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.AddTestAttachment(screenshotPath, "Screenshot on failure");
            
            // Add screenshot to Allure report
            if (screenshotBytes != null)
            {
                AllureApi.AddAttachment("Screenshot on failure", "image/png", screenshotBytes, ".png");
            }
            
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Captured screenshot for failed test '{testName}'");
        }
        
        private async Task CaptureTrace(string testName)
        {
            if (!Config.CaptureTraceOnFailure || Context == null) return;
            
            var tracePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "traces", $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.zip");
            
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(tracePath));
            
            await Context.Tracing.StopAsync(new TracingStopOptions
            {
                Path = tracePath
            });
            
            TestContext.AddTestAttachment(tracePath, "Trace file");
            
            // Add error details to test context
            CaptureErrorDetails(testName);
            
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Captured trace for failed test '{testName}'");
        }
        
        private void CaptureErrorDetails(string testName)
        {
            var errorMessage = TestContext.CurrentContext.Result.Message;
            var stackTrace = TestContext.CurrentContext.Result.StackTrace;
            
            if (string.IsNullOrEmpty(errorMessage)) return;
            
            string errorFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "errors", $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}_error.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(errorFilePath));
            File.WriteAllText(errorFilePath, $"Message: {errorMessage}\n\nStack Trace: {stackTrace}");
            TestContext.AddTestAttachment(errorFilePath, "Error Details");
        }
        
        private async Task CleanupResources(string testName)
        {
            try
            {
                await ClosePage(testName);
                await CloseContext(testName);
                await CloseBrowser(testName);
                DisposePlaywright(testName);
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error during cleanup: {ex.Message}");
            }
        }
        
        private async Task ClosePage(string testName)
        {
            if (Page == null) return;
            
            await Page.CloseAsync();
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Closed page for test '{testName}'");
        }
        
        private async Task CloseContext(string testName)
        {
            if (Context == null) return;
            
            await Context.CloseAsync();
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Closed context for test '{testName}'");
        }
        
        private async Task CloseBrowser(string testName)
        {
            if (Browser == null) return;
            
            await Browser.CloseAsync();
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Closed browser for test '{testName}'");
        }
        
        private void DisposePlaywright(string testName)
        {
            if (Playwright == null) return;
            
            Playwright.Dispose();
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Disposed Playwright for test '{testName}'");
        }
        
        private void LogTestCompletion(string testName, int threadId)
        {
            TestContext.Out.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Completed test '{testName}' on thread {threadId}");
        }
        
        #endregion
    }
}
