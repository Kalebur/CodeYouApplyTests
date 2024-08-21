using OpenQA.Selenium;

namespace CodeYouApplyTests.Selectors
{
    public class HomePage(IWebDriver driver)
    {
        public string Url { get; set; } = "http://code-you.org";
        public IWebElement ApplyLink => driver.FindElement(By.XPath("//li[@id='menu-item-44']//a[normalize-space()='Apply']"));
    }
}
