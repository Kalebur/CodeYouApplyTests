using CodeYouApplyTests.Selectors;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CodeYouApplyTests
{
    public class Tests
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
             _wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(500));
        }

        [Test]
        public void FormSubmission_DisplaysErrorWithSpecificText_WhenBlankFormSubmitted()
        {

            NavigateTo(HomePage.Url);
            ClickElement(FindElement(HomePage.ApplyLink));

            var submitButton = FindElement(ApplicationPage.SubmitButton);

            // Simply using submitButton.Click() kept giving "click intercepted" errors
            // Clicking via JavaScript works just fine, though
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)_driver;
            javaScriptExecutor.ExecuteScript("arguments[0].click();", submitButton);

            _wait.Until((_driver) => AlertDisplayed());

            var alertText = GetAlertTextAndDismiss();

            Assert.That(ApplicationPage.BlankSubmissionErrorText, Is.EqualTo(alertText));
        }

        [Test]
        public void HomepageApplyLink_RedirectsToCorrectUri_WhenClicked()
        {

            NavigateTo(HomePage.Url);
            ClickElement(FindElement(HomePage.ApplyLink));

            Assert.That(ApplicationPage.Url, Is.EqualTo(_driver.Url));
        }

        [Test]
        public void FormStateDropdown_DisplaysOnlyCoveredStates_WhenSelected()
        {
            NavigateTo(ApplicationPage.Url);
            var stateOptionsAsStrings = GetSelectOptionsAsStrings(
                new SelectElement(FindElement(ApplicationPage.StateDropdown)));

            // The first option should just be the placeholder 'Please select' type text
            // and shouldn't be checked
            stateOptionsAsStrings.RemoveAt(0);


            CollectionAssert.AreEquivalent(ApplicationPage.ValidStateOptions, stateOptionsAsStrings);
        }

        [Test]
        public void ApplicationForm_ContainsCorrectIntroText()
        {
            NavigateTo(ApplicationPage.Url);
            var introText = FindElement(ApplicationPage.FormIntroText).Text;
            var expectedText = ApplicationPage.ExpectedFormIntroText;

            Assert.That(expectedText, Is.EqualTo(introText));
        }

        [Test]
        public void FormSubmission_FailsAndDisplaysInvalidDateError_WhenBirthDateIsInInvalidFormat()
        {
            NavigateTo(ApplicationPage.Url);
            var formFields = FindElement(ApplicationPage.Form).GetChildren();
            var cheese = formFields[0].GetAttribute("id");

            Assert.That(cheese, Is.EqualTo("Nom Nom"));
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

        private IList<IWebElement> GetSelectOptions(SelectElement selectElement)
        {
            return selectElement.Options;
        }

        private IList<string> GetSelectOptionsAsStrings(SelectElement selectElement)
        {
            List<string> optionsAsStrings = [];

            foreach (var option in selectElement.Options)
            {
                optionsAsStrings.Add(option.Text);
            }

            return optionsAsStrings;
        }
    }
}