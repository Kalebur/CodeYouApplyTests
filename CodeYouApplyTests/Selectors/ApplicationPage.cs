namespace CodeYouApplyTests.Selectors
{
    public static class ApplicationPage
    {
        public static string Url { get; set; } = "https://code-you.org/apply/";
        public static string SubmitButton { get; set; } = "//input[@id='submit_button']";
        public static string StateDropdown { get; set; } = "(//select[@id='tfa_220'])[1]";
        public static string BlankSubmissionErrorText { get; set; } = "The form is not complete and has not " +
                "been submitted yet. There are 28 problems with your submission.";

        public static List<string> ValidStateOptions { get; set; } = ["IN", "KY", "OH"];
    }
}
