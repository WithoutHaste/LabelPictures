using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPictures
{
	public class LabelPictures
	{
		private static string sourceDirectory = ConfigurationManager.AppSettings["SourceDirectory"] + "";
		private static string destinationDirectory = ConfigurationManager.AppSettings["DestinationDirectory"] + "";
		private static string[] validExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

		static void Main(string[] args)
		{
			ProcessImages();

			Console.WriteLine();
			Console.WriteLine("Process complete.");
			Console.ReadLine();
		}

		public static void ProcessImages()
		{
			string[] sourceFullPaths = Directory.GetFiles(sourceDirectory);
			string[] destinationFullPaths = Directory.GetFiles(destinationDirectory);

			foreach(string sourceFullPath in sourceFullPaths)
			{
				string sourceExtension = Path.GetExtension(sourceFullPath);
				if(!validExtensions.Contains(sourceExtension.ToLower())) continue;

				string sourceFileName = Path.GetFileName(sourceFullPath);
				if(destinationFullPaths.Any(x => Path.GetFileName(x) == sourceFileName)) continue;

				int buffer = 100;

				Image image = Image.FromFile(sourceFullPath);
				Bitmap editedImage = new Bitmap(image.Width, image.Height + buffer);
				using(Graphics graphics = Graphics.FromImage(editedImage))
				{
					graphics.DrawImage(image, 0, buffer, image.Width, image.Height);
				}

				string destinationFullPath = Path.Combine(destinationDirectory, sourceFileName);
				editedImage.Save(destinationFullPath);

				Console.WriteLine("Copied {0} to destination", sourceFileName);
			}
		}
	}
}
