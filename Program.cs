using System.Diagnostics;
using System.Text;
using Whisper.net;

namespace MediaFileTranscriber;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // Папка, откуда запущен .exe (bin\Debug\net8.0\)
        string baseDir = AppContext.BaseDirectory;
        string ffmpeg = Path.Combine(baseDir, "ffmpeg.exe");
        string model = Path.Combine(baseDir, "ggml-small.bin");
        string outputDir = Path.Combine(baseDir, "output");

        Console.OutputEncoding = Encoding.UTF8;
        Directory.CreateDirectory(outputDir);

        if (!File.Exists(ffmpeg)) { Console.WriteLine("Нет ffmpeg.exe рядом с программой"); Pause(); return; }
        if (!File.Exists(model)) { Console.WriteLine("Нет ggml-base.bin рядом с программой"); Pause(); return; }

        // Меню
        Console.WriteLine("=== Аудио/Видео → Текст ===\n");
        Console.WriteLine("1 - MP3");
        Console.WriteLine("2 - MP4");
        Console.Write("Выбор: ");

        string? ext = Console.ReadLine()?.Trim() switch
        {
            "1" => "mp3",
            "2" => "mp4",
            _ => null
        };
        if (ext == null) return;

        // Выбор файлов через проводник
        var files = SelectFiles(ext);
        if (files.Length == 0) { Console.WriteLine("Файлы не выбраны."); Pause(); return; }

        // Загрузка модели 
        Console.WriteLine("\nЗагрузка модели Whisper...");
        using var factory = WhisperFactory.FromPath(model);

        // Обработка каждого файла
        foreach (var file in files)
        {
            await ProcessFileAsync(factory, file, ffmpeg, outputDir);
        }

        Console.WriteLine($"\nВсё готово. Результаты: {outputDir}");
        Pause();
    }

    
    /// Открывает системный проводник с фильтром по расширению.
    
    private static string[] SelectFiles(string ext)
    {
        string ps = $@"Add-Type -AssemblyName System.Windows.Forms
$d = New-Object System.Windows.Forms.OpenFileDialog
$d.Filter = '{ext.ToUpper()} (*.{ext})|*.{ext}'
$d.Multiselect = $true
if ($d.ShowDialog() -eq 'OK') {{ $d.FileNames | ForEach-Object {{ Write-Output $_ }} }}";

        var psi = new ProcessStartInfo("powershell.exe",
            "-NoProfile -STA -Command \"" + ps.Replace("\"", "\\\"") + "\"")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proc = Process.Start(psi)!;
        string stdout = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();

        return stdout
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(File.Exists)
            .ToArray();
    }

    
    /// Конвертация ffmpeg → WAV → транскрипция → сохранение .txt
    
    private static async Task ProcessFileAsync(
        WhisperFactory factory, string inputFile, string ffmpeg, string outputDir)
    {
        Console.WriteLine($"\n--- {Path.GetFileName(inputFile)} ---");
        string wav = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");

        try
        {
            // 1. Конвертация в WAV 16 kHz mono
            Console.WriteLine("Конвертация через ffmpeg...");
            var conv = Process.Start(new ProcessStartInfo(ffmpeg,
                $"-hide_banner -loglevel error -y -i \"{inputFile}\" " +
                $"-vn -ac 1 -ar 16000 -c:a pcm_s16le \"{wav}\"")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            })!;
            conv.WaitForExit();

            if (!File.Exists(wav))
            {
                Console.WriteLine("Ошибка конвертации.");
                return;
            }

            // 2 Транскрипция
            Console.WriteLine("Расшифровка...");
            using var processor = factory.CreateBuilder()
                .WithLanguage("ru")
                .WithThreads(Environment.ProcessorCount)   // использовать все ядра
                .WithTemperature(0.0f)                     // меньше «фантазий»
                .WithNoContext()                           // не тащить контекст между кусками
                .Build();

            var sb = new StringBuilder();
            using var stream = File.OpenRead(wav);

            await foreach (var seg in processor.ProcessAsync(stream))
                sb.Append(seg.Text);

            // 3. Сохранение
            string outTxt = Path.Combine(outputDir,
                Path.GetFileNameWithoutExtension(inputFile) + ".txt");

            await File.WriteAllTextAsync(outTxt, sb.ToString().Trim(), Encoding.UTF8);

            Console.WriteLine($"Сохранено: {outTxt}");
        }
        finally
        {
            try { if (File.Exists(wav)) File.Delete(wav); } catch { }
        }
    }

    private static void Pause()
    {
        Console.Write("\nEnter для выхода...");
        Console.ReadLine();
    }
}