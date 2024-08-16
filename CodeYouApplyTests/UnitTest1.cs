using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CodeYouApplyTests
{
    public class Tests
    {
        private IWebDriver _driver;
        private readonly string homepageUrl = "https://code-you.org";
        private readonly string applyUrl = "/apply/";

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
        }

        [Test]
        public void FormSubmission_DisplaysErrorWithSpecificText_WhenBlankFormSubmitted()
        {
            var expectedAlertText = "The form is not complete and has not " +
                "been submitted yet. There are 28 problems with your submission.";
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

            NavigateTo(homepageUrl);
            var applyLink = _driver.FindElement(By.XPath("//li[@id='menu-item-44']//a[normalize-space()='Apply']"));
            applyLink.Click();

            var submitButton = _driver.FindElement(By.XPath("//input[@id='submit_button']"));

            // Simply using submitButton.Click() kept giving "click intercepted" errors
            // Clicking via JavaScript works just fine, though
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)_driver;
            javaScriptExecutor.ExecuteScript("arguments[0].click();", submitButton);

            wait.Until((_driver) => AlertDisplayed());

            var alert = _driver.SwitchTo().Alert();
            var alertText = alert.Text;
            alert.Dismiss();

            Assert.That(expectedAlertText, Is.EqualTo(alertText));
        }

        [Test]
        public void HomepageApplyLink_RedirectsToCorrectUri_WhenClicked()
        {
            var expectedUrl = homepageUrl + applyUrl;

            NavigateTo(homepageUrl);
            ClickElement(GetElementBySelector());

            Assert.That(expectedUrl, Is.EqualTo(_driver.Url));
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        private bool AlertDisplayed()
        {
            try
            {
                _driver.SwitchTo().Alert();
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void NavigateTo(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }

        private static void ClickElement(IWebElement element)
        {
            element.Click();
        }

        private IWebElement GetElementBySelector(string selector)
        {
            return _driver.FindElement(By.XPath(selector));
        }
    }
}