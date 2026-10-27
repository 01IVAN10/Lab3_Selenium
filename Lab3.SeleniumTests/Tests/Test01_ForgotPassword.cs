using Lab3.SeleniumTests.Infrastructure;
using OpenQA.Selenium;

namespace Lab3.SeleniumTests.Tests;

/// <summary>Тест 1. Сторінка Forgot Password (/forgot_password).</summary>
[TestFixture]
public class Test01_ForgotPassword : BaseTest
{
    private const string Email = "lab3.tester@example.com";

    [Test]
    [Description("Форма відновлення пароля: наявність елементів, введення email, відправка і перехід на сторінку підтвердження")]
    public void SubmitEmail_ShowsEmailSentConfirmation()
    {
        Open("/forgot_password");

        // 1. Перевіряємо наявність і стан елементів форми
        var heading = Driver.FindElement(By.CssSelector("#content h2"));
        var emailInput = Driver.FindElement(By.Id("email"));
        var submitButton = Driver.FindElement(By.Id("form_submit"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(heading.Text, Is.EqualTo("Forgot Password"), "Заголовок сторінки");
            Assert.That(emailInput.Displayed && emailInput.Enabled, Is.True, "Поле E-mail видиме й активне");
            Assert.That(emailInput.GetDomProperty("value"), Is.Empty, "Поле E-mail спочатку порожнє");
            Assert.That(submitButton.Text, Is.EqualTo("Retrieve password"), "Текст кнопки");
        }

        // 2. Вводимо email і перевіряємо, що значення потрапило в поле
        emailInput.SendKeys(Email);
        Assert.That(emailInput.GetDomProperty("value"), Is.EqualTo(Email), "Значення поля після введення");
        TakeScreenshot("email-entered");

        // 3. Відправляємо форму і чекаємо переходу на сторінку підтвердження
        submitButton.Click();
        Wait.Until(d => d.Url.EndsWith("/email_sent"));

        var content = Driver.FindElement(By.Id("content"));
        Assert.That(content.Text, Is.EqualTo("Your e-mail's been sent!"), "Повідомлення про відправку листа");
    }
}
