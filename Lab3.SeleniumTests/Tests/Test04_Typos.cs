using Lab3.SeleniumTests.Infrastructure;
using OpenQA.Selenium;

namespace Lab3.SeleniumTests.Tests;

/// <summary>
/// Тест 4. Сторінка Typos (/typos).
/// Сторінка навмисно містить дефект: при кожному завантаженні з імовірністю 50%
/// у реченні з'являється друкарська помилка ("won,t" замість "won't").
/// Тест стабільний: перезавантажує сторінку, доки не зафіксує дефект,
/// і щоразу перевіряє, що текст — один із двох відомих варіантів.
/// </summary>
[TestFixture]
public class Test04_Typos : BaseTest
{
    private const string CorrectText = "Sometimes you'll see a typo, other times you won't.";
    private const string TypoText = "Sometimes you'll see a typo, other times you won,t.";
    private const int MaxReloads = 20; // імовірність не побачити помилку за 20 спроб ≈ 0,0001%

    [Test]
    [Description("Виявлення друкарської помилки, яка випадково з'являється на сторінці")]
    public void ReloadPage_DetectsRandomTypo()
    {
        Open("/typos");
        Assert.That(Driver.FindElement(By.CssSelector("#content h3")).Text, Is.EqualTo("Typos"));

        var seenCorrect = 0;
        var seenTypo = 0;

        for (var attempt = 1; attempt <= MaxReloads && seenTypo == 0; attempt++)
        {
            if (attempt > 1) Driver.Navigate().Refresh();

            // Другий абзац сторінки містить речення, яке перевіряємо
            var text = Driver.FindElements(By.CssSelector("#content .example p"))[1].Text.Trim();

            Assert.That(text, Is.EqualTo(CorrectText).Or.EqualTo(TypoText),
                $"Спроба {attempt}: несподіваний текст абзацу");

            if (text == CorrectText) seenCorrect++;
            else seenTypo++;
        }

        TestContext.Out.WriteLine($"Правильний текст: {seenCorrect} раз(и), з помилкою: {seenTypo} раз(и)");
        Assert.That(seenTypo, Is.GreaterThan(0),
            $"За {MaxReloads} перезавантажень друкарську помилку не виявлено");
        TakeScreenshot("typo-detected");
    }
}
