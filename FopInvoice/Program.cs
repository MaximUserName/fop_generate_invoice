// See https://aka.ms/new-console-template for more information

using FopInvoice;

static async Task GenerateInvoiceFromDocx()
{
    Console.WriteLine("Generating invoice");
    await Task.CompletedTask;

    var pathToDocx = Path.Combine("../../../", "invoice_template.docx");

    DocxTextReplacer.SearchAndReplaceVars(pathToDocx, "");

    // NumToTextUtil.TestOnConsole();
    // DocxTextReplacer.SearchAndReplace(pathToDocx, "2025", "2026");
}

static async Task GenerateInvoice2()
{
    Console.WriteLine("Generating invoice");
    await Task.CompletedTask;

    var pathToPdf = Path.Combine("../../../", "invoice_2025_04_25.pdf");
    Console.WriteLine(Path.GetFullPath(pathToPdf));

    var file = await File.ReadAllBytesAsync(pathToPdf);
    Console.WriteLine(file.Length);
}

await GenerateInvoiceFromDocx();
