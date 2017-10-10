/*
	TextureManager.cpp

	See TextureManager.h for a class description.
*/

#include "../stdafx.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/graphics/GameGraphics.h"
#include "../GameEngine/os/GameOS.h"
#include "../GameEngine/game/StringTable.h"
#include "../GameEngine/dataLoader/TextFileWriter.h"
#include "../GameEngine/graphics/TextureManager.h"
#include <map>
#include <vector>

/*
	TextureManager - Default Constructor, it creates the StringTable.
*/
TextureManager::TextureManager()  
{
	stringTable = new StringTable();
}

/*
	~TextureManager - Destructor, it cleans up the StringTable pointer.
*/
TextureManager::~TextureManager() 
{
	delete stringTable;
}