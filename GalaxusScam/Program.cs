using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.InteropServices;

class Products
{
	public int Id { get; set; }
	public int Page { get; set; }
	public int Product { get; set; }
}

class AdditionalDetailsInformation
{
	public int LastProductId { get; set; }
	public int SleepTimeBeforeClosing { get; set; }
	public int MinWaitingTimeRange { get; set; }
	public int MaxWaitingTimeRange { get; set; }
}

static class Program
{
	public static string fileProductsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Products.json");
	public static string fileInfoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");

	static AdditionalDetailsInformation _additionalInformation = new AdditionalDetailsInformation();
	static List<Products> _products = new List<Products>();

	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	public const int SW_MINIMIZE = 6;

	public static async Task Main(string[] args)
	{
		ProgramApplication();
	}

	static void ProgramApplication()
	{
		Console.WriteLine("*********************** Start");
		Console.WriteLine("Do you want to download a file?:");
		Console.WriteLine(" Press Y for Settings.json and Products.json");
		Console.WriteLine(" Press S for Settings.json");
		Console.WriteLine(" Press P for Products.json");
		Console.WriteLine(" Press anything else for none");
		string input = Console.ReadLine();
		input = input?.ToLower().Trim();
		if (input == "y")
		{
			AppendInformationJsonFile();
			AppendProductsJsonFile();
		}
		else if (input == "s")
		{
			AppendInformationJsonFile();
		}
		else if (input == "p")
		{ 
			AppendProductsJsonFile();
		}
		Console.WriteLine("***********************");
		Console.Clear();
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine("Loading Information...");
		DeserializeInformationJson();
		DeserializeProductsJson();
		Console.Clear();
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine("If you encounter any problems please contact your amazing girlfriend who did this program for you. Kuss");
		Console.ForegroundColor = ConsoleColor.Cyan;
		Products lastProduct = _products.FirstOrDefault(p => p.Id == _additionalInformation.LastProductId); 
		Console.WriteLine("*****************************");
		Console.WriteLine($"LastProduct:");
		Console.ForegroundColor = ConsoleColor.DarkCyan;
		Console.WriteLine($" Id: {lastProduct.Id}");
		Console.WriteLine($" Page: S{lastProduct.Page}");
		Console.WriteLine($" Product Number: {lastProduct.Product}");
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.WriteLine($"SleepTimeBeforeClosing: {_additionalInformation.SleepTimeBeforeClosing}");
		Console.WriteLine($"MinWaitingTimeRange: {_additionalInformation.MinWaitingTimeRange}");
		Console.WriteLine($"MaxWaitingTimeRange: {_additionalInformation.MaxWaitingTimeRange}");
		Console.WriteLine("*****************************");
		Console.ForegroundColor = ConsoleColor.White;
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("Reviews to go: " + (_products.Count - _additionalInformation.LastProductId));
		Console.ForegroundColor = ConsoleColor.White;

		foreach (Products product in _products)
		{
			if (product.Id < _additionalInformation.LastProductId)
			{
				continue;
			}

			string line = $"https://www.galaxus.ch/s{product.Page}/product/{product.Product}/ratings/ratingform?OverallRating=4";
			ProcessStartInfo psi = new ProcessStartInfo("brave.exe")
			{
				FileName = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe",
				Arguments = line
			};

			Console.WriteLine($"Opened Tab: {DateTime.Now} - ({product.Id}) {line}");
			var process = Process.Start(psi);

			// Minimize the window after the process has started
			Thread.Sleep(1000); // Wait for a brief moment to ensure the window has been created
			if (process.MainWindowHandle != IntPtr.Zero)
			{
				ShowWindow(process.MainWindowHandle, SW_MINIMIZE);
			}

			Timer(_additionalInformation.SleepTimeBeforeClosing, "Time until Tab closing", "Tab closed.");

			process.Kill();
			int waitTime = new Random().Next(1);
			Timer(waitTime, "Time until next Tab", "*******************************");
			
			_additionalInformation.LastProductId = product.Id;
			UpdateLastProuctId();

		}

		Console.WriteLine("Done!");
	}

