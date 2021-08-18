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
    public enum TypeLanguage
    {
        Russian = 0,
        English = 1,
    }
    public enum TypeDriver
    {
        Chrome = 0,
        Firefox = 1,
        IE = 2,
    }
    public class Page : IDisposable
    {
        public void Dispose()
        {
            if (driver != null)
            {
                if (!String.IsNullOrEmpty(driver.CurrentWindowHandle))
                {
                    driver.Close();
                    driver.Quit();
                }
                driver.Dispose();
            }
            //GC.Collect(1,GCCollectionMode.Forced);
            return;
        }
        public class Vacancies
        {
            public string header { get; set; }
            public string body { get; set; }
            public string footer { get; set; }
        }
        public IWebDriver driver;

        public Page(TypeDriver type, bool HideCommandPromptWindow, TimeSpan ts)
        {
            InitBrowser(type, HideCommandPromptWindow, ts);
        }
        public Page()
        {

        }
        /// <summary>
        /// Инициализация веб-драйвера
        /// </summary>
        /// <param name="type">Тип браузера</param>
        /// <param name="HideCommandPromptWindow">Убирать диагностическу информацию из командной строки</param>
        /// <param name="ts">Таймаут время ожидания загрузки страниц, по умолчнию: 500мс</param>
        //Инициализируем веб-драйвер
        public void InitBrowser(TypeDriver type, bool HideCommandPromptWindow, TimeSpan ts)
        {
            try
            {
                switch (type)
                {
                    case (TypeDriver.Chrome):
                        {
                            var service = OpenQA.Selenium.Chrome.ChromeDriverService.CreateDefaultService();
                            SetStatusDiagnostic(service, HideCommandPromptWindow);
                            driver = new OpenQA.Selenium.Chrome.ChromeDriver(service);
                            break;
                        }
                    case (TypeDriver.Firefox):
                        {
                            var service = OpenQA.Selenium.Firefox.FirefoxDriverService.CreateDefaultService();
                            SetStatusDiagnostic(service, HideCommandPromptWindow);
                            driver = new OpenQA.Selenium.Firefox.FirefoxDriver();
                            break;
                        }
                    case (TypeDriver.IE):
                        {
                            var service = OpenQA.Selenium.IE.InternetExplorerDriverService.CreateDefaultService();
                            SetStatusDiagnostic(service, HideCommandPromptWindow);
                            driver = new OpenQA.Selenium.IE.InternetExplorerDriver();
                            break;
                        }
                }
                PageFactory.InitElements(driver, this);
                if (ts.Ticks > 0)
                    driver.Manage().Timeouts().ImplicitWait = ts;
                else
                    throw new ArgumentOutOfRangeException("Тайм-аут меньше или равен нулю.");
            }
            catch (OpenQA.Selenium.DriverServiceNotFoundException)
            {
                throw new OpenQA.Selenium.DriverServiceNotFoundException();
            }
        }

        public void GoToUrl(string link, bool IsFullScreen)
        {
            if (link == null) throw new ArgumentNullException();
            Uri result_link;
            var is_link = Uri.TryCreate(link, UriKind.Absolute, out result_link);
            if (is_link)
            {
                driver.Navigate().GoToUrl(result_link);
                if (IsFullScreen)
                    driver.Manage().Window.FullScreen();
            }
            else throw new UriFormatException();
        }

        //Устанавливаем статус, отображать диагностическу информацию или нет
        public void SetStatusDiagnostic(DriverService service, bool HideCommandPromptWindow)
        {
            service.SuppressInitialDiagnosticInformation = !HideCommandPromptWindow;
            service.HideCommandPromptWindow = HideCommandPromptWindow;
        }

        //page objects
        [FindsBy(How = How.XPath, Using = "//button[.='Все отделы']")]
        [CacheLookup]
        private IWebElement Button_All_Department;
        [FindsBy(How = How.XPath, Using = "//button[.='Все языки']")]
        [CacheLookup]
        private IWebElement Button_All_Languages;

        //Находим количество вакансий
        public List<Vacancies> FindCountVacancies(TypeLanguage language, string department)
        {
            CloseCookieAttention();
            try
            {
                Button_All_Department?.TryClick();

                IWebElement button_department;
                if (String.IsNullOrEmpty(department))
                {
                    button_department = SelectDepartment();
                }
                else
                {
                    button_department = driver.FindElement(By.XPath("//a[.='" + department + "']"));
                }

                button_department.TryClick();

                Button_All_Languages?.TryClick();

                string str_language = "";
                switch (language)
                {
                    case (TypeLanguage.Russian):
                        {
                            str_language = "lang-option-0";
                            break;
                        }
                    case (TypeLanguage.English):
                        {
                            str_language = "lang-option-1";
                            break;
                        }
                }
                var button_language = driver.FindElement(By.XPath("//input[@id='" + str_language + "']"));
                button_language.TryClick();

                var links = driver.FindElements(By.XPath("//a[@class='card card-no-hover card-sm']"));
                var result = new List<Vacancies>();
                foreach (var s in links)
                {
                    result.Add(
                        new Vacancies()
                        {
                            header = s.FindElement(By.XPath(".//div[starts-with(@class,'card-header')]"))?.Text,
                            body = s.FindElement(By.XPath(".//div[starts-with(@class,'card-body')]"))?.Text,
                            footer = s.FindElement(By.XPath(".//div[starts-with(@class,'card-footer')]"))?.Text
                        });
                }
                return result;
            }
            catch (OpenQA.Selenium.NoSuchElementException)
            {
                throw new Exception("Искомый элемент не найден.");
            }
        }
        //закрываем окно с предложением принять cookie
        public void CloseCookieAttention()
        {
            var cookie_div = driver.FindElement(By.Id("cookiescript_close"));
            if (cookie_div == null) throw new OpenQA.Selenium.NoSuchElementException();
            cookie_div.TryClick();
        }

        //выбираем отдел если изначально он не был указан
        public IWebElement SelectDepartment()
        {
            try
            {
                var departments = driver.FindElements(By.XPath("//a[@class='dropdown-item']"));
                Dictionary<int, string> dictionary_of_department = new Dictionary<int, string>();
                int counter = 1;
                Console.WriteLine("Выберите интересующий отдел:");
                foreach (var s in departments)
                {
                    dictionary_of_department.Add(counter, s.Text);
                    Console.WriteLine("{0}:{1}", counter, s.Text);
                    counter++;
                }
                int counter_cases = 0;
                var selected = Console.ReadLine();
                while (true)
                {
                    if (Int32.TryParse(selected, out counter_cases) == true)
                    {
                        if (counter_cases >= dictionary_of_department.Count() || counter_cases < 1)
                        {
                            Console.WriteLine("Неверный номер, выберите другой");
                        }
                        else break;
                    }
                    else
                        Console.WriteLine("Введенное значение не является числом");
                    selected = Console.ReadLine();
                }
                return driver.FindElement(By.XPath("//a[.='" + dictionary_of_department[counter_cases] + "']"));
            }
            catch (OpenQA.Selenium.NoSuchElementException) { throw new Exception("Искомый элемент не найден."); }
        }

        //выводим количество вакансий
        public int? ResultVacancies(List<Vacancies> list)
        {
            if (list == null) throw new ArgumentNullException();
            int i = 1;
            foreach (var s in list)
            {
                Console.WriteLine("{3}:{0}\n{1}\n{2}", s.header, s.body, s.footer, i);
                i++;
            }
            return list?.Count();
        }
    }
}
