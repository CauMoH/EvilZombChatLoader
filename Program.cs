using System.Text;
using HtmlAgilityPack;

namespace EvilZombChatLoader;

internal static class Program
{
    private const string ChatUrl = "https://evilzomb.myarena.site/hlstats.php?mode=chat&game=cstrike";
    private static readonly HtmlWeb HtmlWeb = new HtmlWeb()
    {
        OverrideEncoding = Encoding.UTF8
    };
    
    static async Task Main()
    {
        using var cts = new CancellationTokenSource();
        Console.WriteLine("Кол-во страницы для скачки..");
        var str = Console.ReadLine();

        if (!int.TryParse(str, out var pagesCount))
        {
            Console.WriteLine("Вы не указали кол-во страниц!");
        }
        else
        {
            Console.WriteLine("Вставьте ссылку на первую страницу чата игрока или оставьте поле пустым для скачки общего чата");
            var chatPage = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(chatPage))
            {
                chatPage = ChatUrl;
            }

            Console.WriteLine(Environment.NewLine);
            LoadChat(pagesCount, chatPage);
        }

        await Task.Delay(-1, cts.Token);
    }

    private static void LoadChat(int pagesCount, string chatFirstPage)
    {
        try
        {
            var path = Directory.GetCurrentDirectory();
            var chatFile = Path.Combine(path, "tmp.txt");

            using var str = new FileStream(chatFile, FileMode.Create);
            using TextWriter writer = new StreamWriter(str);

            for (var i = 1; i <= pagesCount; i++)
            {
                var strPage = string.Empty;
                if (i != 1)
                {
                    strPage = $"&page={i + 1}";
                }

                var doc = HtmlWeb.Load(ChatUrl + strPage).DocumentNode;
                var rows = doc.SelectNodes("//table[@class='data-table']//tr");

                foreach (var row in rows)
                {
                    if (row.GetAttributeValue("class", string.Empty) == "data-table-head")
                        continue;

                    var name = row.ChildNodes[3].ChildNodes[1].InnerText.Trim();
                    var message = row.ChildNodes[5].InnerText.Trim();

                    writer.WriteLine(name + "   " + message);
                }

                Console.WriteLine($"Страница: {i} - записана");
            }

            Console.WriteLine($"{Environment.NewLine}Готово!, файл лежит по пути: {chatFile}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}