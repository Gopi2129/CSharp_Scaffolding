using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Utils
{
    public class BrowserClass
    {

        static IWebDriver? driver;
        private static readonly string browser = new AppConfiguration().BrowserName;
        static readonly bool isIncognitoMode = !string.IsNullOrEmpty(new AppConfiguration().IncognitoMode) &&
!new AppConfiguration().IncognitoMode.Equals("No", StringComparison.OrdinalIgnoreCase);
        static readonly bool isHeadlessMode = !string.IsNullOrEmpty(new AppConfiguration().HeadlessBrowser) &&
!new AppConfiguration().HeadlessBrowser.Equals("No", StringComparison.OrdinalIgnoreCase);

        public static IWebDriver GetBrowserInstanceCreated(bool isLocationEnable = false)
        {
            switch (browser.ToLower().Trim())
            {
                case "chrome":
                    return ChromeSettings(isLocationEnable);

                case "firefox":
                case "mozilla firefox":
                    return FireFoxSettings();

                case "ie":
                case "internet explorer":
                    return InternetExplorerSettings();

                default:
                    return ChromeSettings(isLocationEnable);
            }
        }

        public static IWebDriver ChromeSettings(bool isLocationEnable)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--start-maximized");
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--disable-geolocation");
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--disable-popup-blocking");
            chromeOptions.AddArgument("--disable-popup-blocking");

            if (isLocationEnable)
            {
                chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 2);
            }
            if (isIncognitoMode)
            {
                chromeOptions.AddArgument("--incognito");
            }
            if (isHeadlessMode)
            {
                chromeOptions.AddArgument("--headless");
            }

            chromeOptions.AddArgument("disable-infobars");
            driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), chromeOptions, TimeSpan.FromSeconds(180));
            return driver;
        }

        public static IWebDriver FireFoxSettings()
        {
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            FirefoxProfile firefoxProfile = new FirefoxProfile();
            firefoxOptions.Profile = firefoxProfile;
            driver = new FirefoxDriver(firefoxOptions);
            return driver;
        }

        public static IWebDriver InternetExplorerSettings()
        {
            driver = new InternetExplorerDriver();
            return driver;
        }

        public static IWebDriver EdgeSettings()
        {
            driver = new EdgeDriver();
            return driver;
        }
    }
}
