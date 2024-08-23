using OpenQA.Selenium;
using System.Reflection;

namespace ApplicationPageTests
{
    public static class IWebElementExtensions
    {
        private static IWebDriver? _cachedDriver = null;

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

        public static void ClickViaJavaScript(this IWebElement element)
        {
            if (_cachedDriver is null)
            {
                var elementDriver = element.GetType().GetProperty("WrappedDriver")?.GetValue(element);
                if (elementDriver is null)
                {
                    throw new Exception($"{nameof(element)} does not have a web driver.");
                }
                _cachedDriver = elementDriver as IWebDriver;
            }

            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)_cachedDriver!;
            javaScriptExecutor.ExecuteScript("arguments[0].click();", element);
        }
    }
}
