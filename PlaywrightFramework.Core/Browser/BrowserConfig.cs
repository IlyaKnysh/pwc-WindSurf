using Microsoft.Playwright;
using PlaywrightFramework.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlaywrightFramework.Core.Browser
{
    public class BrowserConfig
    {
        private readonly Dictionary<string, object> _options = new Dictionary<string, object>();
        
        // Default constructor uses TestConfiguration
        public BrowserConfig()
        {
            // Default values are taken from TestConfig
        }
        
        // Browser configuration properties with fallback to TestConfig
        public string BrowserType => GetOption<string>("BrowserType") ?? TestConfig.Instance.BrowserType;
        public bool Headless => GetOption<bool?>("Headless") ?? TestConfig.Instance.Headless;
        public int SlowMo => GetOption<int?>("SlowMo") ?? TestConfig.Instance.SlowMo;
        public int Timeout => GetOption<int?>("Timeout") ?? TestConfig.Instance.Timeout;
        public int ViewportWidth => GetOption<int?>("ViewportWidth") ?? TestConfig.Instance.ViewportWidth;
        public int ViewportHeight => GetOption<int?>("ViewportHeight") ?? TestConfig.Instance.ViewportHeight;
        public bool CaptureTraceOnFailure => GetOption<bool?>("CaptureTraceOnFailure") ?? TestConfig.Instance.CaptureTraceOnFailure;
        
        // Factory methods
        public static BrowserConfig FromTestConfiguration() => new BrowserConfig();
        
        public static BrowserConfig Custom(Action<BrowserConfig> configure)
        {
            var config = new BrowserConfig();
            configure(config);
            return config;
        }
        
        // Mobile configuration factory
        public static async Task<BrowserConfig> ForMobileAsync(string deviceName = "iPhone 13")
        {
            var playwright = await Playwright.CreateAsync();
            var device = playwright.Devices[deviceName];
            
            return Custom(config => {
                config.SetOption("UserAgent", device.UserAgent);
                config.SetOption("ViewportWidth", device.ViewportSize.Width);
                config.SetOption("ViewportHeight", device.ViewportSize.Height);
                config.SetOption("DeviceScaleFactor", device.DeviceScaleFactor);
                config.SetOption("HasTouch", device.HasTouch);
                config.SetOption("IsMobile", device.IsMobile);
            });
        }
        
        // Option management
        public void SetOption<T>(string key, T value)
        {
            _options[key] = value;
        }
        
        public T GetOption<T>(string key)
        {
            if (_options.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }
        
        // Browser launch options
        public BrowserTypeLaunchOptions GetLaunchOptions()
        {
            return new BrowserTypeLaunchOptions
            {
                Headless = Headless,
                SlowMo = SlowMo,
                Timeout = Timeout
            };
        }
        
        // Browser context options
        public BrowserNewContextOptions GetContextOptions()
        {
            var options = new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize
                {
                    Width = ViewportWidth,
                    Height = ViewportHeight
                },
                UserAgent = GetOption<string>("UserAgent"),
                DeviceScaleFactor = GetOption<float?>("DeviceScaleFactor"),
                HasTouch = GetOption<bool?>("HasTouch"),
                IsMobile = GetOption<bool?>("IsMobile")
            };
            
            return options;
        }
    }
}
