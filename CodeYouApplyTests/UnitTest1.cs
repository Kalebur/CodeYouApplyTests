using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CodeYouApplyTests
{
    public class Tests
    {
        private IWebDriver _driver;
        private string homepageUri = "https://code-you.org";
        private string applyUriPath = "/apply/";

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
        }

        [Test]
        public void Test1()
        {
            var expectedAlertText = "The form is not complete and has not " +
                "been submitted yet. There are 28 problems with your submission.";
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

            _driver.Navigate().GoToUrl(homepageUri);
            var applyLink = _driver.FindElement(By.XPath("//li[@id='menu-item-44']//a[normalize-space()='Apply']"));
            applyLink.Click();

            var submitButton = _driver.FindElement(By.XPath("//input[@id='submit_button']"));
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)_driver;
            javaScriptExecutor.ExecuteScript("arguments[0].click();", submitButton);

            wait.Until((_driver) => AlertDisplayed());

            var alert = _driver.SwitchTo().Alert();
            var alertText = alert.Text;
            alert.Dismiss();

            Assert.That(expectedAlertText, Is.EqualTo(alertText));
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
            catch (NoAlertPresentException e)
            {
                return false;
            }

            return true;
        }
    }
}