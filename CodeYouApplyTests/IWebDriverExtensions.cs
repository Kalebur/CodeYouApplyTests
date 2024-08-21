using OpenQA.Selenium;
using System.Runtime.CompilerServices;

namespace ApplicationPageTests
{
    public static class IWebDriverExtensions
    {
        public static bool AlertDisplayed(this IWebDriver _driver)
        {
            try
            {
                _driver.SwitchTo().Alert();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static string GetAlertText(this IWebDriver _driver)
        {
            return _driver.SwitchTo().Alert().Text;
        }

        public static void DismissAlert(this IWebDriver _driver)
        {
            _driver.SwitchTo().Alert().Dismiss();
        }
    }
}
