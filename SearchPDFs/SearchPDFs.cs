using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Commons.Actions;


/*	A simple tool that searches for text in all PDFs in a folder + Subdirectories.
    Copyright (C) 2023 Stoia

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
class SearchPDFs {
	static void Main() {

		// Disable Warning
		EventManager.AcknowledgeAgplUsageDisableWarningMessage();

		string toBeFound = string.Empty;
		string directory = string.Empty;
		string outputDir = string.Empty;

		double count = 0;
		int found = 0;
		int errCount = 0;
		int maxErr = 10;

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

		StreamWriter saveFile = new StreamWriter(outputDir);

		try {
			var files = Directory.EnumerateFiles(directory, "*.pdf", SearchOption.AllDirectories);

			Console.WriteLine($"Scanning {files.Count()} .pdfs. Starting Work...");

			double divisor = Math.Pow(10, (int)Math.Log10(files.Count()) + 1);
			double formattedNumber = 1 - (Math.Round(files.Count() / divisor, 2));
			count = formattedNumber;

			using (var progress = new ProgressBar()) {
				foreach (string file in files) {
					try {
						PdfDocument pdf = new PdfDocument(new PdfReader(file));

						for (int i = 1;i <= pdf.GetNumberOfPages();i++) {
							var pageText = PdfTextExtractor.GetTextFromPage(pdf.GetPage(i));
							if (pageText.Contains(toBeFound)) {
								Console.WriteLine($"Found a file at: {file}");
								saveFile.WriteLine(file);
								found++;
							}
						}

						progress.Report(count);
						count--;

					} catch (Exception e) {
						errCount++;
						Console.WriteLine($"Error: {e}");
						Console.WriteLine($"At file: {file}");

						if (errCount > maxErr) {
							Console.WriteLine($"We had {errCount} Errors, something is off. Continue? (Y)es/(N)o");
							var shouldCont = Console.ReadKey(true);

							if (shouldCont.Equals('y')) {
								maxErr += 10;
								Console.WriteLine("Continue...");
								continue;
							} else {
								throw;
							}

						} else {
							Console.WriteLine("Continue...");
							continue;
						}
					}
				}
			}
		} catch (Exception e) {
			Console.WriteLine(e);
			Console.WriteLine();
			Console.WriteLine("Fuck. We crashed. Press Enter to exit. All found things _should_ still be saved!");
			Console.ReadLine();
		}

		saveFile.Close();

		Console.WriteLine($"There have been {found} matches. You can finde them in {outputDir}. Press ENTER to close this window.");
		Console.ReadLine();
	}
}