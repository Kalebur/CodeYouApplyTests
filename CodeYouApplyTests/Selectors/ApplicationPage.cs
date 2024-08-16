using NUnit.Framework.Internal.Execution;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace CodeYouApplyTests.Selectors
{
    public static class ApplicationPage
    {
        public static string Url { get; set; } = "https://code-you.org/apply/";
        public static string SubmitButton { get; set; } = "//input[@id='submit_button']";
        public static string StateDropdown { get; set; } = "(//select[@id='tfa_220'])[1]";
        public static string BlankSubmissionErrorText { get; set; } = "The form is not complete and has not " +
                "been submitted yet. There are 28 problems with your submission.";

        public static string FormIntroText { get; set; } = "//span[contains(text()," +
            "'Please use this form to sign-up for Code:You. Appl')]";

        public static string ExpectedFormIntroText { get; set; } = "Please use this form to sign-up for Code:You. Applicants will be sent pre-work for the course on a rolling basis until spots are filled. Please note we have a considerable waiting list to get into the program – it could be many months (even a year or more) before your spot comes up in line.\r\n\r\nApplicants will be contacted to complete the necessary pre-work for the upcoming course a month or two before the class is slated to begin. Please check your email and be sure to whitelist info@code-you.org to receive periodic updates from our team.\r\n  \r\nIMPORTANT NOTE: Due to overwhelming interest in the program, we have a very long waiting list to get into the program. Sometimes a year or even more. Please be patient, but in the meantime you are welcome to contact info@code-you.org for any questions.";

        public static List<string> ValidStateOptions { get; set; } = ["IN", "KY", "OH"];

        public static string Form { get; set; } = "//div[@id='tfa_515']";
    }
}
