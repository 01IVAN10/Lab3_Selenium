using Lab3.SeleniumTests.Infrastructure;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Lab3.SeleniumTests.Tests;

/// <summary>Тест 3. Сторінка Dropdown (/dropdown).</summary>
[TestFixture]
public class Test03_Dropdown : BaseTest
{
    [Test]
    [Description("Випадаючий список: склад опцій, стан за замовчуванням, вибір Option 1 і Option 2")]
    public void SelectOptions_ChangesSelectedValue()
    {
        Open("/dropdown");

        var dropdown = new SelectElement(Driver.FindElement(By.Id("dropdown")));
        var optionTexts = dropdown.Options.Select(o => o.Text).ToList();

        // 1. Склад списку і стан за замовчуванням
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dropdown.IsMultiple, Is.False, "Список з одиночним вибором");
            Assert.That(optionTexts, Is.EqualTo(new[] { "Please select an option", "Option 1", "Option 2" }), "Перелік опцій");
            Assert.That(dropdown.SelectedOption.Text, Is.EqualTo("Please select an option"), "Вибрано за замовчуванням");
            Assert.That(dropdown.Options[0].Enabled, Is.False, "Опція-підказка неактивна");
        }

        // 2. Неактивну опцію-підказку вибрати не можна
        Action selectDisabled = () => dropdown.SelectByText("Please select an option");
        Assert.Throws<InvalidOperationException>(selectDisabled, "Selenium не дозволяє вибрати неактивну опцію");

        // 3. Вибір за текстом
        dropdown.SelectByText("Option 1");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dropdown.SelectedOption.Text, Is.EqualTo("Option 1"));
            Assert.That(dropdown.SelectedOption.GetDomProperty("value"), Is.EqualTo("1"));
            Assert.That(dropdown.Options[1].Selected, Is.True, "Option 1 позначена як вибрана");
        }
        TakeScreenshot("option-1");

        // 4. Вибір за значенням (value) — вибір змінюється
        dropdown.SelectByValue("2");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dropdown.SelectedOption.Text, Is.EqualTo("Option 2"));
            Assert.That(dropdown.Options[1].Selected, Is.False, "Option 1 більше не вибрана");
            Assert.That(dropdown.AllSelectedOptions, Has.Count.EqualTo(1), "Вибрана рівно одна опція");
        }
    }
}
