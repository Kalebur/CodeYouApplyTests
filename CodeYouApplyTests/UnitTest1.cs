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
		private IWebDriver _driver;
		private WebDriverWait _wait;
		private Random _random;
		// Declare the classes you will need access to up here
		private ApplicationPage _applicationPage;
		private HomePage _homePage;

		[SetUp]
		public void Setup()
		{
			_driver = new ChromeDriver();
			_wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(500));
			_random = new Random();
			// I would add this line of code to maximize the browser window to allow for more visibility when running your tests locally
			_driver.Manage().Window.Maximize();
			// I would also go ahead and initialize the classes you will need in your test class here
			_applicationPage = new ApplicationPage(_driver);
			_homePage = new HomePage();
		}

		[Test]
		public void FormSubmission_DisplaysErrorWithSpecificText_WhenBlankFormSubmitted()
		{
			// Why not just _driver.Navigate().GoToUrl(HomePage.Url);
			NavigateTo(HomePage.Url);
			ClickElement(FindElement(HomePage.ApplyLink));

			// With the submitButton locator changed you can just use 
			_applicationPage.submitButton.ClickViaJavaScript();
			//var submitButton = FindElement(ApplicationPage.SubmitButton);
			//ClickViaJavaScript(submitButton);

			_wait.Until((_driver) => AlertDisplayed());

			var alertText = GetAlertText();

			Assert.That(ApplicationPage.BlankSubmissionErrorText, Is.EqualTo(alertText));
		}

		[Test]
		public void HomepageApplyLink_RedirectsToCorrectUri_WhenClicked()
		{
			NavigateTo(HomePage.Url);
			// The click element method might be a little overkill. Using the submitButton example, I would just so something like 
			// _applicationPage.submitButton.Click();
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

			submitButton.ClickViaJavaScript();
			DismissAlert();

			var errorText = FindElement(ApplicationFormFields.BirthDateErrorMessage).Text;

			Assert.That(errorText, Is.EqualTo(ApplicationPage.InvalidDateErrorText));
		}

		// I would break this into TestCases that test both under 18 and birthdates in the future using the [TestCase] attribute
		[TestCase("future")]
		[TestCase("under18")]
		public void FormSubmission_FailsAndDisplaysDateRangeError_WhenBirthDateIsFutureOrAgeUnderEighteen(string rangeType)
		{
			var birthDateInputText = GetRandomBirthdate(rangeType).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
			NavigateTo(ApplicationPage.Url);

			var submitButton = FindElement(ApplicationPage.SubmitButton);
			var birthdateInput = FindElement(ApplicationFormFields.BirthDateInput);
			birthdateInput.SendKeys(birthDateInputText);
			submitButton.ClickViaJavaScript();

			//ClickViaJavaScript(submitButton);
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

			// Theoretically this could select and unselect the same checkbox
			// These select/ click the boxes
			SelectRandomElementInCollection(raceCheckBoxes, raceCheckBoxes.Count - 2);
			SelectRandomElementInCollection(raceCheckBoxes, raceCheckBoxes.Count - 2);
			SelectRandomElementInCollection(raceCheckBoxes, raceCheckBoxes.Count - 2);
			// This also selects a checkbox
			raceCheckBoxes[raceCheckBoxes.Count - 1].ClickViaJavaScript();
			// This count could be 1 to 3
			int selectedCount = GetSelectedItemsCount(raceCheckBoxes);

			// Why is this set to Is.EqualTo(1)?...this number could be anywhere from 1 to 4 depending on what is selected
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
		[TestCase("OH")]
		[TestCase("KY")]
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

		#region Private Helper Methods

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

		// If you are going to write helper methods for the driver, they should go in their own helper class instead of in the test class
		// Assumingly you would use these helpers in all of your test classes

		// Although I would argue that some of these methods are not needed at all. You have written them to only except XPath and there are multiple ways to find an element other than XPath. 
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
			int month = _random.Next(1, 13);
			int day;

			if (month == 2)
			{
				day = _random.Next(1, 29);
			}
			else if (month == 4 ||
					month == 6 ||
					month == 9 ||
					month == 11)
			{
				day = _random.Next(1, 31);
			}
			else
			{
				day = _random.Next(1, 32);
			}

			var year = DateTime.Today.AddYears(_random.Next(-100, 100));

			return new DateTime(year.Year, month, day);
		}
		// Here is an updated birthdate generator that you can choose what kind of birthdate you want
		private DateTime GetRandomBirthdate(string option)
		{
			int month = _random.Next(1, 13); // Corrected to include December
			int day;

			if (month == 2)
			{
				day = _random.Next(1, 29); // Corrected to include the 28th day
			}
			else if (month == 4 || month == 6 || month == 9 || month == 11)
			{
				day = _random.Next(1, 31); // Corrected to include the 30th day
			}
			else
			{
				day = _random.Next(1, 32); // Corrected to include the 31st day
			}

			int year;
			switch (option.ToLower())
			{
				case "future":
					year = DateTime.Today.AddYears(_random.Next(1, 100)).Year;
					break;
				case "under18":
					year = DateTime.Today.AddYears(-_random.Next(0, 18)).Year;
					break;
				case "valid":
				default:
					year = DateTime.Today.AddYears(-_random.Next(18, 100)).Year;
					break;
			}

			return new DateTime(year, month, day);
		}

        private void SelectRandomElementInCollection(IList<IWebElement> elements, int? maxIndex = null, int minIndex = 0)
		{
			maxIndex ??= elements.Count;
			var randomIndex = _random.Next(minIndex, (int)maxIndex);

			elements[randomIndex].ClickViaJavaScript();
		}

		#endregion Private Helper Methods
	}
}
