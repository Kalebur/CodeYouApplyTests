using OpenQA.Selenium;

namespace CodeYouApplyTests
{
    public static class IWebElementExtensions
    {
        public static IList<IWebElement> GetChildren(this IWebElement element)
        {
            return element.FindElements(By.XPath(".//*"));
        }
    }
}
