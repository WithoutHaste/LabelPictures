using System;
using System.Collections.Generic;
using System.Configuration;
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
				string sourceFileName = Path.GetFileName(sourceFullPath);
				if(destinationFullPaths.Any(x => Path.GetFileName(x) == sourceFileName)) continue;

				string destinationFullPath = Path.Combine(destinationDirectory, sourceFileName);
				File.Copy(sourceFullPath, destinationFullPath);

				Console.WriteLine("Copied {0} to destination", sourceFileName);
			}
		}
	}
}
