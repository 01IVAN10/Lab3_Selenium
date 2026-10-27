using Lab3.SeleniumTests.Infrastructure;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Lab3.SeleniumTests.Tests;

/// <summary>
/// Тест 6. Сторінка File Download (/download).
/// Список файлів на сайті формують інші користувачі (через File Upload),
/// тому тест бере стабільний файл some-file.txt, а якщо його немає — перший у списку.
/// </summary>
[TestFixture]
public class Test06_FileDownload : BaseTest
{
    private const string PreferredFile = "some-file.txt";

    [Test]
    [Description("Сторінка містить список файлів, а натискання на посилання завантажує файл на диск")]
    public void ClickFileLink_DownloadsFile()
    {
        Open("/download");

        // 1. На сторінці є посилання на файли
        var links = Driver.FindElements(By.CssSelector("#content .example a"));
        Assert.That(links, Is.Not.Empty, "Список файлів для завантаження порожній");
        Assert.That(links.All(l => (l.GetDomAttribute("href") ?? "").StartsWith("download/")), Is.True,
            "Усі посилання ведуть на /download/<файл>");

        var link = links.FirstOrDefault(l => l.Text == PreferredFile) ?? links[0];
        var fileName = link.Text.Trim();
        TestContext.Out.WriteLine($"Файлів у списку: {links.Count}. Завантажуємо: {fileName}");
        TakeScreenshot("file-list");

        // 2. Натискаємо на посилання і чекаємо, поки файл повністю збережеться
        link.Click();
        var expectedPath = Path.Combine(DownloadDirectory, fileName);

        var downloadWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
        downloadWait.Until(_ =>
            File.Exists(expectedPath) &&
            !Directory.EnumerateFiles(DownloadDirectory, "*.crdownload").Any());

        // 3. Файл існує і не порожній
        var info = new FileInfo(expectedPath);
        TestContext.Out.WriteLine($"Збережено: {info.FullName} ({info.Length} байт)");
        Assert.That(info.Length, Is.GreaterThan(0), "Завантажений файл порожній");
    }
}
