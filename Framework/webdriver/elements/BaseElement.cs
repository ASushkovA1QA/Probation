using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    /// <summary>
    /// Abstract class for any elements in the application
    /// All classes that described elements such as Button, TextBox and etc.
    /// should inherit of this class
    /// </summary>
    public abstract class BaseElement : BaseEntity
    {
        /// <summary>
        /// Name of elements (usually used for logging)
        /// </summary>
        protected readonly string Name;
        /// <summary>
        /// locator to element
        /// </summary>
        protected By Locator;
        /// <summary>
        /// IWebElement instance
        /// </summary>
        protected IWebElement Element;

        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="locator">locator By of the element</param>
        /// <param name="name">name of the element</param>
        protected BaseElement(By locator, string name)
        {
            Locator = locator;
            Name = name == "" ? GetText() : name;
        }

        /// <summary>
        /// return IWeb-Element
        /// </summary>
        /// <returns>IWeb-Element</returns>
        public IWebElement GetElement()
        {
            try
            {
                Element = Browser.GetDriver().FindElement(Locator);
            }
            catch (NoSuchElementException)
            {
                KAssert.Fail("Element not found");
            }
            return Element;
        }
        /// <summary>
        /// returns elements, currently displayed on page
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static List<IWebElement> GetDisplayedElements(By locator)
        {
            var resultList = new List<IWebElement>();
            try
            {
                var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(Browser.TimeoutForElementWaiting));
                IWebElement element;
                element = wait.Until(d =>
                {
                    element = d.FindElement(locator);
                    return element.Displayed ? element : null;
                });
                resultList.AddRange(Browser.GetDriver().FindElements(locator).Where(e=>e.Displayed));
            }
            catch (TimeoutException ex)
            {
                Browser.GetDriver().Navigate().Refresh();
                if (resultList.Count == 0)
                {
                    Logger.Instance.Fail(ex.Message + "\n" + ("Elements not found; timeout extended"));
                }
            }
            catch (NoSuchElementException)
            {
                KAssert.Fail("Elements not found");
            }
            return resultList;
        }

        /// <summary>
        /// get locator By
        /// </summary>
        /// <returns>locator By of the element</returns>
        public By GetLocator()
        {
            return Locator;
        }

        /// <summary>
        /// method gets the name of element
        /// </summary>
        /// <returns>name of the element</returns>
        public string GetName()
        {
           return Name;
        }

        /// <summary>
        /// formats the value for logging "element type - name - log splitter - the message"
        /// </summary>
        /// <param name="message">message for format</param>
        /// <returns>a formatted value for logging "element type - name - log splitter - the message"</returns>
        protected override string FormatLogMsg(string message)
        {
            return string.Format("{0} '{1}' {2} {3}", GetElementType(), Name, Logger.LogDelimiter, message);
        }


        /// <summary>
        /// assertion of the presence of the element on the page
        /// </summary>
        public void AssertIsPresent()
        {
            if (!IsPresent())
            {
                Log.Fail(FormatLogMsg("is not present"));
            }
            else
            {
                Log.Info(FormatLogMsg("is present"));
            }
        }

        /// <summary>
        ///  wait until element is absent
        /// </summary>
        /// <param name="seconds">count of seconds to wait for the absence of an element on the page</param>
        /// <returns>true if absent</returns>
        public bool IsAbsent(int seconds)
        {
            var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(Convert.ToDouble(seconds)));
            try
            {
                wait.Until(waiting =>
                {
                    var elements = Browser.GetDriver().FindElements(Locator);
                    Browser.Refresh();
                    Browser.WaitForPageToLoad();
                    if (elements.Count == 0)
                    {
                        return true;
                    }
                    return false;
                });
            }
            catch (WebDriverTimeoutException e)
            {
                Logger.Instance.Debug("Element is present: " + GetLocator() + "\r\n" + e.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        ///  assertion of the absence of the element on the page
        /// </summary>
        public void AssertIsAbsent()
        {
            if (IsPresent())
            {
                Log.Fail(FormatLogMsg("is present"));
            }
            else
            {
                Log.Info(FormatLogMsg("is absent"));
            }
        }

        /// <summary>
        ///  assertion of the disable element on the page
        /// </summary>
        public void AssertIsDisable()
        {
            if (!IsDisabled())
            {
                Log.Fail(FormatLogMsg("is enabled"));
            }
            else
            {
                Log.Info(FormatLogMsg("is disabled"));
            }
        }


        /// <summary>
        /// check that the element is enabled (performed by a class member)
        /// </summary>
        /// <returns>true if element is enabled</returns>
        public bool IsEnabled()
        {
            WaitForElementIsPresent();
            String elementClass = Element.GetAttribute("class");
            return Element.Enabled && (!elementClass.ToLower().Contains("disabled"));
        }


        /// <summary>
        /// workaround for AJAX 
        /// </summary>
        /// <returns>element if element is enabled else null</returns>
        public void WaitForIsEnabled()
        {
            try
            {
                var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(Browser.TimeoutForElementWaiting));

                Element = wait.Until(d =>
                {
                    try
                    {
                        Element = d.FindElement(Locator);
                        return Element.Enabled ? Element : null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                    catch (InvalidElementStateException)
                    {
                        return null;
                    }
                    catch (StaleElementReferenceException e)
                    {
                        Logger.Instance.Debug(e.Message + "\n" +  FormatLogMsg(" bad situation"));
                        return null;
                    }
                });
            }
            catch (TimeoutException ex)
            {
                Logger.Instance.Fail(ex.Message + "\n" + FormatLogMsg(" is not enabled"));
            }
            catch (Exception exec)
            {
                Logger.Instance.Fail(exec.Message + "\n" + FormatLogMsg(" bad situation"));
            }
        }

        /// <summary>
        /// workaround for AJAX
        /// </summary>
        /// <returns>element if element is enabled else null</returns>
        public void WaitForIsDisabled()
        {
            try
            {
                var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(Browser.TimeoutForElementWaiting));
                Element = wait.Until(d =>
                {
                    Element = d.FindElement(Locator);
                    return Element.Enabled ? null : Element;
                });
            }
            catch (TimeoutException ex)
            {
                Logger.Instance.Fail(ex.Message + "\n" + FormatLogMsg(" is not Disabled"));
            }
        }

        /// <summary>
        /// wait until element is presence
        /// </summary>
        /// <returns>element if element is displayed else null</returns>
        public void WaitForElementIsPresent()
        {
            try
            {
                var wait = new WebDriverWait(Browser.GetDriver(), Browser.GetElementTimeoutInSeconds());
                Element = wait.Until(d =>
                {
                    try
                    {
                        try
                        {
                            var elements = d.FindElements(Locator);
                            return elements.FirstOrDefault(webElement => webElement.Displayed);
                        }
                        catch (NoSuchElementException)
                        {
                            return null;
                        }
                    }
                    catch (StaleElementReferenceException e)
                    {
                        Logger.Instance.Debug(e.StackTrace);
                        return null;
                    }
                });
            }
            catch (TimeoutException ex)
            {
                Browser.GetDriver().Navigate().Refresh();
                if (!IsPresent())
                {
                    Logger.Instance.Fail(ex.Message + "\n" + FormatLogMsg(" is not Present" + " by locator: " + Locator));
                }
            }
            catch (WebDriverTimeoutException ex)
            {
                if (!IsPresent())
                {
                    Logger.Instance.Fail(ex.Message + "\n" + FormatLogMsg(" is not Present" + " by locator: " + Locator));
                }
            }
        }


        /// <summary>
        /// wait until element exists
        /// initialize Element var if element exist
        /// </summary>
        public void WaitForElementExists()
        {
            try
            {
                var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(Browser.TimeoutForElementWaiting));
                Element = wait.Until(d =>
                {
                    try
                    {
                        var elements = d.FindElements(Locator);
                        var count = elements.Count;
                        if (count > 0)
                        {
                            Logger.Instance.Debug(count + " elements was found by locator " + GetLocator());
                            Element = elements[0];
                            return Element;
                        }
                        return null;
                    }
                    catch (StaleElementReferenceException e)
                    {
                        Logger.Instance.Debug(e.StackTrace);
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException ex)
            {
                if (!IsPresent())
                {
                    Logger.Instance.Fail(ex.Message + "\n" + FormatLogMsg(" is not Exists" + " by locator: " + Locator));
                }
            }
        }

        /// <summary>
        ///  wait until element is presence updating each time the page
        /// </summary>
        /// <param name="url">url for update</param>
        /// <returns>element if element is displayed else null</returns>
        public void WaitForIsElementPresentReloadingPage(string url)
        {
            try
            {
                //var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(Browser.TimeoutForElementWaiting));
                var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(120));
                Element = wait.Until(d =>
                {
                    Browser.NavigateTo(url);
                    Element = d.FindElement(Locator);
                    return Element.Displayed ? Element : null;
                });
            }
            catch (TimeoutException ex)
            {
                Browser.GetDriver().Navigate().Refresh();
                if (!IsPresent())
                {
                    Logger.Instance.Fail(ex.Message + "\n" + FormatLogMsg(" is not Present"));
                }
            }
        }

        /// <summary>
        /// wait until element is absent
        /// </summary>
        public void WaitForIsAbsent()
        {
            try
            {
                if (IsPresent(3))
                {
                    var wait = new WebDriverWait(Browser.GetDriver(),
                        TimeSpan.FromSeconds(Browser.TimeoutForElementWaiting));
                    wait.Until(d => (Browser.GetDriver().FindElements(Locator).Count <= 0));
                }
            }
            catch (StaleElementReferenceException)
            {
                Log.Info(FormatLogMsg(" is already absent"));
            }
            catch (WebDriverException)
            {
                Log.Info(FormatLogMsg(" is already absent"));
            }
            catch (InvalidOperationException)
            {
                Log.Info(FormatLogMsg(" is already absent"));
            }
        }


        /// <summary>
        /// checks the presence of an element on the page
        /// </summary>
        /// <returns>true if element is displayed</returns>
        public bool IsPresent()
        {
            return IsPresent(5);
        }

        /// <summary>
        /// check if element exists on the page
        /// </summary>
        /// <returns>true if exists</returns>
        public bool IsExists()
        {
            return IsExists(5);
        }

        /// <summary>
        /// check for is element present on the page
        /// </summary>
        /// <param name="sec">count of seconds to wait for the absence of an element on the page</param>
        /// <returns>true if element is present</returns>
        public bool IsPresent(int sec)
        {
            var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(Convert.ToDouble(sec)));
            try
            {
                wait.Until(waiting =>
                {
                    var elements = Browser.GetDriver().FindElements(Locator);
                    try
                    {
                        foreach (var webElement in elements.Where(webElement => webElement.Displayed))
                        {
                            Element = webElement;
                            return true;
                        }
                    }
                    catch (StaleElementReferenceException)
                    {
                            return false;
                    }
                    return false;
                });
            }
            catch (WebDriverTimeoutException e)
            {
                Logger.Instance.Debug("Element is not present: " + GetLocator() + "\r\n" + e.StackTrace);
                return false;
                
            }
            return true;
        }

        /// <summary>
        /// check for is element exists on the page
        /// </summary>
        /// <param name="sec">wait in seconds until element is not exists</param>
        /// <returns>true if exists</returns>
        public bool IsExists(int sec)
        {
            var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(Convert.ToDouble(sec)));
            try
            {
                wait.Until(waiting =>
                {
                    var elements = Browser.GetDriver().FindElements(Locator);
                    if (elements.Count > 0)
                    {
                        Element = elements[0];
                        return true;
                    }
                    return false;
                });
            }
            catch (WebDriverTimeoutException e)
            {
                Logger.Instance.Debug("Element is not exists: " + GetLocator() + "\r\n" + e.StackTrace);
                return false;

            }
            return true;
        }


        /// <summary>
        /// check for is element disabled on the page
        /// </summary>
        /// <returns></returns>
        public bool IsDisabled()
        {
            try
            {
                Element = Browser.GetDriver().FindElement(Locator);
            }
            catch (Exception)
            {
                Log.Info(FormatLogMsg(" is not present"));
                return false;
            }
            return !Element.Enabled;
        }

        /// <summary>
        /// click on the element
        /// </summary>
        public void Click()
        {
            WaitForElementIsPresent();
            Info("Clicking");
            try
            {
                if (Browser.CurrentBrowser == BrowserFactory.BrowserType.Iexplore)
                {
                    new Actions(Browser.GetDriver()).MoveToElement(Element).Click(Element).Perform();
                }
                else
                {
                    Element.Click();
                }
            }
            catch (InvalidOperationException ex)
            {
                Warn(ex.Message);
                Fatal(" is not available for click ");
            }
            
        }

        /// <summary>
        /// Clicks via Js on element, waits for page to load, then switches to the NEXT tab
        /// </summary>
        public void ClickAndWaitThenSwitchTab()
        {
            ClickJs();
            Browser.WaitForPageToLoad();
            Thread.Sleep(2000);
            var currentHandler = Browser.GetDriver().CurrentWindowHandle;
            var nextWindowIndex = Browser.GetDriver().WindowHandles.IndexOf(currentHandler) + 1;
            Browser.GetDriver().SwitchTo().Window(Browser.GetDriver().WindowHandles[nextWindowIndex]);
        }

        /// <summary>
        ///click on an item and wait for the page is loaded
        /// </summary>
        public void ClickAndWaitForLoading()
        {
            try
            {
                Info("Perform click and wait for page to load");
                Click();
                Browser.WaitForPageToLoad();
            }
            catch (WebDriverException exc)
            {
                Info("An exception accured while we were trying to click by " + Name + "One attemp yet...\r\n" + exc.Message);
                WaitForElementIsPresent();
                Info("Perform click and wait for page to load");
                Click();
                Browser.WaitForPageToLoad();
            }
        }

        public void ClickJs()
        {
            WaitForElementIsPresent();
            Logger.Instance.Info(GetName() + " :: Click with JavaScript");
            try
            {
                Browser.GetDriver().ExecuteScript("arguments[0].click();", Element);
            }
            catch (Exception e)
            {
                Logger.Instance.Warn("Возникла проблема при выполнении нажатия JS: " + e.Message);
            }
        }

        /// <summary>
        /// extended click through Enter
        /// </summary>
        public void ClickExt()
        {
            WaitForElementIsPresent();
            Info("extended Clicking");
            Browser.GetDriver().FindElement(Locator).SendKeys(Keys.Enter);
        }

        /// <summary>
        /// fails test if text not equal current
        /// </summary>
        /// <param name="expected"></param>
        public void AssertEqualsText(string expected)
        {
            new Label(By.XPath(String.Format("//*[contains(.,'{0}')]", expected)), expected).WaitForElementIsPresent();
            Log.Info("Text [" + expected + "] is shown on the page!");
        }

        /// <summary>
        /// fails test if text equal current
        /// </summary>
        /// <param name="expected"></param>
        public void AssertNotEqualsText(string expected)
        {
            KAssert.AssertNotEqualsPosMsg(string.Format("Text of [{0}] not equals [{1}]", Name, expected), expected, GetText());
        }

        /// <summary>
        /// returns count of elements using findElements method of selenium
        /// </summary>
        /// <param name="locator">locator to element</param>
        /// <returns></returns>
        public static int GetElementsCount(By locator)
        {
            Browser.WaitForPageToLoad();
            return Browser.GetDriver().FindElements(locator).Count;
        }

        /// <summary>
        /// click on an item ext click through Enter and wait for the page is loaded.
        /// </summary>
        public void ClickExtAndWait()
        {
            try
            {
                ClickExt();
                Browser.WaitForPageToLoad();
            }
            catch (WebDriverException exc)
            {
                Warn(exc.Message);
                Browser.GetDriver().Navigate().Refresh();
                Browser.GetDriver().FindElement(By.XPath("//")).SendKeys(Keys.Enter);
                //Browser.Sleep(5000);

                ClickExt();
                Browser.WaitForPageToLoad();
            }
        }
        
        /// <summary>
        /// click via Action.
        /// </summary>
        public void ClickViaAction()
        {
            WaitForElementIsPresent();
            AssertIsPresent();
            Info("Clicking");
            var action = new Actions(Browser.GetDriver());
            action.Click(GetElement());
            action.Perform();
        }

        /// <summary>
        /// click via JS.
        /// </summary>
        public void ClickViaJs()
        {
            WaitForElementIsPresent();
            Info("Clicking");
            ((IJavaScriptExecutor)Browser.GetDriver()).ExecuteScript("arguments[0].click();", GetElement());
        }

        /// <summary>
        /// click via JS.
        /// </summary>
        public void ClickInvisible()
        {
            WaitForElementExists();
            Info("Clicking");
            ((IJavaScriptExecutor)Browser.GetDriver()).ExecuteScript("arguments[0].click();", GetElement());
        }

        /// <summary>
        /// click on an item js click and wait for the page is loaded.
        /// </summary>
        public void ClickViaJsAndWait()
        {
            try
            {
            ClickViaJs();
            Browser.WaitForPageToLoad();
            }
            catch (WebDriverException exc)
            {
                Logger.Instance.Debug(exc.Message);
                Browser.GetDriver().Navigate().Refresh();
                Browser.GetDriver().FindElement(By.XPath("//")).SendKeys(Keys.Enter);
                ClickViaJs();
                Browser.WaitForPageToLoad();
            }
        }

        /// <summary>
        /// move the cursor to the element and click him
        /// </summary>
        public void ClickWithMouseOver()
        {
            WaitForElementIsPresent();
            Info("Clicking with mouse over");
            new Actions(Browser.GetDriver()).MoveToElement(Element).Click(Element).Perform();
        }

        /// <summary>
        /// click and look forward to the emergence of a new window
        /// </summary>
        public void ClickAndWaitForNewWindow()
        {
            int count = Browser.WindowCount();
            Click();
            Info("Select next window");
            Browser.WaitForNewWindow(count);
            Browser.SwitchWindow(count);
            Browser.WindowMaximise();
        }

        /// <summary>
        /// click and look forward to closing the current window
        /// </summary>
        public void ClickAndWaitForWindowClose()
        {
            int count = Browser.WindowCount();
            Click();
            Info("Select previous window");
            Browser.WaitForNewWindow(count - 2);
            Browser.SwitchWindow(count - 2);
        }

        /// <summary>
        /// double click
        /// </summary>
        public void DoubleClick()
        {
            WaitForElementIsPresent();
            Info("Double clicking");
            new Actions(Browser.GetDriver()).DoubleClick(Element).Perform();
        }

        /// <summary>
        /// send keys
        /// </summary>
        /// <param name="key"></param>
        public void SendKeys(string key)
        {
            Info(string.Format("Typing '{0}'", key));
            WaitForElementIsPresent();
            Browser.GetDriver().FindElement(Locator).SendKeys(key);
        }
         
        /// <summary>
        /// gets the value of the title attribute
        /// </summary>
        /// <param name="attr">attribute of the element</param>
        /// <returns>the value of the title attribute of the element</returns>
        public string GetAttribute(string attr)
        {
            WaitForElementIsPresent();
            return Element.GetAttribute(attr);
        }

        /// <summary>
        /// get attribute value from element that is not displayed
        /// </summary>
        /// <param name="attr">attribute</param>
        /// <returns>value of attribute</returns>
        public string GetAttributeInvisible(string attr)
        {
            WaitForElementExists();
            return Element.GetAttribute(attr);
        }

        /// <summary>
        /// gets the value
        /// </summary>
        /// <returns>the value of the element</returns>
        public string GetValue()
        {
            WaitForElementIsPresent();
            return GetAttribute("value");
        }

        /// <summary>
        /// gets the value
        /// </summary>
        /// <returns>the value of the element</returns>
        public string GetValueInvisible()
        {
            WaitForElementExists();
            return Element.GetAttribute("value");
        }

        /// <summary>
        /// get the text of the element
        /// </summary>
        /// <returns>the text of the element</returns>
        public string GetText()
        {
            WaitForElementIsPresent();
            return Element.Text;
        }

        /// <summary>
        /// focus on the element and send key ""
        /// </summary>
        public void FocusWithKeys()
        {
            Focus();
            try
            {
                Element.SendKeys("");
            }
            catch (Exception)
            {
                Info("Focused");
            }
        }

        /// <summary>
        /// focuses the element
        /// </summary>
        public void Focus()
        {
            Info("Focusing");
            WaitForElementIsPresent();
            new Actions(Browser.GetDriver()).MoveToElement(Element).Build().Perform();
        }
        
        /// <summary>
        /// abstract method for get the type of the element 
        /// </summary>
        protected abstract string GetElementType();

        /// <summary>
        /// right click
        /// </summary>
        public void ClickRight()
        {
            WaitForElementIsPresent();
            Info("Right clicking");
            var action = new Actions(Browser.GetDriver());
            action.ContextClick(Element);
            action.Perform();
        }

        /// <summary>
        /// verify that the drop-down element is minimized (performed by a class member)
        /// </summary>
        /// <returns>true if collapsed</returns>
        public bool IsCollapsed()
        {
            WaitForElementIsPresent();
            String elementClass = Element.GetAttribute("class");
            return elementClass.ToLower().Contains("collapse");
        }

        /// <summary>
        /// set value via javascript <b>document.getElementById('{0}').value='{1}' </b>
        /// </summary>
        /// <param name="elementId">Element Id</param>
        /// <param name="value">Value</param>
        public void SetValueViaJs(string elementId, string value)
        {
            try
            {
                ((IJavaScriptExecutor) Browser.GetDriver()).ExecuteScript(string.Format(
                    "document.getElementById('{0}').value='{1}'", elementId, value), Element);
            }
            catch (Exception r)
            {
                Logger.Instance.Warn(r.Message);
            }
        }

        /// <summary>
        /// set innerHtml via javascript <b>arguments[0].innerHTML='{0}' </b>
        /// </summary>
        /// <param name="value">value</param>
        public void SetInnerHtml(string value)
        {
            WaitForElementIsPresent();
            AssertIsPresent();
            Element.Click();
            Info("Ввод текста '" + value + "'");
            // clear fields
            ((IJavaScriptExecutor) Browser.GetDriver()).ExecuteScript("arguments[0].innerHTML=\"\";", Element);
            // element.sendKeys(value);
            ((IJavaScriptExecutor) Browser.GetDriver()).ExecuteScript("arguments[0].innerHTML=\"" + value + "\";",
                Element);
        }

        /// <summary>
        /// set value via javascript <b>arguments[0].value='{0}' </b>
        /// </summary>
        /// <param name="value"></param>
        public void SetValueViaJs(string value)
        {
            WaitForElementIsPresent();
            Element.Click();
            Info("Ввод текста '" + value + "'");
            // clear fields
            ((IJavaScriptExecutor)Browser.GetDriver()).ExecuteScript("arguments[0].value=\"\";", Element);
            // element.sendKeys(value);
            ((IJavaScriptExecutor)Browser.GetDriver()).ExecuteScript("arguments[0].value=\"" + value + "\";", Element);
        }

        /// <summary>
        /// enum to set expected condition for explicit wait
        /// </summary>
        public enum ExpectedConditions
        {
            /// <summary>
            /// condition when element exists in the html source code
            /// </summary>
            ElementExists,
            /// <summary>
            /// condition when element exists in the html source code and visible now
            /// </summary>
            ElementIsVisible
        }

        /// <summary>
        /// set explicit wait
        /// </summary>
        /// <param name="condition">Expected condition for explicit wait</param>
        /// <param name="seconds">Explicit wait timeout</param>
        public void ExplicitWait(ExpectedConditions condition, int seconds)
        {
            try
            {
                var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(seconds));
                switch (condition.ToString())
                {
                    case "ElementExists":
                        Element = wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.ElementExists(Locator));
                        break;
                    case "ElementIsVisible":
                        Element = wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.ElementIsVisible(Locator));
                        break;
                }
            }
            catch (TimeoutException ex)
            {
                Browser.GetDriver().Navigate().Refresh();
                if (!IsPresent())
                {
                    Logger.Instance.Fail(ex.Message + "\n" + FormatLogMsg(" is not Present"));
                }
            }
        }
    }
}
