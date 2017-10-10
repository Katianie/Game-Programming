#include <iostream>
#include <fstream>
#include <string>
#include <stdio.h>
#include <stdlib.h>

#include "../GameEngine/world/AnimatedSprite.h"
#include "../GameEngine/world/AnimatedSpriteType.h"
#include "../GameEngine/physics/BoundingVolume.h"
#include "../GameEngine/physics/CollidableObject.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/dataLoader/GameDataLoader.h"
#include "../GameEngine/graphics/GameGraphics.h"
#include "../GameEngine/world/GameWorld.h"
#include "../GameEngine/physics/PhysicalProperties.h"
#include "../GameEngine/world/TiledLayer.h"
#include "../GameEngine/world/SparseLayer.h"
#include "../GameEngine/world/SpriteManager.h"
#include "../GameEngine/world/Viewport.h"
#include "../GameEngine/text/GameText.h"
#include "../GameEngine/dataLoader/GameDataLoader.h"

using namespace std;

vector<int> readCSV(wchar_t *file)
{
	ifstream inputFile(file);
	string line;
	int linenum = 0;
	vector<int> itemNums;

	while (getline(inputFile, line))
	{  
		linenum++;
		istringstream linestream(line);
		string item;
		int itemnum = 0;

		while (getline (linestream, item, ','))
		{
			itemnum++;

			itemNums.push_back(atoi(item.c_str()));  
		}
	}

	return itemNums;
}

