using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
//using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

namespace SeleniumApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (Page page = new Page(TypeDriver.Chrome, true, TimeSpan.FromSeconds(10)))
            {
                page.GoToUrl("https://careers.veeam.ru/vacancies", true);
                var ListOfVacancies = page.FindCountVacancies(TypeLanguage.Russian, "Разработка продуктов");
                var result = page.ResultVacancies(ListOfVacancies);
                Console.WriteLine($"Количество найденных вакансий {result}");
            }
            Console.ReadKey();
        }
    }
}
