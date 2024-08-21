using CodeYouApplyTests.Selectors;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Globalization;

// Name this something more specific...like ApplicationPageTests
namespace CodeYouApplyTests
{
    public class Tests
    {
        private readonly int _expectedErrorsForBlankForm = 28;
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private ApplicationPage _applicationPage;
        private ApplicationFormFields _applicationFormFields;
        private HomePage _homePage;
        private TestHelpers _testHelpers;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(500));
            _driver.Manage().Window.Maximize();
            _applicationPage = new ApplicationPage(_driver);
            _applicationFormFields = new ApplicationFormFields(_driver);
            _homePage = new HomePage(_driver);
            _testHelpers = new TestHelpers();
        }

        [Test]
        public void FormSubmission_DisplaysErrorWithSpecificText_WhenBlankFormSubmitted()
        {
            _driver.Navigate().GoToUrl(_homePage.Url);
            _homePage.ApplyLink.Click();

            ApplicationPage.SubmitButton.ClickViaJavaScript();
            _wait.Until((_driver) => _driver.AlertDisplayed());
            var alertText = _driver.GetAlertText();

            Assert.That(ApplicationPage.GetExpectedErrorAlertText(_expectedErrorsForBlankForm), Is.EqualTo(alertText));
        }

        [Test]
        public void HomepageApplyLink_RedirectsToCorrectUri_WhenClicked()
        {
            _driver.Navigate().GoToUrl(_homePage.Url);
            _homePage.ApplyLink.Click();

            Assert.That(ApplicationPage.Url, Is.EqualTo(_driver.Url));
        }

        [Test]
        public void FormStateDropdown_DisplaysOnlyCoveredStates_WhenSelected()
        {
            _driver.Navigate().GoToUrl(ApplicationPage.Url);
            var stateOptionsAsStrings = _testHelpers.GetSelectOptionsAsStrings(
                    new SelectElement(ApplicationPage.StateDropdown));

            // The first option should just be the placeholder 'Please select' type text
            // and shouldn't be checked, so we'll remove it from the possibilities
            stateOptionsAsStrings.RemoveAt(0);

            CollectionAssert.AreEquivalent(ApplicationPage.ValidStateOptions, stateOptionsAsStrings);
        }

        [Test]
        public void ApplicationForm_ContainsCorrectIntroText()
        {
            _driver.Navigate().GoToUrl(ApplicationPage.Url);
            var introText = ApplicationPage.FormIntroText.Text;
            var expectedText = ApplicationPage.ExpectedFormIntroText;

            Assert.That(expectedText, Is.EqualTo(introText));
        }

        [Test]
        public void FormSubmission_FailsAndDisplaysInvalidDateError_WhenBirthDateIsInInvalidFormat()
        {
            _driver.Navigate().GoToUrl(ApplicationPage.Url);

            _applicationFormFields.BirthDateInput.SendKeys("88-88");

            ApplicationPage.SubmitButton.ClickViaJavaScript();
            _driver.DismissAlert();
            var errorText = _applicationFormFields.BirthDateErrorMessage.Text;


            Assert.That(errorText, Is.EqualTo(ApplicationPage.InvalidDateErrorText));
        }

        [TestCase(BirthdateRange.Future)]
        [TestCase(BirthdateRange.Under18)]
        public void FormSubmission_FailsAndDisplaysDateRangeError_WhenBirthDateIsFutureOrAgeUnderEighteen(BirthdateRange rangeType)
        {
            var birthDateInputText = _testHelpers.GetRandomBirthdate(rangeType)
                .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            _driver.Navigate().GoToUrl(ApplicationPage.Url);

            _applicationFormFields.BirthDateInput.SendKeys(birthDateInputText);
            ApplicationPage.SubmitButton.ClickViaJavaScript();

            _driver.DismissAlert();

            var errorText = _applicationFormFields.BirthDateErrorMessage;
            Assert.That(errorText.Text, Is.EqualTo(ApplicationFormFields.GetExpectedBirthDateRangeErrorMsg()));
        }

        [Test]
        public void ComputerSkillsRadioButtonGroup_OnlyAllowsOneSelectionAtATime()
        {
            _driver.Navigate().GoToUrl(ApplicationPage.Url);
            var radioButtons = _applicationFormFields.ComputerSkillsRadioButtonGroup.GetChildrenOfType("span//input");

            _testHelpers.SelectRandomElementInCollection(radioButtons);
            _testHelpers.SelectRandomElementInCollection(radioButtons);
            _testHelpers.SelectRandomElementInCollection(radioButtons);
            _testHelpers.SelectRandomElementInCollection(radioButtons);
            _testHelpers.SelectRandomElementInCollection(radioButtons);

            int selectedCount = TestHelpers.GetSelectedItemsCount(radioButtons);

            Assert.That(selectedCount, Is.AtMost(1));
        }

        [Test]
        public void RaceCheckboxGroup_ClearsAllOtherSelections_WhenSelectingPreferNotToSay()
        {
            _driver.Navigate().GoToUrl(ApplicationPage.Url);

            var raceCheckBoxes = _applicationFormFields.RaceCheckboxGroup.GetChildrenOfType("span//input");

            // Select boxes at random, resulting in 1 or 3 options selected
            _testHelpers.SelectRandomElementInCollection(raceCheckBoxes, raceCheckBoxes.Count - 2);
            _testHelpers.SelectRandomElementInCollection(raceCheckBoxes, raceCheckBoxes.Count - 2);
            _testHelpers.SelectRandomElementInCollection(raceCheckBoxes, raceCheckBoxes.Count - 2);

            // Select the final checkbox in the field, which should be the "Prefer Not to Say" field
            raceCheckBoxes[raceCheckBoxes.Count - 1].ClickViaJavaScript();
            int selectedCount = TestHelpers.GetSelectedItemsCount(raceCheckBoxes);

            // It is expected that clicking "Prefer Not to Say" clears all other fields except itself
            // leaving only 1 field selected
            Assert.That(selectedCount, Is.EqualTo(1));
        }

        [Test]
        public void GovernmentServicesCheckboxGroup_DoesNotExistWhenPageInitiallyLoaded()
        {
            _driver.Navigate().GoToUrl(ApplicationPage.Url);

            var governmentServicesCheckboxGroup = _applicationFormFields.GovernmentServicesCheckboxGroup;
            

            Assert.That(governmentServicesCheckboxGroup.Displayed, Is.Not.True);
        }

        [TestCase("IN")]
        [TestCase("OH")]
        [TestCase("KY")]
        public void CountyDropdown_DisplaysOnlyValidCounties_ForSelectedState(string state)
        {
            _driver.Navigate().GoToUrl(ApplicationPage.Url);
            new SelectElement(_applicationFormFields.StateDropdownList).SelectByText(state);

            var countyDropdown = new SelectElement(_applicationFormFields.CountyDropdownList);
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
    }
}
