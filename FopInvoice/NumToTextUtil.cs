using System.Text;

namespace FopInvoice;

public static class NumToTextUtil
{
    public static void TestOnConsole()
    {
        Console.WriteLine(1590.ToTextInUkrainian());
        Console.WriteLine(3468.ToTextInUkrainian());
        Console.WriteLine(2380.ToTextInUkrainian());
        Console.WriteLine(1317.ToTextInUkrainian());
        Console.WriteLine(2316.ToTextInUkrainian());
        Console.WriteLine(2420.ToTextInUkrainian());
        Console.WriteLine(2430.ToTextInUkrainian());
        Console.WriteLine(2000.ToTextInUkrainian());
        Console.WriteLine(1010.ToTextInUkrainian());
        Console.WriteLine(2222.ToTextInUkrainian());
        Console.WriteLine(300.ToTextInUkrainian());
        Console.WriteLine(250.ToTextInUkrainian());
        Console.WriteLine(72.ToTextInUkrainian());

        Console.WriteLine(970.ToTextInUkrainian());
    }

    public static string ToTextInUkrainian(this int num)
    {
        var digits = new[] { "", "один", "два", "три", "чотири", "пʼять", "шість", "сім", "вісім", "девʼять" };
        var tens = new Dictionary<int, string>()
        {
            { 10, "десять" },
            { 11, "одина́дцять" },
            { 12, "двана́дцять" },
            { 13, "трина́дцять" },
            { 14, "чотирна́дцять" },
            { 15, "п'ятна́дцять" },
            { 16, "шістна́дцять" },
            { 17, "сімна́дцять" },
            { 18, "вісімна́дцять" },
            { 19, "дев'ятна́дцять" }
        };
        var hundreds = new[]
        {
            "", "сто", "двісті", "триста", "чотириста", "п'ятсот", "шістсот", "сімсот",
            "вісімсот", "дев'ятсот"
        };
        var thousands = new[] { "", "одна", "дві", "три", "чотири", "пʼять", "шість", "сім", "вісім", "девʼять" };

        var from1_To_99_array = new string[]
        {
            "",
            "один",
            "два",
            "три",
            "чотири",
            "п'ять",
            "шість",
            "сім",
            "вісім",
            "дев'ять",
            "десять ",
            "одинадцять",
            "дванадцять",
            "тринадцять",
            "чотирнадцять",
            "п'ятнадцять",
            "шістнадцять",
            "сімнадцять",
            "вісімнадцять",
            "дев'ятнадцять",
            "двадцять ",
            "двадцять один",
            "двадцять два",
            "двадцять три",
            "двадцять чотири",
            "двадцять п'ять",
            "двадцять шість",
            "двадцять сім",
            "двадцять вісім",
            "двадцять дев'ять",
            "тридцять ",
            "тридцять один",
            "тридцять два",
            "тридцять три",
            "тридцять чотири",
            "тридцять п'ять",
            "тридцять шість",
            "тридцять сім",
            "тридцять вісім",
            "тридцять дев'ять",
            "сорок ",
            "сорок один",
            "сорок два",
            "сорок три",
            "сорок чотири",
            "сорок п'ять",
            "сорок шість",
            "сорок сім",
            "сорок вісім",
            "сорок дев'ять",
            "п'ятдесят ",
            "п'ятдесят один",
            "п'ятдесят два",
            "п'ятдесят три",
            "п'ятдесят чотири",
            "п'ятдесят п'ять",
            "п'ятдесят шість",
            "п'ятдесят сім",
            "п'ятдесят вісім",
            "п'ятдесят дев'ять",
            "шістдесят ",
            "шістдесят один",
            "шістдесят два",
            "шістдесят три",
            "шістдесят чотири",
            "шістдесят п'ять",
            "шістдесят шість",
            "шістдесят сім",
            "шістдесят вісім",
            "шістдесят дев'ять",
            "сімдесят ",
            "сімдесят один",
            "сімдесят два",
            "сімдесят три",
            "сімдесят чотири",
            "сімдесят п'ять",
            "сімдесят шість",
            "сімдесят сім",
            "сімдесят вісім",
            "сімдесят дев'ять",
            "вісімдесят ",
            "вісімдесят один",
            "вісімдесят два",
            "вісімдесят три",
            "вісімдесят чотири",
            "вісімдесят п'ять",
            "вісімдесят шість",
            "вісімдесят сім",
            "вісімдесят вісім",
            "вісімдесят дев'ять",
            "дев'яносто ",
            "дев'яносто один",
            "дев'яносто два",
            "дев'яносто три",
            "дев'яносто чотири",
            "дев'яносто п'ять",
            "дев'яносто шість",
            "дев'яносто сім",
            "дев'яносто вісім",
            "дев'яносто дев'ять ",
        };

        var numAsString = num.ToString();

        StringBuilder sb = new StringBuilder();

        if (numAsString.Length >= 4)
        {
            var thousandsParsed = int.Parse(numAsString.ElementAtFromEnd(4 - 1));
            if (thousandsParsed > 0)
            {
                var tysyachaEnding = GetUkrTysyachaEnding(thousandsParsed);
                sb.Append($"{thousands[thousandsParsed]} {tysyachaEnding} ");
            }
        }

        if (numAsString.Length >= 3)
        {
            var hundredsParsed = int.Parse(numAsString.ElementAtFromEnd(3 - 1));
            if (hundredsParsed > 0)
                sb.Append($"{hundreds[hundredsParsed]} ");
        }


        var numLast2Digits = numAsString.GetLastTwoCharsAsNumber();

        var text = from1_To_99_array[numLast2Digits];


        sb.Append(text);
        return sb.ToString();
    }

    private static string GetUkrTysyachaEnding(int numberOfThousands)
    {
        switch (numberOfThousands)
        {
            case 1: return "тисяча";
            case 2:
            case 3:
            case 4:
                return "тисячі";
            default:
                return "тисяч";
        }
    }

    public static string ElementAtFromEnd(this IEnumerable<char> text, int index)
    {
        var array = text.ToArray();
        return array[array.Length - index - 1].ToString();
    }

    public static int GetLastTwoCharsAsNumber(this string originalString)
    {
        string lastTwoChars;

        if (originalString.Length >= 2)
        {
            lastTwoChars = originalString.Substring(originalString.Length - 2);
        }
        else
        {
            // Handle cases where the string has fewer than 2 characters
            lastTwoChars = originalString; // Or handle as desired (e.g., throw an exception)
        }

        return int.Parse(lastTwoChars);
    }
}
