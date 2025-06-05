using System;
using System.Collections.Specialized;
using NUnit.Framework;

namespace PlaywrightFramework.Core.Configuration
{
    public class TestConfig
    {
        private static readonly Lazy<TestConfig> _instance = new Lazy<TestConfig>(() => new TestConfig());
        
        public static TestConfig Instance => _instance.Value;

        private TestConfig()
        {
            // Set default values
            BaseUrl = "https://www.saucedemo.com";
            BrowserType = "chromium";
            Headless = true;
            SlowMo = 0;
            Timeout = 30000;
            ViewportWidth = 1280;
            ViewportHeight = 720;
            CaptureScreenshotOnFailure = true;
            CaptureTraceOnFailure = true;
            RetryCount = 1;
             
            // Override with values from runsettings if available
            LoadFromTestParameters();
            
            // Override with environment variables if available
            LoadFromEnvironmentVariables();
        }
        
        private void LoadFromTestParameters()
        {
            var testParameters = TestContext.Parameters;
            if (testParameters == null) return;
            
            // Browser configuration
            if (testParameters.Exists("BaseUrl")) BaseUrl = testParameters["BaseUrl"];
            if (testParameters.Exists("BrowserType")) BrowserType = testParameters["BrowserType"];
            if (testParameters.Exists("Headless")) Headless = bool.Parse(testParameters["Headless"]);
            if (testParameters.Exists("SlowMo")) SlowMo = int.Parse(testParameters["SlowMo"]);
            if (testParameters.Exists("Timeout")) Timeout = int.Parse(testParameters["Timeout"]);
            if (testParameters.Exists("ViewportWidth")) ViewportWidth = int.Parse(testParameters["ViewportWidth"]);
            if (testParameters.Exists("ViewportHeight")) ViewportHeight = int.Parse(testParameters["ViewportHeight"]);
            
            // Test execution configuration
            if (testParameters.Exists("CaptureScreenshotOnFailure")) 
                CaptureScreenshotOnFailure = bool.Parse(testParameters["CaptureScreenshotOnFailure"]);
            if (testParameters.Exists("CaptureTraceOnFailure")) 
                CaptureTraceOnFailure = bool.Parse(testParameters["CaptureTraceOnFailure"]);
            if (testParameters.Exists("RetryCount")) 
                RetryCount = int.Parse(testParameters["RetryCount"]);
        }
        
        private void LoadFromEnvironmentVariables()
        {
            // Browser configuration
            var baseUrl = System.Environment.GetEnvironmentVariable("BASE_URL");
            if (!string.IsNullOrEmpty(baseUrl)) BaseUrl = baseUrl;
            
            var browserType = System.Environment.GetEnvironmentVariable("BROWSER_TYPE");
            if (!string.IsNullOrEmpty(browserType)) BrowserType = browserType;
            
            var headless = System.Environment.GetEnvironmentVariable("HEADLESS");
            if (!string.IsNullOrEmpty(headless)) Headless = bool.Parse(headless);
            
            var retryCount = System.Environment.GetEnvironmentVariable("RETRY_COUNT");
            if (!string.IsNullOrEmpty(retryCount)) RetryCount = int.Parse(retryCount);
        }

        // Browser configuration
        public string BaseUrl { get; set; }
        public string BrowserType { get; set; }
        public bool Headless { get; set; }
        public int SlowMo { get; set; }
        public int Timeout { get; set; }
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }
        
        // Test execution configuration
        public bool CaptureScreenshotOnFailure { get; set; }
        public bool CaptureTraceOnFailure { get; set; }
        public int RetryCount { get; set; }
        
        // Environment-specific configuration
        public string Environment => System.Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "Local";
        
        // Authentication
        public string Username
        {
            get
            {
                // First try environment variable based on current environment
                var envUsername = Environment.ToLowerInvariant() == "production" 
                    ? System.Environment.GetEnvironmentVariable("PROD_USERNAME")
                    : System.Environment.GetEnvironmentVariable("TEST_USERNAME"); // Changed from USERNAME to TEST_USERNAME to avoid system username conflict
                    
                if (!string.IsNullOrEmpty(envUsername))
                    return envUsername;
                    
                // Fall back to default
                return "standard_user";
            }
        }
            
        public string Password
        {
            get
            {
                // First try environment variable based on current environment
                var envPassword = Environment.ToLowerInvariant() == "production" 
                    ? System.Environment.GetEnvironmentVariable("PROD_PASSWORD")
                    : System.Environment.GetEnvironmentVariable("TEST_PASSWORD"); // Changed from PASSWORD to TEST_PASSWORD to avoid system variable conflicts
                    
                if (!string.IsNullOrEmpty(envPassword))
                    return envPassword;
                    
                // Fall back to default
                return "secret_sauce!";
            }
        }
    }
}
