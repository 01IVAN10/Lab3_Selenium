using Lab3.SeleniumTests.Infrastructure;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Lab3.SeleniumTests.Tests;

/// <summary>
/// Тест 9. Сторінка Context Menu (/context_menu).
/// Правий клік по виділеній області викликає JavaScript-алерт.
/// </summary>
[TestFixture]
public class Test09_ContextMenu : BaseTest
{
    [Test]
    [Description("Правий клік по області викликає JS-алерт із потрібним текстом; алерт закривається")]
    public void RightClickHotSpot_ShowsJavaScriptAlert()
    {
        Open("/context_menu");

        var hotSpot = Driver.FindElement(By.Id("hot-spot"));
        Assert.That(hotSpot.Displayed, Is.True, "Область для правого кліку видима");
        TakeScreenshot("before-right-click");

        // 1. Правий клік (контекстне меню) через Actions API
        new Actions(Driver).ContextClick(hotSpot).Perform();

        // 2. Чекаємо появи алерта і перевіряємо його текст
        var alert = Wait.Until(d =>
        {
            try { return d.SwitchTo().Alert(); }
            catch (NoAlertPresentException) { return null; }
        })!;
        Assert.That(alert.Text, Is.EqualTo("You selected a context menu"), "Текст алерта");

        // 3. Закриваємо алерт і перевіряємо, що його більше немає
        alert.Accept();
        Action readAlert = () => _ = Driver.SwitchTo().Alert().Text;
        Assert.Throws<NoAlertPresentException>(readAlert, "Після Accept() алерт має зникнути");
        Assert.That(Driver.FindElement(By.CssSelector("#content h3")).Text, Is.EqualTo("Context Menu"),
            "Після закриття алерта сторінка доступна");
    }
}
