// See https://aka.ms/new-console-template for more information

static async Task GenerateInvoice()
{
    Console.WriteLine("Generating invoice");
    await Task.CompletedTask;

    var pathToPdf = Path.Combine("../../../", "invoice_2025_04_25.pdf");
    Console.WriteLine(Path.GetFullPath(pathToPdf));

    var file = await File.ReadAllBytesAsync(pathToPdf);
    Console.WriteLine(file.Length);
}

await GenerateInvoice();
