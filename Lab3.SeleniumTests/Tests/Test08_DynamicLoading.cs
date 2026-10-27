using Lab3.SeleniumTests.Infrastructure;
using OpenQA.Selenium;

namespace Lab3.SeleniumTests.Tests;

/// <summary>
/// Тест 8. Сторінка Dynamic Loading, Example 1 (/dynamic_loading/1).
/// Елемент "Hello World!" вже є на сторінці, але прихований; після натискання Start
/// приблизно 5 секунд показується індикатор завантаження, а потім елемент стає видимим.
/// </summary>
[TestFixture]
public class Test08_DynamicLoading : BaseTest
{
    [Test]
    [Description("Прихований елемент стає видимим після асинхронного завантаження (явне очікування)")]
    public void ClickStart_ShowsHiddenElementAfterLoading()
    {
        Open("/dynamic_loading/1");

        var startButton = Driver.FindElement(By.CssSelector("#start button"));
        var finish = Driver.FindElement(By.Id("finish"));

        // 1. Початковий стан: кнопка видима, результат є в DOM, але прихований
        using (Assert.EnterMultipleScope())
        {
            Assert.That(startButton.Displayed, Is.True, "Кнопка Start видима");
            Assert.That(startButton.Text, Is.EqualTo("Start"));
            Assert.That(finish.Displayed, Is.False, "Результат спочатку прихований");
        }

        // 2. Натискаємо Start — з'являється індикатор завантаження
        startButton.Click();
        var loading = WaitVisible(By.Id("loading"));
        Assert.That(loading.Text, Does.Contain("Loading"), "Індикатор завантаження");
        TakeScreenshot("loading");

        // 3. Явно чекаємо (до 15 с), поки результат стане видимим
        WaitVisible(By.Id("finish"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(finish.Text, Is.EqualTo("Hello World!"), "Текст результату");
            Assert.That(Driver.FindElement(By.Id("loading")).Displayed, Is.False, "Індикатор зник");
            Assert.That(Driver.FindElement(By.Id("start")).Displayed, Is.False, "Кнопка Start прихована");
        }
    }
}
