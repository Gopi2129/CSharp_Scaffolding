using Automation.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Automation.Web.Tests.Support
{
    public class CommonActionClass
    {

        public static IWebDriver driver = null;

        public void LaunchApplication(string url, bool isLocationEnabled = false)
        {
            if (driver == null || isLocationEnabled == true)
            {
                driver = BrowserClass.GetBrowserInstanceCreated(isLocationEnabled);
                RemoveAllBrowserCookies();
                driver.Url = url;
            }
            else
            {
                driver.Url = url;
            }
        }
        public string getHtmlContentHasString(string htmlContent)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";
            const string stripFormatting = @"<[^>]*(>|$)";
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);
            var text = htmlContent;
            text = System.Net.WebUtility.HtmlDecode(text);
            text = tagWhiteSpaceRegex.Replace(text, "><");
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            text = stripFormattingRegex.Replace(text, string.Empty);
            text = text.Replace(@"\n", "").Replace("\r", "");
            return text;
        }

        public void SleepTime(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        public void DoubleClick(By element)
        {
            try
            {
                Actions actions = new Actions(driver);
                IWebElement elementLocator = driver.FindElement(element);
                actions.DoubleClick(elementLocator).Perform();
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not Double Clickable/Visible. " + e.Message);
            }
        }
        public static double SubRound(double d, double val)
        {
            return double.Parse(string.Format("{0:0.00}", d - val));
        }

        public void KeyEnterAction()
        {
            Actions actions = new Actions(driver);
            actions.SendKeys(Keys.Enter);
            actions.Build().Perform();

        }

        public void KeyUpAction(int count)
        {
            Actions actions = new Actions(driver);
            for (int i = 0; i < count; i++)
            {
                actions.SendKeys(Keys.ArrowUp);
                actions.Perform();
            }
        }

        public string RemoveAllSpace(string spacetoberemoved)
        {
            return Regex.Replace(spacetoberemoved, @"\s", "");
        }

        public string GetTitle()
        {
            return driver.Title;
        }

        public string ReplaceLast(string find, string replace, string str)
        {
            int lastIndex = str.LastIndexOf(find);

            if (lastIndex == -1)
            {
                return str;
            }

            string beginString = str.Substring(0, lastIndex);
            string endString = str[(lastIndex + find.Length)..];

            return beginString + replace + endString;
        }

        public void ScrollPage(int scrollsize)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("scroll(0, " + scrollsize + ")");
        }

        public void SetElementFocus(By Element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].focus();", driver.FindElement(Element));
        }

        public List<string> GetWindowHandles()
        {
            List<string> windows = new List<string>(driver.WindowHandles);

            return windows;
        }

        public void SwitchWindow(int i, List<string> windows)
        {
            driver.SwitchTo().Window(windows[i]);

        }

        public void ScrollToElement(By Element)
        {
            int locationValue = driver.FindElement(Element).Location.Y;
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scroll(0," + locationValue + ");");
        }

        public void ScrollIntoElementView(IWebElement Element)
        {
            try
            {
                //int locationValue = driver.FindElement(Element).Location.Y;
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", Element);
            }
            catch (NoSuchElementException e)
            {
                ReporterClass.AddFailedStepLog($"Element {Element} is not available. " + e.Message);
            }
        }


        public int GetCountOfAllOptionInDropdown(By Element)
        {
            try
            {
                SelectElement select = new SelectElement(driver.FindElement(Element));
                return select.Options.Count;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Dropdown {Element} is not displayed or not able to select option. " + e.Message);
            }
            return 0;
        }

        public List<string> GetAllOptionsNamesInDropDown(By Element)
        {
            List<string> options = new List<string>();
            try
            {
                SelectElement select = new SelectElement(driver.FindElement(Element));
                foreach (IWebElement optionElement in select.Options)
                {
                    options.Add(optionElement.Text);
                }
                return options;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Dropdown {Element} is not displayed or not able to select option. " + e.Message);
            }
            return null;
        }


        public By CreateWebElementWithDynamicXpath(string xpathValue, string substitutionValue)
        {
            string replacedXpath = xpathValue.Replace("PARAMETER", substitutionValue);
            try
            {
                By xpathVal = By.XPath(replacedXpath);
                return xpathVal;
            }
            catch (Exception)
            {
                ReporterClass.AddFailedStepLog("Unhandled exception found with the replaced dynamic xpath will return a null element");
                return null;
            }
        }

        public void VerifySelectedListItem(By Element, string expectedValue)
        {
            bool itemFound = false;
            string selectedOption = GetSelectedValueFromDropdown(Element);
            if (selectedOption.Equals(expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                itemFound = true;
            }
            if (itemFound == true)
            {
                ReporterClass.AddStepLog("Item is found in the drop down and is currently selected with value - " + expectedValue);
            }
            else
            {
                ReporterClass.AddFailedStepLog("Item not found in the drop down or is not currently selected with value - " + expectedValue);
            }
        }

        public string GetSelectedValueFromDropdown(IWebElement Element)
        {
            try
            {
                SelectElement select = new SelectElement(Element);
                return select.SelectedOption.Text;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Dropdown {Element} is not displayed or not able to select option. " + e.Message);
            }
            return null;
        }

        public bool VerifyTableRowCount(List<IWebElement> Element, int ExpectedRowCnt)
        {
            int ACount = Element.Count;
            if (ACount == ExpectedRowCnt)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ClickBasedOnVisibility(By Element)
        {
            bool isVisible = false;
            try
            {
                int height = driver.FindElement(Element).Location.Y;
                int width = driver.FindElement(Element).Location.X;
                isVisible = height > 0 && width > 0;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog("Exception in Optional Click element " + e);
            }

            if (isVisible)
            {
                IWebElement webElement = driver.FindElement(Element);
                if (webElement.TagName.Equals("input") || driver.FindElement(Element).TagName.Equals("button"))
                {
                    Actions builder = new Actions(driver);
                    builder.MoveToElement(webElement).Build().Perform();
                    builder.MoveToElement(webElement).Click().Perform();
                }
            }
            else
            {
                ReporterClass.AddFailedStepLog("Element not Exists on Screen, Skipping Click Operation");
            }
        }

        public void SelectValueByValue(By Element, string value)
        {
            try
            {
                SelectElement select = new SelectElement(driver.FindElement(Element));
                select.SelectByValue(value);
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Dropdown {Element} is not displayed or not able to select option. " + e.Message);
            }
        }

        public void SelectValueByVisibleText(By Element, string visibleText)
        {
            try
            {
                SelectElement select = new SelectElement(driver.FindElement(Element));
                select.SelectByText(visibleText);
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Dropdown {Element} is not displayed or not able to select option. " + e.Message);
            }
        }

        public void SelectValueByVisibleText(IWebElement Element, string visibleText)
        {
            try
            {
                SelectElement select = new SelectElement(Element);
                select.SelectByText(visibleText);
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Dropdown {Element} is not displayed or not able to select option. " + e.Message);
            }
        }

        public void SwitchToParentFrame()
        {
            try
            {
                driver.SwitchTo().ParentFrame();

            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog("No Parent Frame to switch! " + e.Message);

            }
        }
        public void SwitchToDefaultContent()
        {
            try
            {
                driver.SwitchTo().DefaultContent();
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($" Not in Frame to switch to Default content. " + e.Message);
            }
        }

        public bool IsElementEnabled(By element)
        {
            try
            {
                Thread.Sleep(3000);
                bool IsElementEnabled = driver.FindElement(element).Enabled;
                if (IsElementEnabled == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
            return false;
        }

        public void SwitchToFrameByElement(By Element)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(50)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.FrameToBeAvailableAndSwitchToIt(Element));
            }
            catch (Exception e)
            {
                ReporterClass.AddWarningStepMessage($"Element {Element} is not in Frame. " + e.Message, false);
            }
        }

        public void WaitForPageLoad()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            Thread.Sleep(2500);
            for (int i = 0; i < 60; i++)
            {
                Thread.Sleep(1000);
                // To check if page is in ready state.
                if (js.ExecuteScript("return document.readyState").ToString().Equals("complete", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }

        public void WaitForUrl(string strURL, int secondsToWait)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsToWait));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(strURL));
        }

        public void VerifyPageTitle(string expectedValue)
        {
            try
            {
                string Title = driver.Title;
                ReporterClass.AddStepLog($"Title of the page is - {Title}");
                if (Title.Equals(expectedValue.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    ReporterClass.AddStepLog("Actual Title - " + Title + " matches with expected value of - " + expectedValue);
                }
                else
                {
                    ReporterClass.AddFailedStepLog("Actual Title - " + Title + " does not match with expected value of - " + expectedValue);
                }
            }
            catch (NoSuchElementException)
            {
                ReporterClass.AddFailedStepLog("Unable to get Title");
            }
        }

        public string GetTextValue(IWebElement element)
        {
            try
            {
                return element.Text;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
            return "";
        }

        public string GetCssValueForElement(By element, string attribute)
        {
            try
            {
                return driver.FindElement(element).GetCssValue(attribute);
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
            return "";
        }

        public string GetCssValueForElement(IWebElement element, string attribute)
        {
            try
            {
                return element.GetCssValue(attribute);
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
            return "";
        }



        public static string TakeScreenshotImage(string filePath)
        {
            string screenshotPath = filePath;
            if (driver != null && !string.IsNullOrEmpty(screenshotPath))
            {
                ITakesScreenshot takeScreenShot = (ITakesScreenshot)driver;
                Screenshot screenshot = takeScreenShot.GetScreenshot();
                screenshot.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
                return screenshotPath;
            }
            else
            {
                ReporterClass.AddFailedStepLog("Please provide the screenshot Path");
                return null;
            }
        }

        /// <summary>
        /// Allows to take a web Element screenshot and save in specified path
        /// </summary>
        /// <param name="element"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string TakeElementScreenshot(By element, string filePath)
        {
            string screenshotPath = filePath;
            if (driver != null && !string.IsNullOrEmpty(screenshotPath))
            {
                IWebElement elementToTakeScreenshot = driver.FindElement(element);
                ITakesScreenshot takeScreenshot = (ITakesScreenshot)elementToTakeScreenshot;
                Screenshot screenshot = takeScreenshot.GetScreenshot();
                screenshot.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
                return screenshotPath;
            }
            else
            {
                ReporterClass.AddFailedStepLog("Please provide the screenshot Path");
                return null;
            }
        }
        public bool WaitForAttributeChange(By Element, string Attribute, string AttributeValue)
        {
            int i = 1;
            do
            {
                Thread.Sleep(2000);
                if (driver.FindElement(Element).GetAttribute(Attribute).Equals(AttributeValue, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else
                {
                    Thread.Sleep(2000);
                    i++;
                }
            } while (i <= 10);
            return false;
        }

        public bool WaitForAttributeChange(IWebElement Element, string Attribute, string AttributeValue)
        {
            int i = 1;
            do
            {
                Thread.Sleep(2000);
                string value = Element.GetAttribute(Attribute);
                if (value.Equals(AttributeValue, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else
                {
                    Thread.Sleep(2000);
                    i++;
                }
            } while (i <= 10);
            return false;
        }

        public bool WaitForDynamicAttribute(By Element, string Attribute, bool ToBeAppeared)
        {
            int i = 1;
            if (ToBeAppeared == true)
            {
                do
                {
                    if (driver.FindElement(Element).GetAttribute(Attribute) != null)
                    {
                        return true;
                    }
                    else
                    {
                        Thread.Sleep(2000);
                        i++;
                    }
                } while (i <= 5);
                return false;
            }
            else
            {
                do
                {
                    if (driver.FindElement(Element).GetAttribute(Attribute) == null)
                    {
                        return true;
                    }
                    else
                    {
                        Thread.Sleep(2000);
                        i++;
                    }
                } while (i <= 5);
                return false;
            }
        }

        public bool WaitForDynamicObjectToAppear(By Element)
        {
            int i = 1;
            Thread.Sleep(1500);
            do
            {
                if (driver.FindElements(Element).Count == 1)
                {
                    return true;
                }
                else
                {
                    Thread.Sleep(2000);
                    i++;
                }
            } while (i <= 5);
            return false;
        }



        public bool ValidateAttributeHasValue(By Element, string attribute)
        {
            int i = 1;
            Thread.Sleep(3000);
            do
            {
                if (driver.FindElement(Element).GetAttribute(attribute) != null)
                {
                    return true;
                }
                else
                {
                    Thread.Sleep(2000);
                    i++;
                }
            } while (i <= 5);
            return false;
        }

        public bool ElementNotDisplayed(By Element)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(15)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(Element));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void RemoveAllBrowserCookies()
        {
            try
            {
                driver.Manage().Cookies.DeleteAllCookies();
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog("Not able to delete cookies. " + e.Message);
            }
        }

        public int GetSizeOfElements(By element)
        {
            try
            {
                return driver.FindElements(element).Count;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
            return 0;
        }

        public bool WaitNotToBeStale(IWebElement element)
        {
            try
            {
                if (!new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.StalenessOf(element)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception" + e);
                return false;
            }
        }


        public IList<IWebElement> GetListOfWebElements(By element)
        {
            try
            {
                IList<IWebElement> webElements = new List<IWebElement>();
                webElements = driver.FindElements(element);
                return webElements;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
            return null;
        }

        public string GetAttributeValue(IWebElement element, string attribute)
        {
            string attributeValue = null;
            try
            {
                attributeValue = element.GetAttribute(attribute);
                return attributeValue;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
            return attributeValue;
        }

        public IWebElement WaitForElement(By element, int secondsToWait)
        {
            try
            {
                return new WebDriverWait(driver, TimeSpan.FromSeconds(secondsToWait)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(element));
            }
            catch (Exception e)
            {

                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
                return null;
            }
        }


        public void ElementExist(By element, int secondsToWait)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(secondsToWait)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(element));
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Waiting for Element failed or Element not present. Element - {element} <br> Exception-{e}");

            }
        }


        public void HighlightElement(By Element)
        {
            try
            {
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                for (int i = 0; i <= 10; i++)
                {
                    jsExecutor.ExecuteScript("arguments[0].style.border='5px solid yellow'", driver.FindElement(Element));
                }
                jsExecutor.ExecuteScript("arguments[0].style.border=''", driver.FindElement(Element));
            }
            catch (NoSuchElementException e)
            {
                ReporterClass.AddFailedStepLog($"Element {Element} is not available. " + e.Message);
            }
        }

        public bool WaitForInputValueToBeAvailable(By Element)
        {
            bool isAvailable = false;
            if (WaitForElement(Element, 35) != null)
            {
                for (int i = 0; i <= 5; i++)
                {
                    if (!string.IsNullOrEmpty(GetAttributeValue(Element, "value")))
                    {
                        isAvailable = true;
                        return isAvailable;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            else
            {
                ReporterClass.AddFailedStepLog($"Wait for Element {Element} Failed.");
            }
            return isAvailable;
        }

        public void ClickElement(By element)
        {
            try
            {
                driver.FindElement(element).Click();
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not Clickable/Visible. " + e.Message);
            }
        }

        public void ClickElement(IWebElement element)
        {
            try
            {
                element.Click();
            }
            catch (NoSuchElementException e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
        }

        public void ClickElementThroughJavaScript(By element)
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", driver.FindElement(element));
                Thread.Sleep(2000);
            }
            catch (NoSuchElementException e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
        }
        public void ClearValues(By element)
        {
            try
            {
                driver.FindElement(element).Clear();
            }
            catch (NoSuchElementException e)
            {
                ReporterClass.AddFailedStepLog("Element is not available. " + e.Message);
            }
        }

        public void SendValue(By element, string value)
        {
            try
            {
                driver.FindElement(element).SendKeys(value);
            }
            catch (NoSuchElementException e)
            {
                ReporterClass.AddFailedStepLog("Element is not available. " + e.Message);
            }
        }

        public void ClickElementThroughJavaScript(IWebElement element)
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
            catch (NoSuchElementException e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
        }

        public string GetAttributeValue(By element, string attribute)
        {
            string attributeValue = null;
            try
            {
                attributeValue = driver.FindElement(element).GetAttribute(attribute);
                return attributeValue;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
            return attributeValue;
        }


        public void ClickBrowserBack()
        {
            try
            {
                driver.Navigate().Back();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ReporterClass.AddFailedStepLog($"Exception in Back functionality.");
            }
        }

        //anu edited
        public bool ElementDisplayed(By Element)
        {
            WaitForDynamicObjectToAppear(Element);
            try
            {
                if (driver.FindElement(Element).Displayed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static bool WaitForVisible(IWebElement element, int timeSpan)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();
            while (watch.Elapsed.Seconds < timeSpan)
            {
                if (element.Displayed)
                    return true;
            }
            watch.Stop();
            throw new ElementNotVisibleException();
        }

        public bool WaitForDynamicObjectToAppear(IWebElement Element)
        {
            int i = 1;
            IList<IWebElement> elements = new List<IWebElement>();
            Thread.Sleep(3000);
            do
            {
                elements = (IList<IWebElement>)Element;
                if (elements.Count == 1)
                {
                    return true;
                }
                else
                {
                    Thread.Sleep(2000);
                    i++;
                }
            } while (i <= 5);
            return false;
        }
        public string GetTextValue(By element)
        {
            try
            {
                return driver.FindElement(element).Text;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Element {element} is not available. " + e.Message);
            }
            return "";
        }

        public static string ConvertRgbaToHex(string rgba)
        {
            if (!Regex.IsMatch(rgba, @"rgba\((\d{1,3},\s*){3}(0(\.\d+)?|1)\)"))
                throw new FormatException("rgba string was in a wrong format");

            var matches = Regex.Matches(rgba, @"\d+");
            StringBuilder hexaString = new StringBuilder("#");

            for (int i = 0; i < matches.Count - 1; i++)
            {
                int value = int.Parse(matches[i].Value);

                hexaString.Append(value.ToString("X"));
            }

            return hexaString.ToString();
        }


        public void AjaxWait(int timeoutInSeconds)
        {
            try
            {
                IJavaScriptExecutor jsDriver = (IJavaScriptExecutor)driver;
                for (int i = 0; i < timeoutInSeconds; i++)
                {
                    object numberOfAjaxConnections = jsDriver.ExecuteScript("return jQuery.active");
                    if (numberOfAjaxConnections is long)
                    {
                        long n = (long)numberOfAjaxConnections;
                        ReporterClass.AddStepLog("Number of active jquery AJAX controls: " + n);
                        if (n == 0L)
                            break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadInterruptedException e)
            {
                ReporterClass.AddFailedStepLog("Interrupted Exception " + e);
            }
        }

        public void SwitchToFrameByID(int FrameID)
        {
            try
            {
                driver.SwitchTo().Frame(FrameID);
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"No Frame to switch! " + e.Message);
            }
        }

        public void SwitchToFrameByName(string FrameName)
        {
            try
            {
                driver.SwitchTo().Frame(FrameName);
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"No Frame to switch! " + e.Message);
            }
        }
        public void SelectValueByIndex(By Element, int index)
        {
            try
            {
                SelectElement select = new SelectElement(driver.FindElement(Element));
                select.SelectByIndex(index);
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Dropdown {Element} is not displayed or not able to select option. " + e.Message);
            }
        }

        public void SelectValueByIndex(IWebElement Element, int index)
        {
            try
            {
                SelectElement select = new SelectElement(Element);
                select.SelectByIndex(index);
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Dropdown {Element} is not displayed or not able to select option. " + e.Message);
            }
        }


        public string GetSelectedValueFromDropdown(By Element)
        {
            try
            {
                SelectElement select = new SelectElement(driver.FindElement(Element));
                return select.SelectedOption.Text;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Dropdown {Element} is not displayed or not able to select option. " + e.Message);
            }
            return null;
        }

        public IList<IWebElement> GetAllOptionsElementsInDropDown(By Element)
        {
            try
            {
                SelectElement select = new SelectElement(driver.FindElement(Element));
                return select.Options;
            }
            catch (Exception e)
            {
                ReporterClass.AddFailedStepLog($"Dropdown {Element} is not displayed or not able to get select option. " + e.Message);
            }
            return null;
        }

        public void SwitchToAlertsAndAccept()
        {
            IAlert alert = driver.SwitchTo().Alert();
            alert.Accept();
        }

        public void SwitchToAlertAndDismiss()
        {
            IAlert alert = driver.SwitchTo().Alert();
            alert.Dismiss();
        }
        public void ScrollIntoElementView(By Element)
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", driver.FindElement(Element));
            }
            catch (NoSuchElementException e)
            {
                ReporterClass.AddFailedStepLog($"Element {Element} is not available. " + e.Message);
            }
        }

        /// <summary>
        /// Create new Tab or Window based on WindowType parameter
        /// </summary>
        /// <param name="window"></param>
        public void CreateNewWindowOrTab(WindowType window)
        {
            try
            {
                driver.SwitchTo().NewWindow(window);
            }
            catch (WebDriverException e)
            {
                ReporterClass.AddFailedStepLog($"Exception in driver." + e.Message);
            }
        }
        public bool WaitForExpectedObjectToAppearWithpageRefresh(By element, string expected)
        {
            int i = 1;
            Thread.Sleep(2000);
            do
            {
                string actual = driver.FindElement(element).Text;
                if (actual.Contains(expected, StringComparison.OrdinalIgnoreCase))
                {
                    driver.Navigate().Refresh();
                    Thread.Sleep(2000);
                    driver.Navigate().Refresh();
                    return true;
                }
                else
                {
                    Thread.Sleep(2000);
                    driver.Navigate().Refresh();
                    i++;
                }
            } while (i <= 15);
            return false;
        }
        public void MoveToElement(By element)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(driver.FindElement(element));
            actions.Perform();
        }

        public void MoveToElement(IWebElement element)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(element);
            actions.Perform();
        }

        public void KeyDownAction(int count)
        {
            Actions actions = new Actions(driver);
            for (int i = 0; i < count; i++)
            {
                actions.SendKeys(Keys.ArrowDown);
                actions.Perform();
            }
        }
        public int GenerateRandomNumber(int maxlimit)
        {
            Random r = new Random();
            int rInt = r.Next(0, maxlimit);

            return rInt;
        }

        public static double AddRound(double d, double val)
        {
            return double.Parse(string.Format("{0:0.00}", d + val));
        }
        public static double MulRound(double d, double val)
        {
            return double.Parse(string.Format("{0:0.00}", d * val));
        }

        public void CloseBrowser()
        {
            driver.Close();
        }

        public void RefreshPage()
        {
            driver.Navigate().Refresh();
        }

        public static void QuitDriverInstance()
        {
            if (driver != null)
            {
                driver.Close();
                driver.Quit();
                driver = null;
            }
            else
            {
                ReporterClass.AddFailedStepLog("No Driver instance available.");
            }
        }
        public string GetCurrentUrl()
        {
            return driver.Url;
        }
        public static bool FieldIsMandatory(IWebElement element)
        {
            return ((IJavaScriptExecutor)driver).ExecuteScript("return window.getComputedStyle(arguments[0], ':after'))." +
                   "getPropertyValue('content');", element).ToString().Contains("*");
        }

        public string GetPsuedoElementCSS(string psuedoelement, string psuedoelementtype, string css)
        {
            string script = $"return window.getComputedStyle(document.querySelector('{psuedoelement}'),'{psuedoelementtype}').getPropertyValue('{css}')";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            string content = (string)js.ExecuteScript(script);
            return content;
        }

        public bool IsElementSelected(By Element)
        {
            WaitForDynamicObjectToAppear(Element);
            if (driver.FindElement(Element).Selected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ElementDisabled(By Element)
        {
            WaitForDynamicObjectToAppear(Element);
            if (!driver.FindElement(Element).Enabled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetTextThroughJavaScript(By element)
        {
            string script = "return arguments[0].innerHTML";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            string text = (string)js.ExecuteScript(script, driver.FindElement(element));
            return text;
        }

        public void SendValue(IWebElement element, string value)
        {
            try
            {
                element.SendKeys(value);
            }
            catch (NoSuchElementException e)
            {
                ReporterClass.AddFailedStepLog("Element is not available. " + e.Message);
            }
        }

    }
}
