/*
	TextFileReader.cpp

	See TextFileReader.h for a class description.
*/

#include "../stdafx.h"
#include "../GameEngine/dataLoader/TextFileReader.h"

/*
	Default Constructor - We don't know the name of the file
	to use yet, so we'll initialize everything to NULL.
*/
TextFileReader::TextFileReader()
{
	fileName = NULL;
	inFile = NULL;
}

/*
	Deconstructor - Make sure the file stuff doesn't cause
	a memory leak.
*/
TextFileReader::~TextFileReader()
{
	closeReader();
}

/*
	closeReader - This method closes the stream used for 
	reading from a file and then deletes the inFile stream. If
	you want to read again from the stream, you must call initFile
	first, which will re-initialize everything.
*/
void TextFileReader::closeReader()
{
	if (fileName != NULL)
	{
		delete fileName;
		fileName = NULL;
	}

	if (inFile != NULL)
	{
		inFile->close();
		delete inFile;
		inFile = NULL;
	}
}

/*
	initFile - This method initializes the stream for reading
	from a file using the file name provided as an argument. After
	calling this method, we are ready to read text.
*/
void TextFileReader::initFile(wchar_t *initFileName)
{
	fileName = initFileName;
	inFile = new ifstream();
	inFile->open(fileName);
}

/*
	readLineOfText - This method reads a line of text from the
	file and returns it. Note that before calling this method, first
	call initFile to setup the stream. Note that this method is
	allocating memory on the heap for 256 characters for each line,
	so that should not be exceeded. Also, remember to delete the
	line when done using it to avoid memory leaks.
*/
wchar_t* TextFileReader::readLineOfText()
{
    if (!inFile)
        return NULL;

	char readText[256];
	inFile->getline(readText, 256);

	wchar_t *charLine = new wchar_t[256];
	charLine[0] = '\0';

	mbtowc(charLine, readText, 256 * sizeof(char));
	return charLine;
}

