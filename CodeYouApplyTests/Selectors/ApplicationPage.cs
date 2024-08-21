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

		public static string Url { get; set; } = "https://code-you.org/apply/";

		public static IWebElement SubmitButton => _driver.FindElement(By.XPath("//input[@id='submit_button']"));

		public static IWebElement StateDropdown => _driver.FindElement(By.XPath("(//select[@id='tfa_220'])[1]"));
		public static IWebElement FormIntroText => _driver.FindElement(By.XPath("//span[contains(text()," +
				"'Please use this form to sign-up for Code:You. Appl')]"));
		public static IWebElement Form => _driver.FindElement(By.XPath("//div[@id='tfa_515']"));

        public static string GetExpectedErrorAlertText(int expectedErrorCount) => $"The form is not complete and has not " +
						$"been submitted yet. There are {expectedErrorCount} problems with your submission.";


		public static string ExpectedFormIntroText { get; set; } = "Please use this form to sign-up for Code:You. Applicants will be sent pre-work for the course on a rolling basis until spots are filled. Please note we have a considerable waiting list to get into the program – it could be many months (even a year or more) before your spot comes up in line.\r\n\r\nApplicants will be contacted to complete the necessary pre-work for the upcoming course a month or two before the class is slated to begin. Please check your email and be sure to whitelist info@code-you.org to receive periodic updates from our team.\r\n  \r\nIMPORTANT NOTE: Due to overwhelming interest in the program, we have a very long waiting list to get into the program. Sometimes a year or even more. Please be patient, but in the meantime you are welcome to contact info@code-you.org for any questions.";

		public static List<string> ValidStateOptions { get; set; } = ["IN", "KY", "OH"];
		public static string InvalidDateErrorText { get; set; } = "This does not appear to be a valid date.";

	}
}
