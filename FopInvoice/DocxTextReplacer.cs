using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using GrapeCity.Documents.Word;
using GrapeCity.Documents.Word.Layout;
using Body = DocumentFormat.OpenXml.Wordprocessing.Body;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace FopInvoice;

public static class DocxTextReplacer
{
    public static void SearchAndReplace(string documentPath, string searchText, string replaceText)
    {
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(documentPath, true))
        {
            string docText = null;
            using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
            {
                docText = sr.ReadToEnd();
            }

            Regex regexText = new Regex(searchText);
            docText = regexText.Replace(docText, replaceText);

            using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
            {
                sw.Write(docText);
            }
        }
    }

    private static string NumberToWordInEnglish(int number)
    {
        return NumToWord.Num2Word.ToWord(number);
    }

    public static string FormatInvoicePriceText(this string priceText)
    {
        var s = priceText.ToLowerInvariant();
        var arr = s.ToCharArray();
        arr[0] = char.ToUpper(arr[0]);
        var res = new string(arr);
        return $"{res}";
    }


    public static void SearchAndReplaceVars(string documentPath, string newDocPath)
    {
        var invoiceNumber = 120;
        var price = 500;

        var dateOfMoneyArrived = "15.09.2025";

        var dateOfMoneyArrivedAdDateTime = DateTime.ParseExact(dateOfMoneyArrived, "dd.MM.yyyy", null);

        // Invoice date should be earlier (2 days) than date of money arrived
        var dateOfInvoice = dateOfMoneyArrivedAdDateTime.AddDays(-2);
        var dateOfInvoiceAsString =  dateOfInvoice.ToString("dd.MM.yyyy");

        // Date pay no later than should be 30 days future (approx.)
        var datePayNoLater = dateOfMoneyArrivedAdDateTime.AddDays(30);
        var datePayNoLaterAsString = datePayNoLater.ToString("dd.MM.yyyy");

        // generate ukrainian text
        var invoicePriceTextUkr = price.ToTextInUkrainian().FormatInvoicePriceText();
        var invoicePriceTextEnglish = NumberToWordInEnglish(price).FormatInvoicePriceText();

        var newInvoiceFileName = $"invoice_{dateOfInvoiceAsString.Replace(".", "_")}.docx";

        var newInvoicePath = Path.Combine(Path.GetDirectoryName(documentPath), newInvoiceFileName);

        File.Copy(documentPath, newInvoicePath, true);

        //return;

        var vars = new Dictionary<string, string>()
        {
            { "invoice_date", dateOfInvoiceAsString },
            { "invoice_number", invoiceNumber.ToString() },
            { "invoice_price", $"{price}.00" },
            { "invoice_price_english_text", $"{invoicePriceTextEnglish} United States dollars." },
            { "invoice_price_ukr", $"{invoicePriceTextUkr}  доларів США." },
            { "invoice_date_pay_not_later", datePayNoLaterAsString },
        }.ToDictionary(k => "{" + k.Key + "}", v => v.Value);

        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(newInvoicePath, true))
        {
            Body body = wordDoc.MainDocumentPart!.Document.Body!;

            foreach (Table table in body.Elements<Table>())
            {
                foreach (TableRow row in table.Elements<TableRow>())
                {
                    foreach (TableCell cell in row.Elements<TableCell>())
                    {
                        ReplaceTextWithVariablesInEachParagraph(vars, cell.Elements<Paragraph>());
                    }
                }
            }

            ReplaceTextWithVariablesInEachParagraph(vars, body.Elements<Paragraph>());
            wordDoc.MainDocumentPart.Document.Save();
        }

        // throws error. Input string '-35.0' was not in correct format
        // ConvertDocxToPdf(newInvoicePath, Path.ChangeExtension(newInvoicePath, ".pdf"));

        Console.WriteLine("Номер контракту:");
        Console.WriteLine($"Invoice (offer) / Інвойс (оферта)  № 1/{invoiceNumber}");
        Console.WriteLine("------");

        var nadayemoText =
            $"Надаємо Контракт на загальну суму {price}$ як оплату за розробку програмного забезпечення. Згідно Invoice (offer) / Інвойс (оферта) №1/{invoiceNumber} від {dateOfInvoiceAsString}р.";
        Console.WriteLine(nadayemoText);
    }

    private static void ConvertDocxToPdf(string sourceFilePath, string destFilePath)
    {
        // Initalize Word instance
        var wordDoc = new GcWordDocument();

        // Load DOCX file
        // System.FormatException: The input string '-35.0' was not in a correct format.
        // at System.Number.ThrowFormatException[TChar](ReadOnlySpan`1 value)
        wordDoc.Load(sourceFilePath);

        // Create an instance of the GcWordLayout
        using (var layout = new GrapeCity.Documents.Word.Layout.GcWordLayout(wordDoc))
        {
            // Define the PDF output settings
            PdfOutputSettings pdfOutputSettings = new PdfOutputSettings();
            pdfOutputSettings.CompressionLevel = CompressionLevel.Fastest;
            pdfOutputSettings.ConformanceLevel = GrapeCity.Documents.Pdf.PdfAConformanceLevel.PdfA1a;

            // Save the Word layout as a PDF
            layout.SaveAsPdf(destFilePath, null, pdfOutputSettings);
        }
    }

    private static void ReplaceTextWithVariablesInEachParagraph(IDictionary<string, string> vars,
        IEnumerable<Paragraph> paragraphs)
    {
        foreach (Paragraph p in paragraphs)
        {
            foreach (Run r in p.Elements<Run>())
            {
                foreach (Text t in r.Elements<Text>())
                {
                    // Console.WriteLine(t.Text);
                    foreach (var variable in vars)
                    {
                        if (t.Text.Contains(variable.Key))
                        {
                            t.Text = t.Text.Replace(variable.Key, variable.Value);
                        }
                    }
                }
            }
        }
    }
}
