using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Linq; // Kelime sayımı için

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Tüm logları stderr'e göndermek için yapılandırın
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();

// Metin Manipülasyon Aracımız
[McpServerToolType]
public static class StringUtilityTool
{
    [McpServerTool, Description("Verilen metin üzerinde farklı işlemler yapar (ters çevirme, büyük/küçük harf, kelime/karakter sayımı).")]
    public static string PerformOperation(
        string text, 
        [Description("Yapılacak işlem: 'reverse', 'uppercase', 'lowercase', 'wordcount', 'charcount'")] string operation)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "Hata: İşlenecek metin boş olamaz.";
        }

        switch (operation.ToLowerInvariant())
        {
            case "reverse":
                char[] charArray = text.ToCharArray();
                Array.Reverse(charArray);
                return $"Metnin tersi: {new string(charArray)}";

            case "uppercase":
                return $"Büyük harfe çevrilmiş metin: {text.ToUpperInvariant()}";

            case "lowercase":
                return $"Küçük harfe çevrilmiş metin: {text.ToLowerInvariant()}";

            case "wordcount":
                // Boşluklara göre ayırıp boş stringleri filtreleyerek kelimeleri say
                var wordCount = text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                return $"Metindeki kelime sayısı: {wordCount}";

            case "charcount":
                return $"Metindeki karakter sayısı (boşluklar dahil): {text.Length}";

            default:
                return "Hata: Geçersiz işlem türü. Desteklenen işlemler: 'reverse', 'uppercase', 'lowercase', 'wordcount', 'charcount'.";
        }
    }
}