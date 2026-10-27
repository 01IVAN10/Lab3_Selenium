using Lab3.SeleniumTests.Infrastructure;
using OpenQA.Selenium;

namespace Lab3.SeleniumTests.Tests;

/// <summary>
/// Тест 7. Сторінка Basic Auth (/basic_auth).
/// Браузерне вікно HTTP-автентифікації не є частиною сторінки, тому Selenium
/// не може з ним взаємодіяти. Логін і пароль передаються прямо в URL:
/// https://admin:admin@the-internet.herokuapp.com/basic_auth
/// </summary>
[TestFixture]
public class Test07_BasicAuth : BaseTest
{
    [Test]
    [Description("Вхід через HTTP Basic Auth з правильними обліковими даними")]
    public void ValidCredentials_ShowCongratulations()
    {
        var url = new UriBuilder(BaseUrl + "/basic_auth")
        {
            UserName = "admin",
            Password = "admin"
        }.Uri;

        Driver.Navigate().GoToUrl(url);

        var heading = WaitVisible(By.CssSelector("#content h3"));
        var message = Driver.FindElement(By.CssSelector("#content .example p"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(heading.Text, Is.EqualTo("Basic Auth"), "Заголовок захищеної сторінки");
            Assert.That(message.Text, Is.EqualTo("Congratulations! You must have the proper credentials."),
                "Повідомлення про успішну автентифікацію");
            Assert.That(Driver.Url, Does.EndWith("/basic_auth"), "Користувач залишився на захищеній сторінці");
        }
    }
}
