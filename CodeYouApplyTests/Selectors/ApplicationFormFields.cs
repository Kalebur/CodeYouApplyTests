using System.Globalization;

namespace CodeYouApplyTests.Selectors
{
    public static class ApplicationFormFields
    {
        private static readonly int _maxAge = 99;
        private static readonly int _minAge = 18;

        public static string EmailInput { get; set; } = "//input[@id='tfa_215']";
        public static string EmailErrorMessage { get; set; } = "//div[@id='tfa_215-E']//span";

        public static string BirthDateInput { get; set; } = "//input[@id='tfa_5']";
        public static string BirthDateErrorMessage { get; set; } = "//div[@id='tfa_5-E']//span";

        public static string ComputerSkillsRadioButtonGroup { get; set; } = "//span[@id='tfa_1110']";
        public static string RaceCheckboxGroup { get; set; } = "//span[@id='tfa_794']";
        public static string GovernmentServicesCheckboxGroup { get; set; } = "//*[@id=\"tfa_510\"]";
        public static string StateDropdownList { get; set; } = "//select[@id='tfa_220']";
        public static string CountyDropdownList { get; set; } = "//select[@id='tfa_59']";

        public static List<string> ValidCountiesIN { get; set; } = [
            "Clark, IN", "Crawford", "Dearborn", "Floyd, IN", "Harrison, IN",
            "Scott, IN", "Washington", "Other",
        ];

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
