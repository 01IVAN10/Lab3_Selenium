using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Lab3.SeleniumTests.Infrastructure;

/// <summary>
/// Базовий клас для всіх тестів: запускає Chrome перед кожним тестом,
/// закриває його після, зберігає скріншоти та надає допоміжні методи.
/// </summary>
public abstract class BaseTest
{
    /// <summary>Адреса сайту. Можна змінити змінною середовища BASE_URL.</summary>
    protected static readonly string BaseUrl =
        (Environment.GetEnvironmentVariable("BASE_URL") ?? "https://the-internet.herokuapp.com").TrimEnd('/');

    protected static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);

    protected IWebDriver Driver = null!;
    protected WebDriverWait Wait = null!;

    /// <summary>Тимчасова папка, куди Chrome зберігає завантажені файли.</summary>
    protected string DownloadDirectory = null!;

    private int _stepNumber;

    [SetUp]
    public void StartBrowser()
    {
        DownloadDirectory = Path.Combine(Path.GetTempPath(), "lab3-downloads-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(DownloadDirectory);

        var options = new ChromeOptions();
        options.AddArgument("--window-size=1366,900");
        options.AddArgument("--lang=en-US");
        options.AddUserProfilePreference("download.default_directory", DownloadDirectory);
        options.AddUserProfilePreference("download.prompt_for_download", false);
        options.AddUserProfilePreference("safebrowsing.enabled", true);

        // HEADLESS=1 — запуск без вікна браузера (наприклад, на CI).
        if (Environment.GetEnvironmentVariable("HEADLESS") == "1")
            options.AddArgument("--headless=new");

        // Selenium Manager сам знаходить Chrome і завантажує відповідний chromedriver.
        Driver = new ChromeDriver(options);

        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero; // лише явні очікування
        Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
        Wait = new WebDriverWait(Driver, DefaultTimeout);
        _stepNumber = 0;
    }

    [TearDown]
    public void StopBrowser()
    {
        try
        {
            var outcome = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed ? "PASSED" : "FAILED";
            TakeScreenshot("result-" + outcome);
        }
        catch (WebDriverException)
        {
            // Скріншот неможливий (наприклад, відкритий алерт) — не валимо тест через це.
        }
        finally
        {
            Driver.Quit();
            Driver.Dispose();
            try { Directory.Delete(DownloadDirectory, recursive: true); } catch (IOException) { }
        }
    }

    /// <summary>Відкриває сторінку за відносним шляхом, наприклад "/dropdown".</summary>
    protected void Open(string relativePath)
    {
        Driver.Navigate().GoToUrl(BaseUrl + relativePath);
    }

    /// <summary>Чекає, доки елемент з'явиться в DOM і стане видимим.</summary>
    protected IWebElement WaitVisible(By locator)
    {
        return Wait.Until(d =>
        {
            try
            {
                var element = d.FindElement(locator);
                return element.Displayed ? element : null;
            }
            catch (NoSuchElementException) { return null; }
            catch (StaleElementReferenceException) { return null; }
        })!;
    }

    /// <summary>Чекає, доки елемент стане невидимим або зникне з DOM.</summary>
    protected bool WaitInvisible(By locator)
    {
        return Wait.Until(d =>
        {
            try { return !d.FindElement(locator).Displayed; }
            catch (NoSuchElementException) { return true; }
            catch (StaleElementReferenceException) { return true; }
        });
    }

    /// <summary>
    /// Зберігає скріншот у папку Screenshots проєкту, щоб їх було зручно вставити у звіт.
    /// Ім'я файлу: {Клас}_{номер кроку}_{назва}.png
    /// </summary>
    protected string TakeScreenshot(string stepName)
    {
        var dir = Path.Combine(FindProjectDirectory(), "Screenshots");
        Directory.CreateDirectory(dir);

        _stepNumber++;
        var file = Path.Combine(dir, $"{GetType().Name}_{_stepNumber:00}_{stepName}.png");
        ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(file);
        TestContext.AddTestAttachment(file);
        return file;
    }

    /// <summary>Шукає папку з .csproj, піднімаючись від bin/Debug/netX.0.</summary>
    private static string FindProjectDirectory()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !dir.EnumerateFiles("*.csproj").Any())
            dir = dir.Parent;
        return dir?.FullName ?? AppContext.BaseDirectory;
    }
}
