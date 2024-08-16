namespace CodeYouApplyTests.Selectors
{
    public class HomePage
    {
        public static string Url { get; set; } = "http://code-you.org";
        public static string ApplyLink { get; set; } = "//li[@id='menu-item-44']//a[normalize-space()='Apply']";
    }
}
