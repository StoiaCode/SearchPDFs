using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

class SearchPDFs {
	static void Main() {
		string toBeFound = string.Empty;
		string directory = string.Empty;
		string outputDir = string.Empty;

		while (directory.Equals(string.Empty)) {
			Console.Write("Input the Directory to scan in: ");
			directory = Console.ReadLine();
		}

		while (toBeFound.Equals(string.Empty)) {
			Console.Write("Input what to search for: ");
			toBeFound = Console.ReadLine();
		}

		while (outputDir.Equals(string.Empty)) {
			Console.Write("Where should we save the found files? ");
			outputDir = Console.ReadLine();
			Directory.CreateDirectory(outputDir);
			outputDir += "\\outputFile.txt";
		}

		List<string> found = new List<string>();

		try {
			foreach (string file in Directory.EnumerateFiles(directory, "*.pdf", SearchOption.AllDirectories)) {
				PdfDocument pdf = new PdfDocument(new PdfReader(file));

				for (int i = 1;i <= pdf.GetNumberOfPages();i++) {
					var pageText = PdfTextExtractor.GetTextFromPage(pdf.GetPage(i));
					if (pageText.Contains(toBeFound)) {
						Console.WriteLine(pageText);
						found.Add(file);
					}
				}
			}
		} catch (Exception e) {
			Console.WriteLine(e);
			throw;
		}


		StreamWriter saveFile = new StreamWriter(outputDir);
		found.ForEach(saveFile.WriteLine);
		saveFile.Close();

		Console.WriteLine($"There have been {found.Count} matches. You can finde them in {outputDir}. Press ENTER to close this window.");
		Console.ReadLine();
	}
}