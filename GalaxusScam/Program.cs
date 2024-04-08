using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Settings
{
    public int LastProduct { get; set; }
    public int LastPage { get; set; }
    public int SleepTimeBeforeClosing { get; set; }
    public int MinWaitingTimeRange { get; set; }
    public int MaxWaitingTimeRange { get; set; }
}

static class Program
{
    private static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");

    private static Settings _settings = new Settings();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public const int SW_MINIMIZE = 6;

    public static async Task Main(string[] args)
    {
        ProgramApplication();
    }

    private static void ProgramApplication()
    {
        try
        {
            Console.WriteLine("*********************** Start");
            Console.Write("Do you want to download / update the Settings.json file?: [y/n]");
            string input = Console.ReadLine();
            input = input?.ToLower().Trim();
            if (input == "y")
            {
                AppendSettingsJsonFile();
            }

            Console.WriteLine("***********************");
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Loading Information...");
            DeserializeInformationJson();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("If you encounter any problems please contact your amazing girlfriend who did this program for you. Kuss");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("*****************************");
            Console.WriteLine($"LastPage: {_settings.LastPage}");
            Console.WriteLine($"LastProduct: {_settings.LastProduct}");
            Console.WriteLine($"SleepTimeBeforeClosing: {_settings.SleepTimeBeforeClosing}");
            Console.WriteLine($"MinWaitingTimeRange: {_settings.MinWaitingTimeRange}");
            Console.WriteLine($"MaxWaitingTimeRange: {_settings.MaxWaitingTimeRange}");
            Console.WriteLine("*****************************");
            Console.ForegroundColor = ConsoleColor.White;


            for (int page = _settings.LastPage; page <= 6; page++)
            {
                for (int product = _settings.LastProduct; product <= 9999999; product++)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{product}/9999999");
                    Console.ForegroundColor = ConsoleColor.White;

                    string line = $"https://www.galaxus.ch/s{page}/product/{product}/ratings/ratingform?OverallRating=4";
                    ProcessStartInfo psi = new ProcessStartInfo("brave.exe")
                    {
                        FileName = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe",
                        Arguments = line
                    };

                    Console.WriteLine($"Opened Tab: {DateTime.Now} - ({product}) {line}");
                    var process = Process.Start(psi);

                    // Minimize the window after the process has started
                    Thread.Sleep(1000); // Wait for a brief moment to ensure the window has been created
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        ShowWindow(process.MainWindowHandle, SW_MINIMIZE);
                    }

                    _settings.LastPage = page;
                    _settings.LastProduct = product;
                    UpdateSettingsFile();
                    Timer(_settings.SleepTimeBeforeClosing, "Time until Tab closing", "Tab closed.");

                    process.Kill();
                    int waitTime = new Random().Next(_settings.MinWaitingTimeRange, _settings.MaxWaitingTimeRange);
                    Timer(waitTime, "Time until next Tab", "*******************************");

                }
            }
            Console.WriteLine("Done!");
        }
        catch (Exception ex)
        {

            WriteErrorMessage(ex, "Programm");
        }
    }

    private static void DeserializeInformationJson()
    {
        try
        {
            string jsonContent = File.ReadAllText(filePath);
            _settings = JsonConvert.DeserializeObject<Settings>(jsonContent);
        }
        catch (Exception ex)
        {
            WriteErrorMessage(ex, "DeserializeInformationJson");
        }
    }

    private static void UpdateSettingsFile()
    {
        try
        {
            var updatedJsonContent = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            // Write the updated JSON content back to the file
            File.WriteAllText(filePath, updatedJsonContent);
            Console.WriteLine("Settings updated successfully!");
        }
        catch (Exception ex)
        {
            WriteErrorMessage(ex, "UpdateSettingsFile");
        }

    }

    private static void Timer(int timeInMinutes, string timerText, string closingText)
    {
        try
        {
            for (int i = timeInMinutes * 60; i > 0; i--)
            {
                int minutes = i / 60;
                string stringMinutes = minutes.ToString();
                int seconds = i % 60;
                string stringSeconds = seconds.ToString();

                if (seconds < 10)
                {
                    stringSeconds = $"0{stringSeconds}";
                }
                if (minutes < 10)
                {
                    stringMinutes = $"0{stringMinutes}";
                }

                Console.Write($"\r{timerText}: {stringMinutes} minutes {stringSeconds} seconds");
                Thread.Sleep(1000); // Sleep for 1 second
            }
            Console.Write($"\r{timerText}: 00 minutes 00 seconds");
            Thread.Sleep(500); // Sleep for half a
                               // second
            Console.Write("\r" + new string(' ', Console.WindowWidth - 1)); // Clear the line
            Console.WriteLine($"\r{closingText}");
            Console.WriteLine();
        }
        catch (Exception ex)
        {

            WriteErrorMessage(ex, "Timer");
        }

    }

    private static void AppendSettingsJsonFile()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Creating settings file...");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            Settings settings = new Settings
            {
                LastProduct = 75000,
                LastPage = 1,
                SleepTimeBeforeClosing = 1, // in minutes
                MinWaitingTimeRange = 5, // in minutes
                MaxWaitingTimeRange = 15 // in minutes
            };

            // Convert data to JSON string
            string jsonContent = JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);

            using (StreamWriter streamWriter = File.AppendText(filePath))
            {
                streamWriter.WriteLine(jsonContent);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Settings File created successfully at: " + filePath);
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch (Exception ex)
        {

            WriteErrorMessage(ex, "Sppend settings json file");
        }

    }

    private static void WriteErrorMessage(Exception ex, string place)
    {
        string traceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorlog.txt");
        if (!File.Exists(traceFilePath))
        {
            File.Create(traceFilePath);
        }

        Console.ForegroundColor = ConsoleColor.Red;
        string errorContent = $"{DateTime.Now}\nAn error occured in {place}\nError: {ex.Message}";
        Console.WriteLine(errorContent);

        using (StreamWriter streamWriter = File.AppendText(filePath))
        {
            streamWriter.WriteLine(errorContent);
        }

        Console.ForegroundColor = ConsoleColor.White;

    }

}
