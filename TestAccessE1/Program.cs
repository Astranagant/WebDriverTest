using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace TestAccessE1
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new InternetExplorerDriver();

            driver.Navigate().GoToUrl("http://e1.englishtown.com/partner/coolschool/Default.aspx");

            IWebElement userNameQuery = driver.FindElement(By.Name("UserName"));
            IWebElement passwdQuery = driver.FindElement(By.Name("Password"));

            userNameQuery.SendKeys("cliu9800");
            passwdQuery.SendKeys("pass");

            IWebElement submitButton = driver.FindElement(By.Id("loginTrigger"));

            submitButton.Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(t => t.PageSource.ToLower().IndexOf("currentcourse") > 0);

            System.Console.WriteLine(string.Format("Page title is: {0}", driver.Title));

            driver.Quit();
        }
    }
}
