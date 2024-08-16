using CodeYouApplyTests.Selectors;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CodeYouApplyTests
{
    public class Tests
    {
        private IWebDriver _driver;

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
            var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(500));

            NavigateTo(HomePage.Url);
            ClickElement(FindElement(HomePage.ApplyLink));

            var submitButton = FindElement(ApplicationPage.SubmitButton);

            // Simply using submitButton.Click() kept giving "click intercepted" errors
            // Clicking via JavaScript works just fine, though
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)_driver;
            javaScriptExecutor.ExecuteScript("arguments[0].click();", submitButton);

            wait.Until((_driver) => AlertDisplayed());

            var alertText = GetAlertTextAndDismiss();

            Assert.That(expectedAlertText, Is.EqualTo(alertText));
        }

        [Test]
        public void HomepageApplyLink_RedirectsToCorrectUri_WhenClicked()
        {

            NavigateTo(HomePage.Url);
            ClickElement(FindElement(HomePage.ApplyLink));

            Assert.That(ApplicationPage.Url, Is.EqualTo(_driver.Url));
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

        private IWebElement FindElement(string selector)
        {
            return _driver.FindElement(By.XPath(selector));
        }

        private string GetAlertTextAndDismiss()
        {
            return _driver.SwitchTo().Alert().Text;
        }
    }
}