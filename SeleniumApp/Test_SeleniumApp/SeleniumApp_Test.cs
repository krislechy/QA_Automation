using NUnit.Framework;
using SeleniumApp;
using System;
using System.Collections.Generic;

namespace Test_SeleniumApp
{
    public class SeleniumApp_Test
    {
        [SetUp]
        public void Setup()
        {

        }
        [Test]
        public void TestInit()
        {
            using (Page page = new Page())
            {
                Assert.Throws<OpenQA.Selenium.DriverServiceNotFoundException>(() =>
                    page.InitBrowser(TypeDriver.IE, true, TimeSpan.FromSeconds(10)));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    page.InitBrowser(TypeDriver.Chrome, true, TimeSpan.FromSeconds(-10)));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    page.InitBrowser(TypeDriver.Chrome, true, TimeSpan.FromSeconds(0)));
                if (page.driver == null)
                    Assert.Fail("Драйвер не инициализирован");
                else Assert.Pass();
            }
        }
        [Test]
        public void TestURL()
        {
            using (Page page = new Page())
            {
                page.InitBrowser(TypeDriver.Chrome, true, TimeSpan.FromSeconds(10));
                Assert.Throws<ArgumentNullException>(() => page.GoToUrl(null, true));
                Assert.Throws<UriFormatException>(() => page.GoToUrl("fhdfghdfgh", true));
            }
        }
        [Test]
        public void TestExtenstions()
        {
            using (Page page = new Page())
            {
                Assert.Throws<ArgumentNullException>(() => Extenstions.TryClick(null));
            }
        }
        [Test]
        public void TestFindCountVacancies()
        {
            using (Page page = new Page())
            {
                page.InitBrowser(TypeDriver.Chrome, true, TimeSpan.FromSeconds(10));
                page.GoToUrl("https://careers.veeam.ru/vacancies", true);
                Assert.Throws<Exception>(() => page.FindCountVacancies(TypeLanguage.English, "dfghdfghd"));
            }
        }
        [Test]
        public void Test_ResultVacancies()
        {
            using (Page page = new Page())
            {
                Assert.Throws<ArgumentNullException>(() => page.ResultVacancies(null));
                var result = page.ResultVacancies(new List<Page.Vacancies> { });
                if (result == null)
                    Assert.Ignore();
            }
        }
        [Test]
        public void Test_ExcpectedValueVacancies()
        {
            using (Page page = new Page())
            {
                page.InitBrowser(TypeDriver.Chrome, true, TimeSpan.FromSeconds(10));
                page.GoToUrl("https://careers.veeam.ru/vacancies", true);
                var list = page.FindCountVacancies(TypeLanguage.Russian, "Разработка продуктов");
                Assert.AreEqual(6, page.ResultVacancies(list));
            }
        }
    }
}