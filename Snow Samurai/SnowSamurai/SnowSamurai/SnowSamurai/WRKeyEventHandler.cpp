/*
	WRKeyEventHandler.cpp

	See WRKeyEventHandler.h for a class description.
*/

#include "../stdafx.h"
#include "../GameEngine/gui/Cursor.h"
#include "../WRKeyEventHandler.h"
#include "../WRButtonEventHandler.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/input/GameInput.h"
#include "../GameEngine/graphics/GameGraphics.h"
#include "../GameEngine/gui/GameGUI.h"
#include "../GameEngine/timer/GameTimer.h"
#include "../GameEngine/gui/ScreenGUI.h"
#include "../GameEngine/game/StringTable.h"
#include "../GameEngine/TechnologyPlugins/WindowsPlugin/WindowsGameTimer.h"
#include "../GameEngine/input/ButtonEventHandler.h"
#include "../GameEngine/world/SpriteManager.h"
#include "../GameEngine/world/AnimatedSprite.h"
#include "../GameEngine/TechnologyPlugins/WindowsPlugin/WindowsGameInput.h"

const int A_KEY = 0X41;
const int B_KEY = 0X42;
const int C_KEY = 0X43;
const int D_KEY = 0X44;

//const int X_KEY = 0X

void WRKeyEventHandler::handleKeyEvents(Game *game)
{
	wchar_t *title;
	wchar_t *counterText;
	GameInput *input = game->getInput();
	GameGraphics *graphics = game->getGraphics();
	ScreenGUI *ingameMenu = game->getGUI()->getScreen(PAUSED_GAME_STATE);
	WindowsGameInput *winGameInput = new WindowsGameInput();
	SpriteManager *spriteManager = game->getWorld()->getSpriteManager();
	AnimatedSprite *player = spriteManager->getPlayer();

	if (game->getGameState() == GAME_PLAYING_GAME_STATE)
	{
		unsigned int aKey = (unsigned int)'A';
		unsigned int dKey = (unsigned int)'D';
		unsigned int sKey = (unsigned int)'S';
		unsigned int wKey = (unsigned int)'W';
		unsigned int spaceKey = (unsigned int)' ';
		unsigned int leftArrowKey = 37;
		unsigned int upArrowKey = 38;
		unsigned int rightArrowKey = 39;
		unsigned int downArrowKey = 40; 
		float incX = 0.0f;
		float incY = 0.0f;
		bool moveViewport = false;
		bool mouseFirstTime = false;

		//State changes
		if(input->isKeyDownForFirstTime(aKey) || input->isKeyDownForFirstTime(leftArrowKey))
		{
			player->setCurrentState(RUNNING_LEFT);
			player->getSpriteType()->setAnimationSpeed(6);
			player->setIsFacingRight(false);
		}
		if(input->isKeyDownForFirstTime(dKey)|| input->isKeyDownForFirstTime(leftArrowKey))
		{
			player->setCurrentState(RUNNING_RIGHT);
			player->getSpriteType()->setAnimationSpeed(6);

			player->setIsFacingRight(true);
		}

		if(input->isKeyDownForFirstTime(spaceKey) || input->isKeyDownForFirstTime(wKey) || input->isKeyDownForFirstTime(upArrowKey))
		{
			if(!player->getIsInAir())
			{
				player->getPhysicalProperties()->incVelocityY(-25.0f);
			}

			player->getBoundingBox()->setY(player->getPhysicalProperties()->getY());
			player->getSpriteType()->setAnimationSpeed(6);

			if(player->getIsFacingRight())
			{
				player->setCurrentState(JUMPING_RIGHT);
			}
			else
			{
				player->setCurrentState(JUMPING_LEFT);
			}
		}


		//Key pressing and holding
		if (input->isKeyDown(aKey) || input->isKeyDown(leftArrowKey))
		{
			incX = -3.0f;

			player->getPhysicalProperties()->setVelocityX(incX);
			player->getBoundingBox()->setX(player->getPhysicalProperties()->getX());

			moveViewport = true;
			player->setIsFacingRight(false);
		}
		else if (input->isKeyDown(dKey) || input->isKeyDown(rightArrowKey))
		{
			incX = 3.0f;

			player->getPhysicalProperties()->setVelocityX(incX);
			player->getBoundingBox()->setX(player->getPhysicalProperties()->getX());

			moveViewport = true;
			player->setIsFacingRight(true);
		}
		else
		{
			player->getPhysicalProperties()->setVelocityX(0.0f);

			moveViewport = false;
		}

		//Attack
		if(winGameInput->isLeftMouseButtonDown())
		{
			if(player->getIsFacingRight())
			{
				player->setCurrentState(ATTACKING_RIGHT);
			}
			else
			{
				player->setCurrentState(ATTACKING_LEFT);
			}

			player->setJustAttacked(true);
		}


		if (input->isKeyDownForFirstTime(VK_ESCAPE))
		{
			GameGUI *gui = game->getGUI();

			Button *newGameButton = new Button();
			Button *controlsButton = new Button();
			Button *helpButton = new Button();
			Button *resumeGameButton = new Button();
			Button *aboutButton = new Button();
			Button *exitButton = new Button();

			StringTable *guiStringTable = graphics->getGUITextureManager()->getStringTable();
			int menuID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/overlays/Ingame_Menu.bmp");
			int exitButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/exit_game.bmp");
			int exitOverButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/exit_game_mo.bmp");
			int newGameButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/new_game.bmp");
			int newGameOverButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/new_game_mo.bmp");
			int controlsButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/controls.bmp");
			int controlsOverButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/controls_mo.bmp");
			int helpButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/help.bmp");
			int helpOverButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/help_mo.bmp");
			int aboutButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/about.bmp");
			int aboutOverButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/about_mo.bmp");
			int resumeGameButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/resume_game.bmp");
			int resumeGameOverButtonID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/buttons/resume_game_mo.bmp");

			wchar_t *exitCommand = constructEmptyWCHAR_TArray(L"Exit");
			wchar_t *newGameCommand = constructEmptyWCHAR_TArray(L"New Game");
			wchar_t *controlsCommand = constructEmptyWCHAR_TArray(L"Controls");
			wchar_t *helpCommand = constructEmptyWCHAR_TArray(L"Help");
			wchar_t *aboutCommand = constructEmptyWCHAR_TArray(L"About");
			wchar_t *resumeGameCommand = constructEmptyWCHAR_TArray(L"Start");

			OverlayImage *imageToAdd = new OverlayImage();

			//Construct the overlay image for the ingame menu
			imageToAdd->x = 0;
			imageToAdd->y = 0;
			imageToAdd->z = 0;
			imageToAdd->alpha = 200;
			imageToAdd->width = 1024;
			imageToAdd->height = 768;
			imageToAdd->imageID = menuID;

			//add the background image to the menu
			ingameMenu->addOverlayImage(imageToAdd);

			//constuct the quit button
			newGameButton->initButton(newGameButtonID, 
				newGameOverButtonID,
				400,
				100,
				0,
				255,
				200,
				100,
				false,
				newGameCommand);

			controlsButton->initButton(controlsButtonID, 
				controlsOverButtonID,
				400,
				200,
				0,
				255,
				200,
				100,
				false,
				controlsCommand);

			resumeGameButton->initButton(resumeGameButtonID, 
				resumeGameOverButtonID,
				400,
				300,
				0,
				255,
				200,
				100,
				false,
				resumeGameCommand);

			helpButton->initButton(helpButtonID, 
				helpOverButtonID,
				400,
				400,
				0,
				255,
				200,
				100,
				false,
				helpCommand);

			aboutButton->initButton(aboutButtonID, 
				aboutOverButtonID,
				400,
				500,
				0,
				255,
				200,
				100,
				false,
				aboutCommand);

			exitButton->initButton(exitButtonID, 
				exitOverButtonID,
				400,
				600,
				0,
				255,
				200,
				100,
				false,
				exitCommand);

			//add the button to the menu
			ingameMenu->addButton(newGameButton);
			ingameMenu->addButton(controlsButton);
			ingameMenu->addButton(resumeGameButton);
			ingameMenu->addButton(helpButton);
			ingameMenu->addButton(aboutButton);
			ingameMenu->addButton(exitButton);

			gui->addScreenGUI(ingameMenu);

			WRButtonEventHandler *eventHandler = new WRButtonEventHandler();
			gui->registerButtonEventHandler((ButtonEventHandler*)eventHandler);

			game->changeGameState(PAUSED_GAME_STATE);
		}


		if (moveViewport)
		{
			GameWorld *world = game->getWorld();
			Viewport *viewport = world->getViewport();
			
			viewport->moveViewport( player->getPhysicalProperties()->getVelocityX() + incX,
									incY,
									world->getWorldWidth(),
									world->getWorldHeight());

		}
		else
		{
			if(player->getIsFacingRight())
			{
				player->setCurrentState(STANDING_FACING_RIGHT);
			}
			else
			{
				player->setCurrentState(STANDING_FACING_LEFT);
			}

			//player->getSpriteType()->setAnimationSpeed(60);
		}

	}
	else if(game->getGameState() == PAUSED_GAME_STATE)
	{
		if(input->isKeyDownForFirstTime(VK_ESCAPE))
		{
			game->changeGameState(GAME_PLAYING_GAME_STATE);
		}
	}


	if ((input->isKeyDownForFirstTime(C_KEY)) && input->isKeyDown(VK_SHIFT))
	{
		Cursor *cursor = game->getGUI()->getCursor();
		StringTable *guiStringTable = graphics->getGUITextureManager()->getStringTable();
		int greenCursorID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/cursor/sword_cursor.bmp");
		int redCursorID = guiStringTable->getIndexOfStringFromStringTable(L"textures/gui/cursor/red_sword_cursor.bmp");
		int currentCursorID = cursor->getActiveCursorID();

		if (currentCursorID == greenCursorID)
		{
			cursor->setActiveCursorID(redCursorID);
		}
		else
		{
			cursor->setActiveCursorID(greenCursorID);
		}
	}

	if (input->isKeyDown(VK_HOME))
	{
		WindowsGameTimer *timer = (WindowsGameTimer*)game->getTimer();
		int fps = timer->getTargetFPS();
		if (fps < 100)
			timer->setTargetFPS(fps + 1);
	}
	else if (input->isKeyDown(VK_END))
	{
		WindowsGameTimer *timer = (WindowsGameTimer*)game->getTimer();
		int fps = timer->getTargetFPS();
		if (fps > 1)
			timer->setTargetFPS(fps - 1);
	}


}