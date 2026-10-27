using Lab3.SeleniumTests.Infrastructure;
using OpenQA.Selenium;

namespace Lab3.SeleniumTests.Tests;

/// <summary>Тест 10. Сторінка Redirect Link (/redirector).</summary>
[TestFixture]
public class Test10_RedirectLink : BaseTest
{
    [Test]
    [Description("Посилання 'here' перенаправляє користувача на сторінку Status Codes")]
    public void ClickRedirectLink_OpensStatusCodesPage()
    {
        Open("/redirector");

        // 1. Перевіряємо сторінку і посилання
        var link = Driver.FindElement(By.Id("redirect"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Driver.FindElement(By.CssSelector("#content h3")).Text, Is.EqualTo("Redirection"));
            Assert.That(link.Text, Is.EqualTo("here"), "Текст посилання");
            Assert.That(link.GetDomAttribute("href"), Is.EqualTo("redirect"), "Посилання веде на /redirect");
        }
        TakeScreenshot("before-redirect");

        // 2. Натискаємо і чекаємо, поки браузер пройде редірект
        link.Click();
        Wait.Until(d => d.Url.EndsWith("/status_codes"));

        // 3. Перевіряємо кінцеву сторінку
        var codes = Driver.FindElements(By.CssSelector("#content ul li a")).Select(a => a.Text).ToList();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Driver.Url, Is.EqualTo(BaseUrl + "/status_codes"), "Кінцева адреса");
            Assert.That(Driver.FindElement(By.CssSelector("#content h3")).Text, Is.EqualTo("Status Codes"));
            Assert.That(codes, Is.EqualTo(new[] { "200", "301", "404", "500" }), "Перелік кодів відповіді");
        }
    }
}
