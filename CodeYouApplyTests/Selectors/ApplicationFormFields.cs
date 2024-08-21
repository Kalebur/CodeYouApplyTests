using OpenQA.Selenium;
using System.Globalization;

namespace CodeYouApplyTests.Selectors
{
	public class ApplicationFormFields(IWebDriver driver)
    {
        private static readonly int _maxAge = 99;
		private static readonly int _minAge = 18;

		public IWebElement EmailInput => driver.FindElement(By.XPath("//input[@id='tfa_215']"));
		public IWebElement EmailErrorMessage => driver.FindElement(By.XPath("//div[@id='tfa_215-E']//span"));

		public IWebElement BirthDateInput => driver.FindElement(By.XPath("//input[@id='tfa_5']"));
		public IWebElement BirthDateErrorMessage => driver.FindElement(By.XPath("//div[@id='tfa_5-E']//span"));

		public IWebElement ComputerSkillsRadioButtonGroup => driver.FindElement(By.XPath("//span[@id='tfa_1110']"));
		public	IWebElement RaceCheckboxGroup => driver.FindElement(By.XPath("//span[@id='tfa_794']"));
		public IWebElement GovernmentServicesCheckboxGroup => driver.FindElement(By.XPath("//*[@id=\"tfa_510\"]"));
		public IWebElement StateDropdownList => driver.FindElement(By.XPath("//select[@id='tfa_220']"));
		public IWebElement CountyDropdownList => driver.FindElement(By.XPath("//select[@id='tfa_59']"));

		public static Dictionary<string, List<string>> ValidCountiesInState { get; set; } = new Dictionary<string, List<string>>
				{
						{"IN", new List<string> {
								"Clark, IN", "Crawford", "Dearborn", "Floyd, IN", "Harrison, IN",
								"Scott, IN", "Washington", "Other",
						}},
						{"OH", new List<string>
						{
								"Butler", "Clermont", "Hamilton", "Warren", "Other",
						}},
						{"KY", new List<string>
						{
								"Adair", "Anderson","Bath", "Bell", "Boone", "Bourbon","Boyd", "Boyle",
								"Breathitt", "Bullitt", "Campbell", "Carroll", "Carter", "Casey", "Clark", "Clay",
								"Clinton", "Cumberland", "Edmonson", "Elliott", "Estill", "Fayette", "Fleming",
								"Floyd", "Franklin", "Gallatin", "Garrard", "Grant", "Green", "Greenup",
								"Harlan", "Harrison", "Hart", "Henry", "Jackson", "Jefferson", "Jessamine", "Johnson",
								"Kenton", "Knott", "Knox", "Laurel", "Lawrence", "Lee", "Leslie", "Letcher", "Lewis",
								"Lincoln", "Madison", "Magoffin", "Martin", "McCreary", "Menifee", "Mercer",
								"Metcalfe", "Monroe", "Montgomery", "Morgan", "Nicholas", "Oldham", "Owen", "Owsley",
								"Pendleton", "Perry", "Pike", "Powell", "Pulaski", "Robertson", "Rockcastle",
								"Rowan", "Russell", "Scott", "Shelby", "Spencer", "Trimble", "Wayne", "Whitley",
								"Wolfe", "Woodford", "Other"
						} }
				};

		public static string GetExpectedBirthDateRangeErrorMsg()
		{
			var youngestBirthDate = DateTime.Today.AddYears(-_minAge);
			var oldestBirthDate = DateTime.Today.AddYears(-_maxAge);

			var formattedYoungestBirthDate = GetFormattedDateTime(youngestBirthDate);
			var formattedOldestBirthDate = GetFormattedDateTime(oldestBirthDate);

			return $"This date must be between {formattedOldestBirthDate} - {formattedYoungestBirthDate}.";
		}

		private static string GetFormattedDateTime(DateTime date)
		{
			return date.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
		}
	}
}
