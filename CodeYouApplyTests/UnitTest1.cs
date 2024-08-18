using CodeYouApplyTests.Selectors;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V125.Page;
using OpenQA.Selenium.Support.UI;
using System.Globalization;

namespace CodeYouApplyTests
{
    public class Tests
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
             _wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(500));
            _random = new Random();
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

        private void ClickViaJavaScript(IWebElement element)
        {
            // Simply using submitButton.Click() kept giving "click intercepted" errors
            // Clicking via JavaScript works just fine, though
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)_driver;
            javaScriptExecutor.ExecuteScript("arguments[0].click();", element);
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
            NavigateTo(ApplicationPage.Url);

            var submitButton = FindElement(ApplicationPage.SubmitButton);
            var birthdateInput = FindElement(ApplicationFormFields.BirthDateInput);
            birthdateInput.SendKeys("88-88");

            ClickViaJavaScript(submitButton);
            DismissAlert();

            var errorText = FindElement(ApplicationFormFields.BirthDateErrorMessage).Text;

            Assert.That(errorText, Is.EqualTo(ApplicationPage.InvalidDateErrorText));
        }

        [Test]
        public void FormSubmission_FailsAndDisplaysDateRangeError_WhenBirthDateIsFutureOrAgeUnderEighteen()
        {
            var birthDateInputText = GetRandomDate().ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            NavigateTo(ApplicationPage.Url);

            var submitButton = FindElement(ApplicationPage.SubmitButton);
            var birthdateInput = FindElement(ApplicationFormFields.BirthDateInput);
            birthdateInput.SendKeys(birthDateInputText);

            ClickViaJavaScript(submitButton);
            DismissAlert();

            var errorText = FindElement(ApplicationFormFields.BirthDateErrorMessage);
            Assert.That(errorText.Text, Is.EqualTo(ApplicationFormFields.GetExpectedBirthDateRangeErrorMsg()));
        }

        [Test]
        public void ComputerSkillsRadioButtonGroup_OnlyAllowsOneSelectionAtATime()
        {
            NavigateTo(ApplicationPage.Url);
            var radioButtons = FindElement(
                ApplicationFormFields.ComputerSkillsRadioButtonGroup).GetChildrenOfType("span//input");

            SelectRandomElementInCollection(radioButtons);
            SelectRandomElementInCollection(radioButtons);
            SelectRandomElementInCollection(radioButtons);
            SelectRandomElementInCollection(radioButtons);
            SelectRandomElementInCollection(radioButtons);

            int selectedCount = GetSelectedItemsCount(radioButtons);

            Assert.That(selectedCount, Is.AtMost(1));
        }

        [Test]
        public void RaceCheckboxGroup_ClearsAllOtherSelections_WhenSelectingPreferNotToSay()
        {
            NavigateTo(ApplicationPage.Url);

            var raceCheckBoxes = FindElement(
                ApplicationFormFields.RaceCheckboxGroup).GetChildrenOfType("span//input");

            SelectRandomElementInCollection(raceCheckBoxes, raceCheckBoxes.Count - 2);
            SelectRandomElementInCollection(raceCheckBoxes, raceCheckBoxes.Count - 2);
            SelectRandomElementInCollection(raceCheckBoxes, raceCheckBoxes.Count - 2);

            ClickViaJavaScript(raceCheckBoxes[raceCheckBoxes.Count - 1]);
            int selectedCount = GetSelectedItemsCount(raceCheckBoxes);

            Assert.That(selectedCount, Is.EqualTo(1));
        }

        private static int GetSelectedItemsCount(IList<IWebElement> raceCheckBoxes)
        {
            var selectedCount = 0;
            foreach (var checkbox in raceCheckBoxes)
            {
                if (checkbox.Selected) selectedCount++;
            }

            return selectedCount;
        }

        [Test]
        public void GovernmentServicesCheckboxGroup_DoesNotExistWhenPageInitiallyLoaded()
        {
            NavigateTo(ApplicationPage.Url);

            var governmentServicesCheckboxGroup = _driver.FindElements(By.XPath(ApplicationFormFields.GovernmentServicesCheckboxGroup));

            Assert.That(governmentServicesCheckboxGroup, Has.Count.EqualTo(0));
        }

        [TestCase("IN")]
        public void CountyDropdown_DisplaysOnlyValidCounties_ForSelectedState(string state)
        {
            NavigateTo(ApplicationPage.Url);
            var stateDropdown = new SelectElement(FindElement(ApplicationFormFields.StateDropdownList));
            stateDropdown.SelectByText(state);

            var countyDropdown = new SelectElement(FindElement(ApplicationFormFields.CountyDropdownList));
            var displayedCounties = countyDropdown.Options.Where(
                option => ApplicationFormFields.ValidCountiesInState[state].Contains(option.Text))
                .ToList();

            var distinctCounties = displayedCounties.DistinctBy(option => option.Text);

            var countyText = distinctCounties.Select(option => option.Text).ToList();

            Assert.That(distinctCounties.Count, Is.EqualTo(ApplicationFormFields.ValidCountiesInState[state].Count));
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

        private List<IWebElement> FindElements(string selector)
        {
            return [.. _driver.FindElements(By.XPath(selector))];
        }

        private string GetAlertText()
        {
            return _driver.SwitchTo().Alert().Text;
        }

        private void DismissAlert()
        {
            _driver.SwitchTo().Alert().Dismiss();
        }

        private static IList<IWebElement> GetSelectOptions(SelectElement selectElement)
        {
            return selectElement.Options;
        }

        private List<string> GetSelectOptionsAsStrings(SelectElement selectElement)
        {
            List<string> optionsAsStrings = [];

            foreach (var option in selectElement.Options)
            {
                optionsAsStrings.Add(option.Text);
            }

            return optionsAsStrings;
        }

        private DateTime GetRandomDate()
        {
            int month = _random.Next(1, 12);
            int day;

            if (month == 2)
            {
                day = _random.Next(1, 28);
            }
            else if (month == 4 ||
                month == 6 ||
                month == 9 ||
                month == 11)
            {
                day = _random.Next(1, 30);
            } else
            {
                day = _random.Next(1, 31);
            }

            var year = DateTime.Today.AddYears(_random.Next(-100, 100));

            return new DateTime(year.Year, month, day);
        }

        private void SelectRandomElementInCollection(IList<IWebElement> elements, int? maxIndex = null, int minIndex = 0)
        {
            maxIndex ??= elements.Count;
            var randomIndex = _random.Next(minIndex, (int)maxIndex);

            ClickViaJavaScript(elements[randomIndex]);
        }
    }
}