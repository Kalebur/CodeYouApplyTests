using OpenQA.Selenium;

namespace CodeYouApplyTests.Selectors
{
	public class ApplicationPage
	{
		private static IWebDriver _driver;

		public ApplicationPage(IWebDriver driver)
		{
			_driver = driver;
		}

		// This should be named something more specific than Url...maybe something like applyPageUrl
		public static string Url { get; set; } = "https://code-you.org/apply/";

		// Locators are usually written like this
		public IWebElement submitButton => _driver.FindElement(By.XPath("//input[@id='submit_button']"));
		// then you do not have to invoke a find element method in your test class
		public static string SubmitButton { get; set; } = "//input[@id='submit_button']";

		public static string StateDropdown { get; set; } = "(//select[@id='tfa_220'])[1]";

		// Maybe make this a string method that accepts a parameter for the number of errors present in the page
		// This will only be valid if you have exactly 28 errors on the page and that number can change depending on what you leave blank in the form
		public static string BlankSubmissionErrorText { get; set; } = "The form is not complete and has not " +
						"been submitted yet. There are 28 problems with your submission.";

		public static string FormIntroText { get; set; } = "//span[contains(text()," +
				"'Please use this form to sign-up for Code:You. Appl')]";

		public static string ExpectedFormIntroText { get; set; } = "Please use this form to sign-up for Code:You. Applicants will be sent pre-work for the course on a rolling basis until spots are filled. Please note we have a considerable waiting list to get into the program – it could be many months (even a year or more) before your spot comes up in line.\r\n\r\nApplicants will be contacted to complete the necessary pre-work for the upcoming course a month or two before the class is slated to begin. Please check your email and be sure to whitelist info@code-you.org to receive periodic updates from our team.\r\n  \r\nIMPORTANT NOTE: Due to overwhelming interest in the program, we have a very long waiting list to get into the program. Sometimes a year or even more. Please be patient, but in the meantime you are welcome to contact info@code-you.org for any questions.";

		public static List<string> ValidStateOptions { get; set; } = ["IN", "KY", "OH"];
		public static string InvalidDateErrorText { get; set; } = "This does not appear to be a valid date.";

		public static string Form { get; set; } = "//div[@id='tfa_515']";
	}
}
