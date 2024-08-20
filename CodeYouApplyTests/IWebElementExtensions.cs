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

		// There is an XPath method that gets the children of elements. We will go over this in class next week
		public static IList<IWebElement> GetChildrenOfType(this IWebElement element, string type)
		{
			return element.FindElements(By.XPath($"./child::{type}"));
		}
	}
}
