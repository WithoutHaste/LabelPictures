## LabelPictures

Workaround for displaying descriptive text with Windows Slideshow images.  Loops through a folder of images and saves them to another folder with filename information added to the image.

Only image files in the Source Directory that do not exist in the Destination Directory will be processed.

# Configuration

SourceDirectory: full path to source folder

DestinationDirectory: full path to destination folder

FileNameDelimiter: single character to split filename into fields by (extension of filename is ignored)
	
DisplayTextFormat: 

	each \digit will be replaced by the corresponding match from the Regex
	ex: "Artist: \1, Title: \2" becomes "Artist: da Vinci, Title: The Mona Lisa"
	
	only works for single digit values
	
	accepts multiple DisplayTextFormat settings, as DisplayTextFormat1, DisplayTextFormat2, etc
	the first one with the correct number of \digit to match the filename fields will be used
	
BreakWordsOnCapitals: if "true", then inserts spaces between camel-cased words
	
	ex: "TheAncientOfDays" becomes "The Ancient Of Days"
