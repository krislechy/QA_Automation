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
            var result = FindCountVacancies(LanguageType.Russian, "Разработка продуктов");
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
        static List<Vacancies> FindCountVacancies(LanguageType language, string department)
        {
            //Ищем по тексту кнопку и вызываем скрипт события click
            ScriptClick(FindButtonByInnerText("Все отделы"));
            //Находим все элементы по тэгу a
            var links_of_departments = chrome.FindElementsByTagName("a");
            //находим среди ссылочных тэгов, тэг с наименованием класса dropdown-item
            var departments = links_of_departments.Where(x => x.GetAttribute("class") == "dropdown-item");
            //объявляем пустой объект для отдел
            IWebElement department_link=null;
            //если входящий параметер department пустой то предлагаем на выбор все отделы которые имеются на сайте
            if (String.IsNullOrEmpty(department))
            {
                //объявляем словарь для отделов
                Dictionary<int, string> dictionary_of_department = new Dictionary<int, string>();
                //объявляем счетчик для словаря
                int counter = 1;
                Console.WriteLine("Выберите интересующий отдел:");
                foreach (var s in departments)
                {
                    //добавляем в словарь новый элемет
                    dictionary_of_department.Add(counter, s.Text);
                    counter++;
                    //выводим номер и названия отделов
                    Console.WriteLine("{0}:{1}", counter, s.Text);
                }
                //объявляем переменную куда присвоим значение выбранного отдела пользователем
                int counter_cases = 0;
                //читаем введенную строку пользователя
                var selected = Console.ReadLine();
                //цикл для проверки введенной строки пользователем, подходит ли под заданное условие
                while (true)
                {
                    //пытаемся распарсить значение, если true то присваиваем результат в переменную counter_cases
                    if (Int32.TryParse(selected, out counter_cases) == true)
                    {
                        //проверка введеных значений если меньше количество отделов и больше 1 включая, в противном случае выводим сообщение
                        if (counter_cases >= dictionary_of_department.Count() || counter_cases < 1)
                        {
                            Console.WriteLine("Неверный номер, выберите другой");
                        }
                        // если проверка прошла успешно то прерываем цикл
                        else break;
                    }
                    else
                        Console.WriteLine("Введенное значение не является числом");
                    //если проверки не пройдены то повторяем заново цикл
                    selected = Console.ReadLine();
                }
                //ищем ссылку на отдел выбранного пользователем и присваиваем переменной department_link
                department_link = chrome.FindElementByLinkText(dictionary_of_department[counter_cases]);
            }
            else
            {
                //ищем ссылку на отдел и присваиваем переменной department_link
                department_link = chrome.FindElementByLinkText(department);
            }
            //вызываем событие click
            ScriptClick(department_link);
            //находим все элементы по тэгу button
            var buttons_in_dropdown = chrome.FindElementsByTagName("button");
            //проверяем на наличие
            if (buttons_in_dropdown != null && buttons_in_dropdown.Count() > 0)
            {
                //находим кнопку с текстом "все языки"
                var button_language = buttons_in_dropdown.Where(x => x.Text == "Все языки").First();
                //вызываем событие click
                ScriptClick(button_language);
            }
            //находим элементы с тэгом div где аттрибут класс равняется show dropdown
            var input_language = chrome.FindElementsByTagName("div").Where(x => x.GetAttribute("class") == "show dropdown").First();
            string str_language = "";
            //выбираем интересующий язык
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
            //находим чек боксы с id равной lang-option-#
            var checkboxs = input_language.FindElements(By.TagName("input")).Where(x => x.GetAttribute("id") == str_language);
            if (checkboxs != null && checkboxs.Count() > 0)
            {
                var input_checkbox = checkboxs.First();
                //вызываем событие click
                ScriptClick(input_checkbox);
            }
            //находим все ссылки с тэгом а и именем аттрибута class card card-no-hover card-sm
            var links = chrome.FindElementsByTagName("a").Where(x => x.GetAttribute("class") == "card card-no-hover card-sm");
            var result = new List<Vacancies>();
            //ищем среди списка
            foreach (var s in links)
            {
                string header = "", body = "", footer = "";
                //находим елементы с тэгом div внутри списка
                var div = s.FindElements(By.TagName("div"));
                foreach (var t in div)
                {
                    //ищем по названию аттрибута class
                    var classname = t.GetAttribute("class");
                    //находим div с названием аттрибута class card-header
                    if (classname.StartsWith("card-header"))
                    {
                        header = t.Text;
                    }
                    //находим div с названием аттрибута class card-body
                    else if (classname.StartsWith("card-body"))
                    {
                        body = t.Text;
                    }
                    //находим div с названием аттрибута class card-footer
                    else if (classname.StartsWith("card-footer"))
                    {
                        footer = t.Text;
                    }
                }
                //результат добавляем в список
                result.Add(new Vacancies() { header = header, body = body, footer = footer });
            }
            //возвращаем список
            return result;
        }
        static void ScriptClick(IWebElement element)
        {
            //если элемент страницы пустой то возвращаем
            if (element == null)
                return;
            //наследуем интерфейс ijavascriptexecuter что бы можно было выполнить скрипт на странице и присваиваем его переменной executor
            if (executor == null)
                executor = chrome as IJavaScriptExecutor;
            //выполняем скрипт на странице
            executor.ExecuteScript("arguments[0].click();", element);
        }
        //Метод нахождения кнопок по тексту
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
