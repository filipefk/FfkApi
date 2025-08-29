using System.Diagnostics;

namespace FfkApi.API.DebugUtil;

public class DebugUtil
{
    public static void AbreSwaggerNoBrowser()
    {
        var swaggerUrl = "https://localhost:7128/swagger/index.html";
        var chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        try
        {
            using var process = new Process();
            process.StartInfo.FileName = chromePath;
            process.StartInfo.Arguments = $"--new-window {swaggerUrl}";
            process.StartInfo.UseShellExecute = false;
            process.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Não foi possível abrir o navegador: {ex.Message}");
        }
    }
}
