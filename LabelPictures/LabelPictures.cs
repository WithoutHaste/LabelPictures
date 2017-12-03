using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabelPictures
{
	public class LabelPictures
	{
		private static string sourceDirectory = ConfigurationManager.AppSettings["SourceDirectory"];
		private static string destinationDirectory = ConfigurationManager.AppSettings["DestinationDirectory"];
		private static char fileNameDelimiter = ConfigurationManager.AppSettings["FileNameDelimiter"][0];
		private static Dictionary<int, string> displayTextFormats; //key=highest field index, value=format
		private static bool breakWordsOnCapitals = (ConfigurationManager.AppSettings["BreakWordsOnCapitals"] == "true");
		private static int? maxImageHeight = null;

		private static string[] validExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

		static void Main(string[] args)
		{
			LoadDisplayTextFormats();

			int temp;
			if(Int32.TryParse(ConfigurationManager.AppSettings["MaxImageHeight"], out temp))
			{
				maxImageHeight = temp;
			}

			ProcessImages();

			Console.WriteLine();
			Console.WriteLine("Process complete.");
			Console.ReadLine();
		}

		private static void LoadDisplayTextFormats()
		{
			displayTextFormats = new Dictionary<int, string>();

			string format = ConfigurationManager.AppSettings["DisplayTextFormat"];
			if(format != null)
			{
				displayTextFormats[HighestFieldIndex(format)] = format;
			}

			int count = 1;
			bool success = false;
			do
			{
				success = false;

				format = ConfigurationManager.AppSettings["DisplayTextFormat" + count];
				if(format != null)
				{
					success = true;
					int highestFieldIndex = HighestFieldIndex(format);
					if(!displayTextFormats.ContainsKey(highestFieldIndex))
					{
						displayTextFormats[highestFieldIndex] = format;
					}
				}

				count++;
			} while(success);
		}

		private static int HighestFieldIndex(string format)
		{
			for(int i = 9; i > 0; i--)
			{
				if(format.IndexOf("\\" + i) > -1)
				{
					return i;
				}
			}

			return 0;
		}

		private static void ProcessImages()
		{
			string[] sourceFullPaths = Directory.GetFiles(sourceDirectory);
			string[] destinationFullPaths = Directory.GetFiles(destinationDirectory);

			foreach(string sourceFullPath in sourceFullPaths)
			{
				string sourceExtension = Path.GetExtension(sourceFullPath);
				if(!validExtensions.Contains(sourceExtension.ToLower())) continue;

				string sourceFileName = Path.GetFileName(sourceFullPath);
				if(destinationFullPaths.Any(x => Path.GetFileName(x) == sourceFileName)) continue;

				int textBuffer = 100;
				int textPadding = 4;

				Image image = Image.FromFile(sourceFullPath);
				int editedImageHeight = image.Height;
				if(maxImageHeight.HasValue)
				{
					editedImageHeight = Math.Min(image.Height, maxImageHeight.Value - textBuffer);
				}
				double scale = (double)editedImageHeight / (double)image.Height;
				int editedImageWidth = (int)(image.Width * scale);

				Bitmap editedImage = new Bitmap(editedImageWidth, editedImageHeight + textBuffer);
				using(Graphics graphics = Graphics.FromImage(editedImage))
				{
					graphics.Clear(System.Drawing.Color.White);
					graphics.DrawImage(image, 0, textBuffer, editedImageWidth, editedImageHeight);

					string text = AssembleDisplayText(sourceFileName);
					Font font = GetLargestFont(text, editedImageWidth - (2 * textPadding), graphics);
					SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);
					graphics.DrawString(text, font, brush, textPadding, textPadding);
				}

				string destinationFullPath = Path.Combine(destinationDirectory, sourceFileName);
				editedImage.Save(destinationFullPath);

				Console.WriteLine("Copied {0} to destination", sourceFileName);
			}
		}

		private static string AssembleDisplayText(string fileName)
		{
			Dictionary<int, string> fields = PullFileNameFields(fileName);

			string text = SelectDisplayTextFormat(fields);
			foreach(KeyValuePair<int, string> pair in fields)
			{
				text = text.Replace("\\" + pair.Key.ToString(), pair.Value);
			}

			if(breakWordsOnCapitals)
			{
				text = Regex.Replace(text, "([a-z])([A-Z])", "$1 $2", RegexOptions.Compiled).Trim();
			}

			return text;
		}

		private static Dictionary<int, string> PullFileNameFields(string fileName)
		{
			Dictionary<int, string> fields = new Dictionary<int, string>();

			string[] nameAndExtension = fileName.Split('.');
			fileName = nameAndExtension[0];

			string[] split = fileName.Split(fileNameDelimiter);
			for(int i = 0; i < split.Length; i++)
			{
				fields[i + 1] = split[i];
			}

			return fields;
		}

		private static string SelectDisplayTextFormat(Dictionary<int, string> fields)
		{
			int maxIndex = fields.Keys.Max();
			while(maxIndex >= 0)
			{
				if(displayTextFormats.ContainsKey(maxIndex))
				{
					return displayTextFormats[maxIndex] + "";
				}
				maxIndex--;
			}

			return "";
		}

		private static Font GetLargestFont(string text, int width, Graphics graphics)
		{
			for(int fontSize = 48; fontSize > 5; fontSize--)
			{
				Font font = new Font("Arial", fontSize);
				SizeF textSize = graphics.MeasureString(text, font);
				if(textSize.Width < width)
				{
					return font;
				}
			}

			return new Font("Arial", 5);
		}
	}
}
