# Лабораторна робота №3 — Автоматизація тестування вебзастосунків із Selenium WebDriver

**Дисципліна:** Компоненти програмної інженерії. Частина 4. Якість та тестування програмного забезпечення
**Виконав:** Білошицький Іван, група КП-43 (КПІ ім. Ігоря Сікорського, ФПМ)
**Варіант:** 2
**Сайт, що тестується:** https://the-internet.herokuapp.com/

## Інструменти

- C# / .NET 9
- Selenium WebDriver 4.51 (+ Selenium.Support)
- NUnit 4
- Google Chrome (драйвер підбирає Selenium Manager автоматично)

## Тести

| № | Сторінка | Тест | Що перевіряється |
|---|----------|------|------------------|
| 1 | Forgot Password | `Test01_ForgotPassword.SubmitEmail_ShowsEmailSentConfirmation` | наявність і стан полів форми, введення email, відправка, повідомлення «Your e-mail's been sent!» |
| 2 | Horizontal Slider | `Test02_HorizontalSlider.ArrowKeys_ChangeSliderValue` | межі повзунка, зміна значення стрілками, клавіші Home/End |
| 3 | Dropdown | `Test03_Dropdown.SelectOptions_ChangesSelectedValue` | склад списку, неактивна опція за замовчуванням, вибір за текстом і за value |
| 4 | Typos | `Test04_Typos.ReloadPage_DetectsRandomTypo` | виявлення випадкової друкарської помилки («won,t») при перезавантаженнях |
| 5 | Entry Ad | `Test05_EntryAd.EntryAd_AppearsAndCanBeClosed` | поява модального вікна, його текст, закриття, відсутність після перезавантаження |
| 6 | File Download | `Test06_FileDownload.ClickFileLink_DownloadsFile` | список файлів, завантаження файлу на диск, файл не порожній |
| 7 | Basic Auth | `Test07_BasicAuth.ValidCredentials_ShowCongratulations` | HTTP Basic-автентифікація (admin/admin) через URL |
| 8 | Dynamic Loading | `Test08_DynamicLoading.ClickStart_ShowsHiddenElementAfterLoading` | прихований елемент, індикатор завантаження, явне очікування появи «Hello World!» |
| 9 | Context Menu | `Test09_ContextMenu.RightClickHotSpot_ShowsJavaScriptAlert` | правий клік (Actions API), JavaScript-алерт і його текст, закриття алерта |
| 10 | Redirect Link | `Test10_RedirectLink.ClickRedirectLink_OpensStatusCodesPage` | редірект на сторінку Status Codes, кінцева URL-адреса і вміст сторінки |

## Структура

```
Lab3.SeleniumTests/
├── Infrastructure/BaseTest.cs   # запуск/закриття Chrome, очікування, скріншоти
├── Tests/Test01..Test10_*.cs     # 10 тестів, по одному на сторінку
└── Screenshots/                  # з'являється після запуску: скріншоти кроків і результатів
```

## Запуск

Потрібні .NET 9 SDK і Google Chrome.

```bash
cd Lab3_Selenium
dotnet test --logger "console;verbosity=normal"
```

Без вікна браузера (headless):

```bash
HEADLESS=1 dotnet test --logger "console;verbosity=normal"
```

Також тести можна запускати з панелі **Testing** у VS Code (розширення C# Dev Kit), з Test Explorer у Visual Studio або з Rider.

Після запуску скріншоти кожного тесту зберігаються в `Lab3.SeleniumTests/Screenshots/`.