AnimatedSprite* createPlayer(Game *game)
{
	GameWorld *world = game->getWorld();
	GameGraphics *graphics = game->getGraphics();
	TextureManager *worldTextureManager = graphics->getWorldTextureManager();
	AnimatedSpriteType *ast = new AnimatedSpriteType();

	int spriteStandID1;
	int spriteStandID2;

	int spriteStandLeftID1;
	int spriteStandLeftID2;

	int spriteRunID1;
	int spriteRunID2;
	int spriteRunID3;
	int spriteRunID4;
	int spriteRunID5;
	int spriteRunID6;
	int spriteRunID7;
	int spriteRunID8;
	int spriteRunID9;
	int spriteRunID10;
	int spriteRunID11;
	int spriteRunID12;
	int spriteRunID13;
	int spriteRunID14;

	int spriteRunLeftID1;
	int spriteRunLeftID2;
	int spriteRunLeftID3;
	int spriteRunLeftID4;
	int spriteRunLeftID5;
	int spriteRunLeftID6;
	int spriteRunLeftID7;
	int spriteRunLeftID8;
	int spriteRunLeftID9;
	int spriteRunLeftID10;
	int spriteRunLeftID11;
	int spriteRunLeftID12;
	int spriteRunLeftID13;
	int spriteRunLeftID14;

	int spriteJumpID1;
	int spriteJumpID2;
	int spriteJumpID3;
	int spriteJumpID4;
	int spriteJumpID5;

	int spriteJumpLeftID1;
	int spriteJumpLeftID2;
	int spriteJumpLeftID3;
	int spriteJumpLeftID4;
	int spriteJumpLeftID5;

	int spriteAttackID1;
	int spriteAttackID2;
	int spriteAttackID3;

	int spriteAttackLeftID1;
	int spriteAttackLeftID2;
	int spriteAttackLeftID3;

	wchar_t *fileName;

	//PLAYER STANDING FRAMES-----------------------------------------------------------------------------
	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_3_0.bmp");
	spriteStandID1 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteStandID1);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_3_1.bmp");
	spriteStandID2 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteStandID2);

	//PLAYER STANDING LEFT FRAMES------------------------------------------------------------------------
	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_5_0.bmp");
	spriteStandLeftID1 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteStandLeftID1);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_5_1.bmp");
	spriteStandLeftID2 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteStandLeftID2);

	//PLAYER RUNNING FRAMES----------------------------------------------------------------------------
	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_2.bmp");
	spriteRunID1 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID1);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_3.bmp");
	spriteRunID2 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID2);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_4.bmp");
	spriteRunID3 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID3);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_5.bmp");
	spriteRunID4 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID4);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_6.bmp");
	spriteRunID5 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID5);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_7.bmp");
	spriteRunID6 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID6);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_8.bmp");
	spriteRunID7 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID7);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_9.bmp");
	spriteRunID8 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID8);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_10.bmp");
	spriteRunID9 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID9);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_11.bmp");
	spriteRunID10 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID10);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_12.bmp");
	spriteRunID11 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID11);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_13.bmp");
	spriteRunID12 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID12);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_14.bmp");
	spriteRunID13 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID13);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_15.bmp");
	spriteRunID14 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunID14);

	//PLAYER RUNNING LEFT FRAMES---------------------------------------------------------------------------------
	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_2.bmp");
	spriteRunLeftID1 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID1);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_3.bmp");
	spriteRunLeftID2 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID2);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_4.bmp");
	spriteRunLeftID3 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID3);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_5.bmp");
	spriteRunLeftID4 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID4);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_6.bmp");
	spriteRunLeftID5 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID5);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_7.bmp");
	spriteRunLeftID6 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID6);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_8.bmp");
	spriteRunLeftID7 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID7);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_9.bmp");
	spriteRunLeftID8 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID8);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_10.bmp");
	spriteRunLeftID9 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID9);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_11.bmp");
	spriteRunLeftID10 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID10);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_12.bmp");
	spriteRunLeftID11 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID11);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_13.bmp");
	spriteRunLeftID12 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID12);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_14.bmp");
	spriteRunLeftID13 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID13);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_4_15.bmp");
	spriteRunLeftID14 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteRunLeftID14);


	//PLAYER JUMPING FRAMES-----------------------------------------------------------------------------
	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_2_0.bmp");
	spriteJumpID1 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteJumpID1);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_2_1.bmp");
	spriteJumpID2 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteJumpID2);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_2_2.bmp");
	spriteJumpID3 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteJumpID3);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_2_3.bmp");
	spriteJumpID4 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteJumpID4);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_2_4.bmp");
	spriteJumpID5 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteJumpID5);

	//PLAYER JUMPING LEFT FRAMES-----------------------------------------------------------------------------
	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_9_0.bmp");
	spriteJumpLeftID1 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteJumpLeftID1);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_9_1.bmp");
	spriteJumpLeftID2 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteJumpLeftID2);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_9_2.bmp");
	spriteJumpLeftID3 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteJumpLeftID3);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_9_3.bmp");
	spriteJumpLeftID4 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteJumpLeftID4);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_9_4.bmp");
	spriteJumpLeftID5 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteJumpLeftID5);

	//PLAYER ATTACKING FRAMES-----------------------------------------------------------------------------
	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_1_0.bmp");
	spriteAttackID1 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteAttackID1);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_1_1.bmp");
	spriteAttackID2 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteAttackID2);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_1_2.bmp");
	spriteAttackID3 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteAttackID3);

	//PLAYER ATTACKING LEFT FRAMES-----------------------------------------------------------------------------
	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_7_0.bmp");
	spriteAttackLeftID1 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteAttackLeftID1);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_7_1.bmp");
	spriteAttackLeftID2 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteAttackLeftID2);

	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_7_2.bmp");
	spriteAttackLeftID3 = worldTextureManager->loadTexture(fileName);
	ast->addImageID(spriteAttackLeftID3);

	// CHANGE THE IMAGE ONCE EVERY 60 FRAMES
	ast->setAnimationSpeed(60);

	// SIZE OF SPRITE IMAGES
	ast->setTextureSize(64, 64);

	// NOW LET'S ADD AN ANIMATION STATE
	// FIRST THE NAME
	wchar_t *standingStateName = constructEmptyWCHAR_TArray(L"STANDING RIGHT");
	vector<int> *standingSequence = new vector<int>();

	wchar_t *runningStateName = constructEmptyWCHAR_TArray(L"RUNNING RIGHT");
	vector<int> *runningSequence = new vector<int>();

	wchar_t *jumpingStateName = constructEmptyWCHAR_TArray(L"JUMPING RIGHT");
	vector<int> *jumpingSequence = new vector<int>();

	wchar_t *attackingStateName = constructEmptyWCHAR_TArray(L"ATTACKING RIGHT");
	vector<int> *attackingSequence = new vector<int>();

	wchar_t *standingLeftStateName = constructEmptyWCHAR_TArray(L"STANDING LEFT");
	vector<int> *standingLeftSequence = new vector<int>();

	wchar_t *runningLeftStateName = constructEmptyWCHAR_TArray(L"RUNNING LEFT");
	vector<int> *runningLeftSequence = new vector<int>();

	wchar_t *jumpingLeftStateName = constructEmptyWCHAR_TArray(L"JUMPING LEFT");
	vector<int> *jumpingLeftSequence = new vector<int>();

	wchar_t *attackingLeftStateName = constructEmptyWCHAR_TArray(L"ATTACKING LEFT");
	vector<int> *attackingLeftSequence = new vector<int>();

	standingSequence->push_back(spriteStandID1);
	standingSequence->push_back(spriteStandID2);
	standingSequence->push_back(spriteStandID1);
	standingSequence->push_back(spriteStandID1);
	standingSequence->push_back(spriteStandID2);
	standingSequence->push_back(spriteStandID1);
	standingSequence->push_back(spriteStandID2);

	standingLeftSequence->push_back(spriteStandLeftID1);
	standingLeftSequence->push_back(spriteStandLeftID2);
	standingLeftSequence->push_back(spriteStandLeftID1);
	standingLeftSequence->push_back(spriteStandLeftID1);
	standingLeftSequence->push_back(spriteStandLeftID2);
	standingLeftSequence->push_back(spriteStandLeftID1);
	standingLeftSequence->push_back(spriteStandLeftID2);

	runningSequence->push_back(spriteRunID1);
	runningSequence->push_back(spriteRunID2);
	runningSequence->push_back(spriteRunID3);
	runningSequence->push_back(spriteRunID4);
	runningSequence->push_back(spriteRunID5);
	runningSequence->push_back(spriteRunID6);
	runningSequence->push_back(spriteRunID7);
	runningSequence->push_back(spriteRunID8);
	runningSequence->push_back(spriteRunID9);
	//runningSequence->push_back(spriteRunID10);
	//runningSequence->push_back(spriteRunID11); commented out because it makes running look choppy
	runningSequence->push_back(spriteRunID12);
	runningSequence->push_back(spriteRunID13);
	runningSequence->push_back(spriteRunID14);

	runningLeftSequence->push_back(spriteRunLeftID1);
	runningLeftSequence->push_back(spriteRunLeftID2);
	runningLeftSequence->push_back(spriteRunLeftID3);
	runningLeftSequence->push_back(spriteRunLeftID4);
	runningLeftSequence->push_back(spriteRunLeftID5);
	runningLeftSequence->push_back(spriteRunLeftID6);
	runningLeftSequence->push_back(spriteRunLeftID7);
	runningLeftSequence->push_back(spriteRunLeftID8);
	runningLeftSequence->push_back(spriteRunLeftID9);
	//runningLeftSequence->push_back(spriteRunLeftID10);
	//runningLeftSequence->push_back(spriteRunLeftID11); commented out because it makes running look choppy
	runningLeftSequence->push_back(spriteRunLeftID12);
	runningLeftSequence->push_back(spriteRunLeftID13);
	runningLeftSequence->push_back(spriteRunLeftID14);

	jumpingSequence->push_back(spriteJumpID1);
	jumpingSequence->push_back(spriteJumpID2);
	jumpingSequence->push_back(spriteJumpID3);
	jumpingSequence->push_back(spriteJumpID4);
	jumpingSequence->push_back(spriteJumpID5);

	jumpingLeftSequence->push_back(spriteJumpLeftID1);
	jumpingLeftSequence->push_back(spriteJumpLeftID2);
	jumpingLeftSequence->push_back(spriteJumpLeftID3);
	jumpingLeftSequence->push_back(spriteJumpLeftID4);
	jumpingLeftSequence->push_back(spriteJumpLeftID5);

	attackingSequence->push_back(spriteAttackID1);
	attackingSequence->push_back(spriteAttackID2);
	attackingSequence->push_back(spriteAttackID3);

	attackingLeftSequence->push_back(spriteAttackLeftID1);
	attackingLeftSequence->push_back(spriteAttackLeftID2);
	attackingLeftSequence->push_back(spriteAttackLeftID3);

	ast->addAnimationState(standingStateName, standingSequence);//0
	ast->addAnimationState(runningStateName, runningSequence);//1
	ast->addAnimationState(standingLeftStateName, standingLeftSequence);//2
	ast->addAnimationState(runningLeftStateName, runningLeftSequence);//3
	ast->addAnimationState(jumpingStateName, jumpingSequence);//4
	ast->addAnimationState(jumpingLeftStateName, jumpingLeftSequence);//5
	ast->addAnimationState(attackingStateName, attackingSequence);//6
	ast->addAnimationState(attackingLeftStateName, attackingLeftSequence);//7

	SpriteManager *spriteManager = world->getSpriteManager();
	spriteManager->addSpriteType(ast);

	AnimatedSprite *player = new AnimatedSprite();
	player->setSpriteType(ast);

	PhysicalProperties *playerProps = new PhysicalProperties();
	player->setPhysicalProperties(playerProps);
	playerProps->setX(200);
	playerProps->setY(GROUND_HEIGHT);
	playerProps->setAccelerationX(0);
	playerProps->setAccelerationY(0);
	//// WE WILL SET LOTS OF OTHER PROPERTIES ONCE
	//// WE START DOING COLLISIONS AND PHYSICS

	player->setScore(0);

	BoundingVolume *boundingBox = new BoundingVolume();
	player->setBoundingBox(boundingBox);
	boundingBox->setX(playerProps->getX());
	boundingBox->setY(playerProps->getY());
	boundingBox->setWidth(64);
	boundingBox->setHeight(64);

	player->setAlpha(255);
	player->setCurrentState(STANDING_FACING_RIGHT);
	player->setIsFacingRight(true);
	
	spriteManager->addSprite(player);
	spriteManager->setPlayer(player);

	return player;
}

