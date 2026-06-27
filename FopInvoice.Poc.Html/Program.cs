namespace FopInvoice.Poc.Html;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var invoicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/invoice-to-pdf.html");

        Console.WriteLine(invoicePath);
        var html = File.ReadAllText(invoicePath);
        Console.WriteLine(html);
    }
}