	static void DeserializeInformationJson()
	{
		string jsonContent = File.ReadAllText(fileInfoPath);
		_additionalInformation = JsonConvert.DeserializeObject<AdditionalDetailsInformation>(jsonContent);
	}

	static void DeserializeProductsJson()
	{
		using (StreamReader streamReader = File.OpenText(fileProductsPath))
		using (JsonTextReader reader = new JsonTextReader(streamReader))
		{
			JsonSerializer serializer = new JsonSerializer();

			// Ensure we start reading from the beginning of an array
			if (!reader.Read() || reader.TokenType != JsonToken.StartArray)
			{
				throw new JsonReaderException("Expected start of array");
			}

			// Read each JSON object within the array
			while (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				Products product = serializer.Deserialize<Products>(reader);
				_products.Add(product);
			}
		}
	}

	static void UpdateLastProuctId()
	{
		var updatedJsonContent = JsonConvert.SerializeObject(_additionalInformation, Formatting.Indented);
		// Write the updated JSON content back to the file
		File.WriteAllText(fileInfoPath, updatedJsonContent);
	}

	static void Timer(int timeInMinutes, string timerText, string closingText)
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
		Thread.Sleep(500); // Sleep for 1 second
		Console.Write("\r" + new string(' ', Console.WindowWidth - 1)); // Clear the line
		Console.WriteLine($"\r{closingText}");
		Console.WriteLine();
	}

	static void AppendInformationJsonFile()
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine("Creating settings file...");
		if (File.Exists(fileInfoPath))
		{
			File.Delete(fileInfoPath);
		}

		AdditionalDetailsInformation
			additionalInformation = new AdditionalDetailsInformation
			{
				LastProductId = 1,
				SleepTimeBeforeClosing = 1, // in minutes
				MinWaitingTimeRange = 5, // in minutes
				MaxWaitingTimeRange = 15 // in minutes
			};

		// Convert data to JSON string
		string jsonContent = JsonConvert.SerializeObject(additionalInformation, Newtonsoft.Json.Formatting.Indented);

		using (StreamWriter streamWriter = File.AppendText(fileInfoPath))
		{
			streamWriter.WriteLine(jsonContent);
		}
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("Settings File created successfully at: " + fileInfoPath);
		Console.ForegroundColor= ConsoleColor.White;
	}

	static void AppendProductsJsonFile()
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		int IdCount = 1;
		Console.Write("Creating products file... 0%");
		for (int i = 1; i < 7; i++)
		{
			List<Products> result = new List<Products>();
			for (int num = 75000; num <= 9999999; num++)
			{
				Products products = new Products()
				{
					Id = IdCount,
					Page = i,
					Product = num
				};
				result.Add(products);
				IdCount++;
			}

			string progress = ((100.00/6.00) * i).ToString("0.00");
			Console.Write($"\rCreating products file... {progress}%");

			string jsonContent = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

			using (StreamWriter streamWriter = File.AppendText(fileProductsPath))
			{
				streamWriter.WriteLine(jsonContent);
			}
		}
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("Products file created successfully at: " + fileInfoPath);
		Console.ForegroundColor = ConsoleColor.White;
	}


	/*
	static void AppendToFile()
	{


		Console.WriteLine("File Creation is starting.");
		using (StreamWriter sw = File.CreateText(filePath))
		{
			for (int i = 1; i < 7; i++)
			{
				for (int num = 75000; num <= 9999999; num++)
				{
					string content = $"https://www.galaxus.ch/s{i}/product/{num}/ratings/ratingform?OverallRating=4";
					sw.WriteLine(content);
				}
				Console.WriteLine(i);
			}
			Console.WriteLine($"The file is located under: {filePath}");
		}
	}*/
}
