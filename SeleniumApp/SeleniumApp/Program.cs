using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace SeleniumApp
{
    class Program
    {
        enum LanguageType
        {
            Russian=0,
            English=1,
        }
        class Vacancies
        {
            public string header { get; set; }
            public string body { get; set; }
            public string footer { get; set; }
        }
        static string link = "https://careers.veeam.ru/vacancies";
        static OpenQA.Selenium.Chrome.ChromeDriver chrome;
        static IJavaScriptExecutor executor;
        static void Main(string[] args)
        {
            InitBrowser();
            var result = FindCountVacancies(LanguageType.Russian);
            if (result != null)
            {
                Console.WriteLine("Количество найденных вакансий {0}:", result.Count());
                int i = 1;
                foreach (var s in result)
                {
                    Console.WriteLine("{3}:{0}\n{1}\n{2}", s.header, s.body, s.footer, i);
                    i++;
                }
            }
            Console.ReadKey();
        }
        static void InitBrowser()
        {
            chrome = new OpenQA.Selenium.Chrome.ChromeDriver();
            chrome.Navigate().GoToUrl(link);
            chrome.Manage().Window.FullScreen();
        }
        static List<Vacancies> FindCountVacancies(LanguageType language)
        {
            ScriptClick(FindButtonByInnerText("Все отделы"));
            var links_of_departments = chrome.FindElementsByTagName("a");
            var departments = links_of_departments.Where(x => x.GetAttribute("class") == "dropdown-item");
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            int counter = 1;
            Console.WriteLine("Выберите интересующий отдел:");
            foreach (var s in departments)
            {
                dictionary.Add(counter, s.Text);
                counter++;
                Console.WriteLine("{0}:{1}", counter, s.Text);
            }
            var selected = Console.ReadLine();
            int _case = 0;
            while (true)
            {
                if (Int32.TryParse(selected, out _case) == true)
                {
                    if (_case >= dictionary.Count() || _case < 1)
                    {
                        Console.WriteLine("Неверный номер, выберите другой");
                    }
                    else break;
                }
                else
                    Console.WriteLine("Введенное значение не является числом");
                selected = Console.ReadLine();
            }
            var department_link = chrome.FindElementByLinkText(dictionary[_case]);
            ScriptClick(department_link);
            var buttons_in_dropdown = chrome.FindElementsByTagName("button");
            if (buttons_in_dropdown != null && buttons_in_dropdown.Count() > 0)
            {
                var button_language = buttons_in_dropdown.Where(x => x.Text == "Все языки").First();
                ScriptClick(button_language);
            }
            var input_language = chrome.FindElementsByTagName("div").Where(x => x.GetAttribute("class") == "show dropdown").First();
            string str_language = "";
            switch (language)
            {
                case (LanguageType.Russian):
                    {
                        str_language = "lang-option-0";
                        break;
                    }
                case (LanguageType.English):
                    {
                        str_language = "lang-option-1";
                        break;
                    }
            }

            var checkboxs = input_language.FindElements(By.TagName("input")).Where(x => x.GetAttribute("id") == str_language);
            if (checkboxs != null && checkboxs.Count() > 0)
            {
                var input_checkbox = checkboxs.First();
                ScriptClick(input_checkbox);
            }
            var tt = chrome.FindElementsByTagName("a").Where(x => x.GetAttribute("class") == "card card-no-hover card-sm");
            var result = new List<Vacancies>();
            foreach (var s in tt)
            {
                string header = "", body = "", footer = "";
                var div = s.FindElements(By.TagName("div"));
                foreach (var t in div)
                {
                    var classname = t.GetAttribute("class");
                    if (classname.StartsWith("card-header"))
                    {
                        header = t.Text;
                    }
                    else if (classname.StartsWith("card-body"))
                    {
                        body = t.Text;
                    }
                    else if (classname.StartsWith("card-footer"))
                    {
                        footer = t.Text;
                    }
                }
                result.Add(new Vacancies() { header = header, body = body, footer = footer });
            }
            return result;
        }
        static void ScriptClick(IWebElement element)
        {
            if (element == null)
                return;
            if (executor == null)
                executor = chrome as IJavaScriptExecutor;
            executor.ExecuteScript("arguments[0].click();", element);
        }
        static IWebElement FindButtonByInnerText(string text)
        {
            var buttons = chrome.FindElementsByTagName("button");
            if (buttons != null && buttons.Count() > 0)
            {
                var button = buttons.Where(x => x.Text == text);
                if (button != null && button.Count() > 0)
                {
                    return button.First();
                }
            }
            return null;
        }
    }
}
