/*
	GameWorld.cpp

	See GameWorld.h for a class description.
*/

#include "../stdafx.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/dataLoader/GameDataLoader.h"
#include "../GameEngine/graphics/GameGraphics.h"
#include "../GameEngine/world/GameWorld.h"
#include "../GameEngine/graphics/RenderList.h"
#include "../GameEngine/world/SpriteManager.h"
#include "../GameEngine/graphics/TextureManager.h"
#include "../GameEngine/world/Viewport.h"
#include <vector>

/*
	GameWorld - Default Constructor, it constructs the layers
	vector, allowing new layers to be added.
*/
GameWorld::GameWorld()	
{
	layers = new vector<WorldLayer*>();
	worldWidth = 0;
	worldHeight = 0;
	currentLevel = 0;
	spriteManager = new SpriteManager();
	viewport = new Viewport();
	levelFileNames = new vector<wchar_t*>();
}

/*
	~GameWorld - This destructor will remove the memory allocated
	for the layer vector.
*/
GameWorld::~GameWorld()	
{
	delete layers;
	delete spriteManager;
	delete viewport;
	delete levelFileNames;
}

/*
	addLayer - This method is how layers are added to the World.
	These layers might be TiledLayers, SparseLayers, or 
	IsometricLayers, all of which are child classes of WorldLayer.
*/
void GameWorld::addLayer(WorldLayer *layerToAdd)
{
	layers->push_back(layerToAdd);
}

/*
	addWorldRenderItemsToRenderList - This method sends the render
	list and viewport to each of the layers such that they
	may fill it with RenderItems to draw.
*/
void GameWorld::addWorldRenderItemsToRenderList(Game *game)
{
	if (game->getGameState() == GAME_PLAYING_GAME_STATE || game->getGameState() == PAUSED_GAME_STATE)
	{
		GameGraphics *graphics = game->getGraphics();
		RenderList *renderList = graphics->getWorldRenderList();

		for (int i = 0; i < layers->size(); i++)
		{
			layers->at(i)->addRenderItemsToRenderList(	renderList,
													viewport);
		}
		spriteManager->addSpriteItemsToRenderList(renderList, viewport);
	}
}

/*
	addLevelFileName - This method adds a level file name to the vector
	of all the level file names. Storing these file names allows us to
	easily load a desired level at any time.
*/
void GameWorld::addLevelFileName(wchar_t *levelFileName)
{
	levelFileNames->push_back(levelFileName);
}

/*
	clear - This method removes all data from the World. It should
	be called first when a level is unloaded or changed. If it
	is not called, an application runs the risk of having lots
	of extra data sitting around that may slow the progam down.
	Or, if the world thinks a level is still active, it might add
	items to the render list using image ids that have already been
	cleared from the GameGraphics' texture manager for the world.
	That would likely result in an exception.
*/
void GameWorld::unloadCurrentLevel(Game *game)
{
	spriteManager->clearSprites();
	layers->clear();	
	currentLevel = 0;
	worldWidth = 0;
	worldHeight = 0;

	game->getWorld()->getViewport()->setViewportX(0);
	game->getWorld()->getViewport()->setViewportY(0);
}

/*
	getLevelFileName - This method gets the name of the file used to
	load the current level. Each level has a file where the layout
	of the level and what artwork to use is specified.
*/
wchar_t* GameWorld::getLevelFileName(int levelNumber)
{
	return levelFileNames->at(levelNumber-1);
}

/*
	loadCurrentLevel - This method changes the current level. This method should
	be called before loading all the data from a level file.
*/
void GameWorld::loadCurrentLevel(Game *game, int initLevel)
{
	if ((initLevel > 0) && (initLevel <= levelFileNames->size()))
	{
		unloadCurrentLevel(game);
		currentLevel = initLevel;
		GameDataLoader *dataLoader = game->getDataLoader();
		dataLoader->loadWorld(game);
	}
}

/*
	update - This method should be called once per frame. It updates
	all of the dynamic level data, like sprite animation states and
	particle locations.
*/
void GameWorld::update(Game *game)
{
	spriteManager->updateAllSprites(game);
}