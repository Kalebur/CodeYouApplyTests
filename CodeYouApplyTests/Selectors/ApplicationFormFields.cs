namespace CodeYouApplyTests.Selectors
{
    public static class ApplicationFormFields
    {
        public static string EmailInput { get; set; } = "//input[@id='tfa_215']";
        public static string EmailErrorMessage { get; set; } = "//div[@id='tfa_215-E']//span";

        public static string BirthDateInput { get; set; } = "//input[@id='tfa_5']";
        public static string BirthDateErrorMessage { get; set; } = "//div[@id='tfa_5-E']//span";

    }
}
