using System.Diagnostics;
using System.Runtime.InteropServices;

static class Program
{
	// The path to the file obvi
	public static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GalaxusLinks.txt");
	public const int SleepTimeBeforeClosing = 15000; // in miliseconds
	public static void Main(string[] args)
	{
		// Creates file but asks u if u want it ;)
		Console.Write("Update / Create file?: [y/n] ");
		string? input = Console.ReadLine();
		input = input.ToLower().Trim();
		if (input == "y")
		{
			AppendToFile();
		}

		// pretty obvious.

		foreach (string line in File.ReadLines(filePath))
		{
			// starts chrome with the galaxus link (line = galaxus link)
			Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", line);

			Thread.Sleep(SleepTimeBeforeClosing);
			// closes the tab so basically whichever tab ur active on
			PressCtrlW();

			// waits 5 to 15 minutes.
			int waitTime = new Random().Next(5, 16);

			Console.WriteLine($"{DateTime.Now} - ({line}) \n Minutes until next Tab: {waitTime}");
			// Minutes in miliseconds
			Thread.Sleep(waitTime * 60 * 1000);
		}
		Console.WriteLine("Done!");
	}

	static void AppendToFile()
	{
		// Deltes the file if it exists and u said yes to creating one
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}

		Console.WriteLine("File Creation is starting.");
		using (StreamWriter sw = File.CreateText(filePath))
		{
			// for every page there is so s1, s2, s3 ...
			for (int i = 1; i < 7; i++)
			{
				// A huge range a products
				for (int num = 75000; num <= 9999999; num++)
				{
					// Galaxus string with rating 4
					string content = $"https://www.galaxus.ch/s{i}/product/{num}/ratings/ratingform?OverallRating=4";

					// Write the content to the file
					sw.WriteLine(content);

				}
				Console.WriteLine(i);
			}
			Console.WriteLine($"The file is located under: {filePath}");
		}
	}

	// Hack to press ctrl something
	[DllImport("user32.dll")]
	public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

	private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
	private const int KEYEVENTF_KEYUP = 0x0002;

	static void PressCtrlW()
	{
		// Simulate pressing Ctrl key
		keybd_event((byte)0x11, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);

		// Simulate pressing W key
		keybd_event((byte)0x57, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);

		// Simulate releasing W key
		keybd_event((byte)0x57, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);

		// Simulate releasing Ctrl key
		keybd_event((byte)0x11, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
	}
}