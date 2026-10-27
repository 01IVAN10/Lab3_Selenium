using Lab3.SeleniumTests.Infrastructure;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Lab3.SeleniumTests.Tests;

/// <summary>Тест 5. Сторінка Entry Ad (/entry_ad) — рекламне модальне вікно.</summary>
[TestFixture]
public class Test05_EntryAd : BaseTest
{
    private static readonly By Modal = By.Id("modal");

    [Test]
    [Description("Модальне вікно з'являється при завантаженні, закривається кнопкою Close і не з'являється повторно")]
    public void EntryAd_AppearsAndCanBeClosed()
    {
        Open("/entry_ad");

        // 1. Вікно з'являється із затримкою — чекаємо на нього явно
        var modal = WaitVisible(Modal);
        TakeScreenshot("modal-shown");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(modal.FindElement(By.CssSelector(".modal-title h3")).Text,
                Is.EqualTo("THIS IS A MODAL WINDOW"), "Заголовок вікна");
            Assert.That(modal.FindElement(By.CssSelector(".modal-body p")).Text,
                Does.StartWith("It's commonly used to encourage a user to take an action"), "Текст вікна");
            Assert.That(Driver.FindElement(By.Id("page-footer")).Displayed, Is.False,
                "Поки відкрите вікно, футер сторінки прихований");
        }

        // 2. Закриваємо вікно кнопкою Close
        var closeButton = modal.FindElement(By.CssSelector(".modal-footer p"));
        Assert.That(closeButton.Text, Is.EqualTo("Close"));
        closeButton.Click();

        Assert.That(WaitInvisible(Modal), Is.True, "Вікно закрилося");
        Assert.That(Driver.FindElement(By.Id("page-footer")).Displayed, Is.True, "Футер знову видимий");

        // Сторінка повідомляє серверу, що вікно закрите (AJAX) — дочекаємося завершення запиту
        Wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return window.jQuery.active === 0;") is true);

        // 3. Після перезавантаження вікно не має з'явитися знову
        Driver.Navigate().Refresh();
        var shortWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(3));
        Action waitForModal = () => shortWait.Until(d => d.FindElement(Modal).Displayed);
        Assert.Throws<WebDriverTimeoutException>(waitForModal,
            "Після закриття вікно не повинно з'являтися повторно");
        TakeScreenshot("modal-not-shown-after-reload");
    }
}
