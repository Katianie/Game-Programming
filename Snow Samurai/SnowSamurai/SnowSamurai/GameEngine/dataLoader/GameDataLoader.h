/*	
	GameDataLoader.h

	This class should read data from CSV (Comma Separated Values)
	files and uses them to load game data for a game
	application. Game data for rendering the game GUI is
	loaded when the game starts, data for displaying each
	game level would be loaded as needed. Note that we are not
	using this in the first benchmark.
*/

#pragma once
#include "../resource.h"
#include "../stdafx.h"

// FORWARD DECLARATIONS OF CLASSES USED BY GAME
class Game;

class GameDataLoader
{
private:
	bool myTilesLoaded;

public:
	// SIMPLE INLINED CONSTRUCTOR/DESTRUCTOR
	GameDataLoader()
	{
		myTilesLoaded = false;
	} 

	~GameDataLoader()			{}

	bool getTilesLoaded()					{	return myTilesLoaded;				}
	void setTilesLoaded(bool tilesLoaded)	{	tilesLoaded	= myTilesLoaded;		}

	// METHODS TO BE DEFINED INSIDE GameDataLoader.cpp
	void loadGUI(Game *game);
	void loadWorld(Game *game);
};