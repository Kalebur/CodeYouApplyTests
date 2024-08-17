using OpenQA.Selenium;

namespace CodeYouApplyTests
{
    public static class IWebElementExtensions
    {
        public static IList<IWebElement> GetChildren(this IWebElement element)
        {
            return element.FindElements(By.XPath(".//*"));
        }

        public static IWebElement GetParent(this IWebElement element)
        {
            return element.FindElement(By.XPath(".."));
        }

        public static IList<IWebElement> GetChildrenOfType(this IWebElement element, string type)
        {
            return element.FindElements(By.XPath($"./child::{type}"));
        }
    }
}
