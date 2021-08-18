using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumApp
{
    //расширения
    public static class Extenstions
    {
        //попытка кликнуть обычным методом, если вызвало ошибку то кликаем через скрипт
        public static void TryClick(this IWebElement element)
        {
            if (element == null)
                throw new ArgumentNullException();
            if (element.Displayed && element.Enabled)
            {
                try
                {
                    element.Click();
                }
                catch (OpenQA.Selenium.ElementClickInterceptedException)
                {
                    element.ScriptExecuter();
                }
                catch (Exception)
                { throw new Exception(); }
            }
            else
            {
                element.ScriptExecuter();
            }
        }
        private static void ScriptExecuter(this IWebElement element)
        {
            var driver = ((IWrapsDriver)element).WrappedDriver;
            var executor = driver as IJavaScriptExecutor;
            executor.ExecuteScript("arguments[0].click();", element);
        }
    }
}
