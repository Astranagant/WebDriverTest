using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;

using OpenQA.Selenium.Support.UI;

namespace WebDriverTEst
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new InternetExplorerDriver();

            driver.Navigate().GoToUrl("http://www.google.com");

            IWebElement query = driver.FindElement(By.Name("q"));

            query.SendKeys("google test");

            query.Submit();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(t => t.Title.ToLower().StartsWith("google test"));

            System.Console.WriteLine(string.Format("Page title is: {0}", driver.Title));

            driver.Quit();
        }
    }
}