//void loadSlideshow(Game *game)
//{
//	// FIRST SETUP THE GAME WORLD DIMENSIONS
//	GameWorld *world = game->getWorld();
//	GameGraphics *graphics = game->getGraphics();
//	TextureManager *worldTextureManager = graphics->getWorldTextureManager();
//	Viewport *viewport = world->getViewport();
//	viewport->setViewportWidth(1024);
//	viewport->setViewportHeight(768);
//	viewport->setViewportOffsetX(0);
//	viewport->setViewportOffsetY(0);
//	world->setWorldWidth(1024);
//	world->setWorldHeight(768);
//
//	AnimatedSpriteType *ast = new AnimatedSpriteType();
//
//	wchar_t *fileName = constructEmptyWCHAR_TArray(L"textures/world/slides/Storyboard_Gameplay.png");
//	int storyBoardGameplayID = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(storyBoardGameplayID);
//
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/slides/Storyboard_IngameMenu.png");
//	int storyBoardIngameMenuID = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(storyBoardIngameMenuID);
//
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/slides/Storyboard_SplashScreen.png");
//	int storyBoardSplashScreenID = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(storyBoardSplashScreenID);
//
//	fileName = constructEmptyWCHAR_TArray(L"textures/gui/overlays/Game_Over.bmp");
//	int gameOverScreenID = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(gameOverScreenID);
//
//	// CHANGE THE IMAGE ONCE EVERY 20 FRAMES
//	ast->setAnimationSpeed(20);
//
//	// SIZE OF SPRITE IMAGES
//
//	// NOW LET'S ADD AN ANIMATION STATE
//	// FIRST THE NAME
//	wchar_t *animStateName = constructEmptyWCHAR_TArray(L"PLAYING");
//	ast->addAnimationState(animStateName);
//	vector<int> *playSequence = new vector<int>();
//
//	ast->addAnimationFrame(0, storyBoardGameplayID);
//	ast->addAnimationFrame(0, storyBoardIngameMenuID);
//	ast->addAnimationFrame(0, storyBoardSplashScreenID);
//	ast->addAnimationFrame(0, gameOverScreenID);
//
//	SpriteManager *spriteManager = world->getSpriteManager();
//	spriteManager->addSpriteType(ast);
//
//	AnimatedSprite *slides = new AnimatedSprite();
//	slides->setSpriteType(ast);
//
//	PhysicalProperties *slidesProps = new PhysicalProperties();
//	slides->setPhysicalProperties(slidesProps);
//	slidesProps->setX(0);
//	slidesProps->setY(0);
//	slidesProps->setAccelerationX(0);
//	slidesProps->setAccelerationY(0);
//	// WE WILL SET LOTS OF OTHER PROPERTIES ONCE
//	// WE START DOING COLLISIONS AND PHYSICS
//
//	slides->setAlpha(255);
//	slides->setCurrentState(STANDING_FACING_RIGHT);
//
//	spriteManager->addSprite(slides);
//	spriteManager->setPlayer(slides);
//}

