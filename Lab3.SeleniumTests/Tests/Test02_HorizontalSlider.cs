using Lab3.SeleniumTests.Infrastructure;
using OpenQA.Selenium;

namespace Lab3.SeleniumTests.Tests;

/// <summary>Тест 2. Сторінка Horizontal Slider (/horizontal_slider).</summary>
[TestFixture]
public class Test02_HorizontalSlider : BaseTest
{
    [Test]
    [Description("Повзунок керується клавіатурою, а значення поруч із ним змінюється відповідно")]
    public void ArrowKeys_ChangeSliderValue()
    {
        Open("/horizontal_slider");

        var slider = Driver.FindElement(By.CssSelector(".sliderContainer input[type='range']"));
        var valueLabel = Driver.FindElement(By.Id("range"));

        // 1. Початковий стан і межі повзунка
        using (Assert.EnterMultipleScope())
        {
            Assert.That(valueLabel.Text, Is.EqualTo("0"), "Початкове значення");
            Assert.That(slider.GetDomAttribute("min"), Is.EqualTo("0.0"), "Мінімум");
            Assert.That(slider.GetDomAttribute("max"), Is.EqualTo("5.0"), "Максимум");
            Assert.That(slider.GetDomAttribute("step"), Is.EqualTo("0.5"), "Крок");
        }

        // 2. Три натискання стрілки вправо: 0 -> 0.5 -> 1.0 -> 1.5
        slider.SendKeys(Keys.ArrowRight + Keys.ArrowRight + Keys.ArrowRight);
        Wait.Until(_ => valueLabel.Text == "1.5");
        Assert.That(slider.GetDomProperty("value"), Is.EqualTo("1.5"), "Значення повзунка після 3 кроків вправо");
        TakeScreenshot("value-1.5");

        // 3. Стрілка вліво зменшує значення на один крок
        slider.SendKeys(Keys.ArrowLeft);
        Wait.Until(_ => valueLabel.Text == "1");
        Assert.That(valueLabel.Text, Is.EqualTo("1"), "Значення після кроку вліво");

        // 4. End — максимум, Home — мінімум
        slider.SendKeys(Keys.End);
        Wait.Until(_ => valueLabel.Text == "5");
        Assert.That(valueLabel.Text, Is.EqualTo("5"), "Максимальне значення (End)");
        TakeScreenshot("value-max");

        slider.SendKeys(Keys.Home);
        Wait.Until(_ => valueLabel.Text == "0");
        Assert.That(valueLabel.Text, Is.EqualTo("0"), "Мінімальне значення (Home)");
    }
}
