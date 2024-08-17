using CodeYouApplyTests.Selectors;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using System.ComponentModel.DataAnnotations;

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
            ClickViaJavaScript(submitButton);

            _wait.Until((_driver) => AlertDisplayed());

            var alertText = GetAlertText();

            Assert.That(ApplicationPage.BlankSubmissionErrorText, Is.EqualTo(alertText));
        }

        private void ClickViaJavaScript(IWebElement submitButton)
        {
            // Simply using submitButton.Click() kept giving "click intercepted" errors
            // Clicking via JavaScript works just fine, though
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)_driver;
            javaScriptExecutor.ExecuteScript("arguments[0].click();", submitButton);
        }

        private void ClickViaJavaScript(string xPath)
        {
            var element = FindElement(xPath);
            ClickViaJavaScript(element);
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
            IWebElement? birthdayField = null;

            NavigateTo(ApplicationPage.Url);
            var submitButton = FindElement(ApplicationPage.SubmitButton);
            var birthdateInput = FindElement("//input[@id='tfa_5']");
            birthdateInput.SendKeys("88-88");
            ClickViaJavaScript(submitButton);
            DismissAlert();

            var formFields = FindElement(ApplicationPage.Form).GetChildrenOfType("div");
            
            foreach (var field in formFields)
            {
                var fieldChildren = field.GetChildren();
                foreach (var child in fieldChildren)
                {
                    if (child.Text.ToLower().Contains("birth"))
                    {
                        birthdayField = child;
                    }
                }
            }

            var birthdayFieldId = ParseIdToBase(birthdayField.GetAttribute("id"));
            var errorSpan = birthdayField.FindElement(By.XPath("//div[@id='tfa_5-E']"));


            Assert.That(errorSpan.Text, Is.EqualTo(ApplicationPage.InvalidDateErrorText));
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

        private void ClickElement(string xPath)
        {
            var element = _driver.FindElement(By.XPath(xPath));
            ClickElement(element);
        }

        private IWebElement FindElement(string selector)
        {
            return _driver.FindElement(By.XPath(selector));
        }

        private string GetAlertText()
        {
            return _driver.SwitchTo().Alert().Text;
        }

        private void DismissAlert()
        {
            _driver.SwitchTo().Alert().Dismiss();
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

        private string ParseIdToBase(string id)
        {
            return id[0..id.IndexOf('-')];
        }
    }
}