/*
loadLevelExample - This method loads the current
level with data. It illustrates all the data
we would need to load ourselves. You should
load your data by reading it from a file.
*/
//void loadLevelExample(Game *game)
//{
//	// FIRST SETUP THE GAME WORLD DIMENSIONS
//	GameWorld *world = game->getWorld();
//	GameGraphics *graphics = game->getGraphics();
//	TextureManager *worldTextureManager = graphics->getWorldTextureManager();
//	Viewport *viewport = world->getViewport();
//	viewport->setViewportWidth(1024);
//	viewport->setViewportHeight(768);
//	viewport->setViewportOffsetX(0);
//	viewport->setViewportOffsetY(0);
//	world->setWorldWidth(1920);
//	world->setWorldHeight(1280);
//
//	// NOW LOAD A TILED BACKGROUND
//	TiledLayer *tiledLayer = new TiledLayer(30, 20, 64, 64, 0, true, viewport, 1920, 1280);
//
//	wchar_t *fileName = constructEmptyWCHAR_TArray(L"textures/world/tiles/grass.bmp");
//	int grassID = worldTextureManager->loadTexture(fileName);
//
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/tiles/wall.bmp");
//	int wallID = worldTextureManager->loadTexture(fileName);
//
//	srand(1);
//
//	for (int i = 0; i < 600; i++)
//	{
//		bool isCollidable = false;
//		int tileIDToUse = grassID;
//		int randomInt = rand() % 100;
//
//		if (randomInt >= 50)
//		{
//			isCollidable = true;
//		}
//
//		randomInt = rand() % 100;
//
//		if (randomInt >= 80)
//		{
//			tileIDToUse = wallID;
//		}
//
//		Tile *tileToAdd = new Tile();
//		tileToAdd->collidable = isCollidable;
//		tileToAdd->textureID = tileIDToUse;
//		tiledLayer->addTile(tileToAdd);
//	}
//	world->addLayer(tiledLayer);
//
//	// AND NOW LET'S MAKE A MAIN CHARACTER SPRITE
//	AnimatedSpriteType *ast = new AnimatedSpriteType();
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_0.bmp");
//	int spriteStandID1 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteStandID1);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_1.bmp");
//	int spriteStandID2 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteStandID2);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_2.bmp");
//	int spriteRunID1 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID1);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_3.bmp");
//	int spriteRunID2 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID2);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_4.bmp");
//	int spriteRunID3 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID3);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_5.bmp");
//	int spriteRunID4 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID4);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_6.bmp");
//	int spriteRunID5 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID5);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_7.bmp");
//	int spriteRunID6 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID6);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_8.bmp");
//	int spriteRunID7 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID7);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_9.bmp");
//	int spriteRunID8 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID8);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_10.bmp");
//	int spriteRunID9 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID9);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_11.bmp");
//	int spriteRunID10 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID10);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_12.bmp");
//	int spriteRunID11 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID11);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_13.bmp");
//	int spriteRunID12 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID12);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_14.bmp");
//	int spriteRunID13 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID13);
//	fileName = constructEmptyWCHAR_TArray(L"textures/world/sprites/zero/zero_0_15.bmp");
//	int spriteRunID14 = worldTextureManager->loadTexture(fileName);
//	ast->addImageID(spriteRunID14);
//
//	// CHANGE THE IMAGE ONCE EVERY 6 FRAMES
//	ast->setAnimationSpeed(6);
//
//	// SIZE OF SPRITE IMAGES
//	ast->setTextureSize(64, 64);
//
//	// NOW LET'S ADD AN ANIMATION STATE
//	// FIRST THE NAME
//	wchar_t *animStateName = constructEmptyWCHAR_TArray(L"RUNNING");
//	ast->addAnimationState(animStateName);
//	vector<int> *standingSequence = new vector<int>();
//
//	//ast->addAnimationFrame(0, spriteStandID1);
//	//ast->addAnimationFrame(0, spriteStandID2);
//	ast->addAnimationFrame(0, spriteRunID1);
//	ast->addAnimationFrame(0, spriteRunID2);
//	ast->addAnimationFrame(0, spriteRunID3);
//	ast->addAnimationFrame(0, spriteRunID4);
//	ast->addAnimationFrame(0, spriteRunID5);
//	ast->addAnimationFrame(0, spriteRunID6);
//	ast->addAnimationFrame(0, spriteRunID7);
//	ast->addAnimationFrame(0, spriteRunID8);
//	ast->addAnimationFrame(0, spriteRunID9);
//	ast->addAnimationFrame(0, spriteRunID10);
//	ast->addAnimationFrame(0, spriteRunID11);
//	ast->addAnimationFrame(0, spriteRunID12);
//	ast->addAnimationFrame(0, spriteRunID13);
//	ast->addAnimationFrame(0, spriteRunID14);
//
//
//	SpriteManager *spriteManager = world->getSpriteManager();
//	spriteManager->addSpriteType(ast);
//
//	AnimatedSprite *player = new AnimatedSprite();
//	player->setSpriteType(ast);
//
//	PhysicalProperties *playerProps = new PhysicalProperties();
//	player->setPhysicalProperties(playerProps);
//	playerProps->setX(10);
//	playerProps->setY(10);
//	playerProps->setAccelerationX(0);
//	playerProps->setAccelerationY(0);
//	// WE WILL SET LOTS OF OTHER PROPERTIES ONCE
//	// WE START DOING COLLISIONS AND PHYSICS
//
//	player->setAlpha(255);
//	player->setCurrentState(STANDING_FACING_RIGHT);
//
//	spriteManager->addSprite(player);
//	spriteManager->setPlayer(player);
//
//}

