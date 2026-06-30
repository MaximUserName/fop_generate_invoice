using HandlebarsDotNet;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace FopInvoice.Poc.Html;

class Program
{
    static async Task Main(string[] args)
    {
        await TryGeneratePdf();
        // await TestPuppeteerFindsBrowser();
    }

    const string ApplicationsGoogleChromeAppContentsMacosGoogleChrome =
        "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";


    static async Task TryGeneratePdf()
    {
        Console.WriteLine("Generate PDF from HTML using headless browser");
        // var invoicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/invoice-to-pdf-sample.html");
        var invoicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/fop-invoice-template.html");

        Console.WriteLine(invoicePath);
        // var html2 = File.ReadAllText(invoicePath);
        // Console.WriteLine(html2);

        var template = await File.ReadAllTextAsync(invoicePath);
        object data = new
        {
            amount_one = 2000,
            amount_two = 5000,
            some_text = " of some text",
            address_ukr = "21037",
            address_en = "21037",
            ipn_fop_tax_number = "1212112121",
        };

        var data1 = new Dictionary<string, object>()
        {
            { "amount_one", 200 },
            { "amount_two", 500 },
            { "some_text", " of some text" },
            { "address_ukr", "21037" },
            { "ipn_fop_tax_number", "1212112121" },
        };

        var invoiceData = InvoiceDataHelper.PrepareVariables();

        var invoiceDataVariables =
            invoiceData.Variables.ToDictionary(key => key.Key.Replace("{", string.Empty).Replace("}", string.Empty),
                val => val.Value);
        var html = Handlebars.Compile(template)(invoiceDataVariables);
        Console.WriteLine(html);
        // var b = await Puppeteer.ConnectAsync(new ConnectOptions()
        // {
        //
        // });
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
        {
            Headless = true,
            ExecutablePath = ApplicationsGoogleChromeAppContentsMacosGoogleChrome,
            Args = new[]
            {
                "--allow-file-access-from-files",
                "--enable-local-file-accesses"
            }
        });
        await using var page = await browser.NewPageAsync();

        // super line, shows image file not found
        page.Console += (sender, e) =>
        {
            TextWriter writer = Console.Error;
            switch (e.Message.Type)
            {
                case ConsoleType.Log:
                    break;
                case ConsoleType.Debug:
                    break;
                case ConsoleType.Info:
                    break;
                case ConsoleType.Error:
                    writer = Console.Error;
                    break;
                case ConsoleType.Warning:
                    writer = Console.Out;
                    break;
                case ConsoleType.Dir:
                    break;
                case ConsoleType.Dirxml:
                    break;
                case ConsoleType.Table:
                    break;
                case ConsoleType.Trace:
                    break;
                case ConsoleType.Clear:
                    break;
                case ConsoleType.StartGroup:
                    break;
                case ConsoleType.StartGroupCollapsed:
                    break;
                case ConsoleType.EndGroup:
                    break;
                case ConsoleType.Assert:
                    break;
                case ConsoleType.Profile:
                    break;
                case ConsoleType.ProfileEnd:
                    break;
                case ConsoleType.Count:
                    break;
                case ConsoleType.TimeEnd:
                    break;
                case ConsoleType.Verbose:
                    break;
                case ConsoleType.Timestamp:
                    break;
                default:
                    writer = Console.Out;
                    break;
            }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            writer.WriteLine(e.Message.Type);
            writer.WriteLine(e.Message.Text);
            if (e.Message.Location is not null)
            {
                writer.WriteLine(FormatConsoleMessageLocation(e.Message.Location));
            }
            if (e.Message.Args is not null)
            {
                writer.WriteLine(string.Join(",", e.Message.Args));
            }
        };

        await page.GoToAsync($"file://{AppDomain.CurrentDomain.BaseDirectory}/");
        // await page.SetContentAsync(template);
        // await page.GoToAsync($"file://{AppContext.BaseDirectory}/");
        await page.SetContentAsync(html, new SetContentOptions()
        {
            WaitUntil = new[] { WaitUntilNavigation.Networkidle0 },
            Timeout = 1000,
        });
        await page.EmulateMediaTypeAsync(MediaType.Print);
        var pdf = await page.PdfDataAsync(new PdfOptions() { Format = PaperFormat.A4, PrintBackground = true, });

        var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../output",
            "invoice-to-pdf-sample.pdf");
        var directory = Path.GetDirectoryName(outputPath)!;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllBytesAsync(outputPath, pdf);
    }

    private static string FormatConsoleMessageLocation(ConsoleMessageLocation location)
    {
        return $"Line {location.LineNumber}:{location.ColumnNumber} \t {location.URL}";
    }

    static async Task TestPuppeteerFindsBrowser()
    {
        Console.WriteLine("Initialize Browser Fetching...");

        // 1. Download Chromium to your Mac if it's not already installed
        // var browserFetcher = new BrowserFetcher();
        // await browserFetcher.DownloadAsync();
        //
        // Console.WriteLine("Launching Browser...");
        // 2. Configure launch options
        // On macOS, running with Headless = true or the new HeadlessMode.Shell is recommended
        var launchOptions = new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox" }, // Often needed to bypass container/sandbox permissions
            // in browser run 'chrome://version'
            ExecutablePath = ApplicationsGoogleChromeAppContentsMacosGoogleChrome,
        };

        // 3. Start the browser and navigate
        await using var browser = await Puppeteer.LaunchAsync(launchOptions);
        await using var page = await browser.NewPageAsync();

        Console.WriteLine("Navigating to example.com...");
        await page.GoToAsync("https://example.com");

        // 4. Save a screenshot to the project directory
        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "screenshot.png");
        await page.ScreenshotAsync(outputPath);

        Console.WriteLine($"Success! Screenshot saved to: {outputPath}");
    }
}