/*
loadAllLevels - This method will load all of the 
data for the game but will use GameWorld::getLevelFileName(int level)
to figure out which file to read which in turn places
our tiles in specific locations. We will also use getCurrentLevel()

*/
void loadAllLevels(Game *game)
{
	// FIRST SETUP THE GAME WORLD DIMENSIONS
	GameWorld *world = game->getWorld();
	GameGraphics *graphics = game->getGraphics();
	TextureManager *worldTextureManager = graphics->getWorldTextureManager();
	GameText *gameText = game->getText();
	Viewport *viewport = world->getViewport();
	AnimatedSprite *player;
	TiledLayer *backgroundLayer;
	TiledLayer *floorLayer;
	Tile *tileToAdd;
	vector<int> backgroundSetup;
	vector<int> floorSetup;
	vector<int> levelSetup;
	wchar_t *fileName;
	int tileArea;
	int numFloorCols;
	int numFloorRows;
	int currId;

	int mountanBackgroundID;
	int iceBackgroundID;
	int nightBackgroundID;
	int redIceBackgroundID;
	int snowFloorID;
	int iceFloorID;
	int blankFloorID;

	int bigSnowballID;
	int smallSnowballID;

	int gameOverScreenID;

	viewport->setViewportWidth(1024);
	viewport->setViewportHeight(767);//one pixel less to remove camera shake
	viewport->setViewportOffsetX(0);
	viewport->setViewportOffsetY(0);

	//if(!myTilesLoaded)
	//{
		fileName = constructEmptyWCHAR_TArray(L"textures/world/tiles/mountanBackground.bmp");
		mountanBackgroundID = worldTextureManager->loadTexture(fileName); //0

		fileName = constructEmptyWCHAR_TArray(L"textures/world/tiles/iceBackground.bmp");
		iceBackgroundID = worldTextureManager->loadTexture(fileName); //1

		fileName = constructEmptyWCHAR_TArray(L"textures/world/tiles/mountanBackground2.bmp");
		nightBackgroundID = worldTextureManager->loadTexture(fileName); //2

		fileName = constructEmptyWCHAR_TArray(L"textures/world/tiles/iceBackground2.bmp");
		redIceBackgroundID = worldTextureManager->loadTexture(fileName); //3
	
		fileName = constructEmptyWCHAR_TArray(L"textures/world/tiles/snow.bmp");
		snowFloorID = worldTextureManager->loadTexture(fileName); //4

		fileName = constructEmptyWCHAR_TArray(L"textures/world/tiles/ice.bmp");
		iceFloorID = worldTextureManager->loadTexture(fileName); //5

		fileName = constructEmptyWCHAR_TArray(L"textures/world/tiles/blank.bmp");
		blankFloorID = worldTextureManager->loadTexture(fileName); //6

		fileName = constructEmptyWCHAR_TArray(L"textures/world/particles/bigSnowBall.bmp");
		bigSnowballID = worldTextureManager->loadTexture(fileName); //7

		fileName = constructEmptyWCHAR_TArray(L"textures/world/particles/snowBall.bmp");
		smallSnowballID = worldTextureManager->loadTexture(fileName); //8

		fileName = constructEmptyWCHAR_TArray(L"textures/gui/overlays/Game_Over.bmp");
		gameOverScreenID = game->getGraphics()->getGUITextureManager()->loadTexture(fileName);

	//	myTilesLoaded = true;
	//}

	if(world->getCurrentLevel() == 1)
	{
		levelSetup = readCSV(world->getLevelFileName(1));
		backgroundSetup = readCSV(world->getLevelFileName(2));
		floorSetup = readCSV(world->getLevelFileName(3));
	}

	world->setWorldWidth(levelSetup.at(0));
	world->setWorldHeight(levelSetup.at(1));

	tileArea = levelSetup.at(2) * levelSetup.at(3);
	numFloorCols = levelSetup.at(0) / levelSetup.at(6);
	numFloorRows = levelSetup.at(1) / levelSetup.at(7);

	// NOW LOAD A TILED BACKGROUND
	backgroundLayer = new TiledLayer(levelSetup.at(2),
		levelSetup.at(3), 
		levelSetup.at(4),
		levelSetup.at(5),
		2,
		false, 
		viewport, 
		levelSetup.at(0), 
		levelSetup.at(1));

	floorLayer = new TiledLayer(numFloorCols, 
		numFloorRows,
		levelSetup.at(6),
		levelSetup.at(7),
		0,
		true,
		viewport,
		levelSetup.at(0),
		levelSetup.at(1));


	for(int i = 0; i < tileArea; i++)
	{
		currId = backgroundSetup.at(i);

		tileToAdd = new Tile();
		tileToAdd->collidable = false;
		tileToAdd->textureID = currId;
		backgroundLayer->addTile(tileToAdd);
	}

	tileArea = numFloorCols * numFloorRows;

	for(int i = 0; i < tileArea; i++)
	{
		currId = floorSetup.at(i);
		tileToAdd = new Tile();

		if(currId == 4)
		{
			tileToAdd->collidable = true;
			tileToAdd->textureID = snowFloorID;
		}
		else if(currId == 5)
		{
			tileToAdd->collidable = true;
			tileToAdd->textureID = iceFloorID;
		}
		else if(currId == 6)//if blank tile
		{
			tileToAdd->collidable = false;
			tileToAdd->textureID = blankFloorID;
		}

		floorLayer->addTile(tileToAdd);
	}

	world->addLayer(backgroundLayer);
	world->addLayer(floorLayer);

	createPlayer(game);

}

/*
loadWorld - This method should load the data
for the GameWorld's current level. The GameWorld
stores a vector of all level file names. Such
a file would describe how a level would be
constructed.
*/
void GameDataLoader::loadWorld(Game *game)	
{
	// NOTE:	I AM DEMONSTRATING HOW TO LOAD A LEVEL
	//			PROGRAMICALLY. YOU SHOULD DO THIS
	//			USING CSV FILES.

	//loadSlideshow(game);

	loadAllLevels(game);
}

/*
loadGUI - One could use this method to build
the GUI based on the contents of a GUI file.
That way we could change the GUI and add to
it without having to rebuild the project.
*/
void GameDataLoader::loadGUI(Game *game)
{

}


