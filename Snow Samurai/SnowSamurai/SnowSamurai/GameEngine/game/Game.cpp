/*
	Game.cpp

	See Game.h for a class description.
*/
#include "../stdafx.h"
#include "../GameEngine/gui/Button.h"
#include "../GameEngine/gui/Cursor.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/ai/GameAI.h"
#include "../GameEngine/dataLoader/GameDataLoader.h"
#include "../GameEngine/graphics/GameGraphics.h"
#include "../GameEngine/gui/GameGUI.h"
#include "../GameEngine/input/GameInput.h"
#include "../GameEngine/os/GameOS.h"
#include "../GameEngine/physics/GamePhysics.h"
#include "../GameEngine/text/GameText.h"
#include "../GameEngine/timer/GameTimer.h"
#include "../GameEngine/world/GameWorld.h"
#include "../GameEngine/gui/ScreenGUI.h"
#include "../GameEngine/text/TextGenerator.h"
#include "../GameEngine/dataLoader/TextFileWriter.h"

/*
constructEmptyWCHAR_TArray - This is a simple helper
method for constructing a wchar_t object on the heap
and filling it in with some literal character data.
*/
wchar_t* constructEmptyWCHAR_TArray(LPCWSTR initChars)
{
	size_t initSize = wcslen(initChars) + 2;
	wchar_t *initArray = new wchar_t[initSize];
	initArray[0] = '\0';
	swprintf(initArray, initSize, initChars);
	return initArray;
}
/*
Game  - Constructor, this method begins the 
construction and loading of all major game objects. 
This method only needs to be called once, when the 
application is first started.

NOTE: This constructor will not initialize the custom
game objects: graphics, input, os, & timer.
These must be constructed separately and fed to this
object using the init method. Doing it this way allows for
more platform independence.
*/
Game::Game()
{
	// DEFAULT GAME STATE
	gameState = SPLASH_SCREEN_GAME_STATE;

	// CONSTRUCT BASIC GAME OBJECTS

	//physics		= new GamePhysics();
	dataLoader	= new GameDataLoader();
	gui			= new GameGUI();
	text		= new GameText();
	world		= new GameWorld();
	//ai			= new GameAI(this);

	alreadyLoaded = false;
	gameInProgress = false;

	// OUR DEBUGGING LOG FILE
	debugLogFileName = new wchar_t[30];
	debugLogFileName[0] = '\0';
	wcscat(debugLogFileName, L"DebugOutput.txt");
	writer = new TextFileWriter();
	writer->initFile(debugLogFileName);

	// NOTE THAT graphics, input, os, & timer
	// ARE CUSTOM GAME OBJECTS. DEPENDING ON WHAT TECHNOLOGY 
	// IS TO BE USED THESE OBJECT SHOULD BE CONSTRUCTED
	// AND THEN FED TO THIS Game USING THE init METHOD
}

/*
init - This method is to be used to feed this Game
the constructed, technology-specific, objects. In addition,
In addition, this method requires gameDataPathName, which
specifies the location of the game design files.
*/
void Game::init(wchar_t *initGameDataPathName,
	GameGraphics *initGraphics,
	GameOS *initOS,
	GameInput *initInput,
	GameTimer *initTimer)
{
	// INITIALIZE ALL OF THE GAME'S CUSTOM OBJECTS
	graphics = initGraphics;
	os = initOS;
	input = initInput;
	timer = initTimer;

	// AND NOW LOAD THE GAME GUI
	gameDataPathName = initGameDataPathName;
	dataLoader->loadGUI(this);
}

/*
~Game - Desctructor, it recovers memory allocated for
the game objects. If Game is deleted, the application
is closing, so everything should be cleaned up.
*/
Game::~Game() 
{
	delete ai;
	delete dataLoader;
	delete graphics;
	delete gui;
	delete input;
	delete os;
	delete physics;
	delete text;
	delete timer;
	delete world;
	delete writer;
	delete debugLogFileName;
}

void Game::changeGameState(int initGameState)
{
	if (initGameState == GAME_PLAYING_GAME_STATE)
	{
		if(alreadyLoaded == false)
		{
			//diffrent states for difrent levels?
			world->loadCurrentLevel(this, 1);
			ai	= new GameAI(this);
			physics	= new GamePhysics(this);
			gui->updateGUIState(0, 0, GAME_PLAYING_GAME_STATE);
			alreadyLoaded = true;
		}
		else
		{
			gui->updateGUIState(0, 0, GAME_PLAYING_GAME_STATE);
		}

		gameInProgress = true;
	}
	else if(initGameState == CONTROLS_MENU_GAME_STATE)
	{
		gui->updateGUIState(0, 0, CONTROLS_MENU_GAME_STATE);
	}
	else if(initGameState == HELP_MENU_GAME_STATE)
	{
		gui->updateGUIState(0, 0, HELP_MENU_GAME_STATE);
	}
	else if(initGameState == ABOUT_MENU_GAME_STATE)
	{
		gui->updateGUIState(0, 0, ABOUT_MENU_GAME_STATE);
	}
	else if(initGameState == PAUSED_GAME_STATE)
	{
		gui->updateGUIState(0, 0, PAUSED_GAME_STATE);
	}
	else if( initGameState == GAME_OVER_GAME_STATE)
	{
		gui->updateGUIState(0, 0, GAME_OVER_GAME_STATE);
	}
	else if (initGameState == SPLASH_SCREEN_GAME_STATE)
	{
		world->unloadCurrentLevel(this);
		gameInProgress = false;
	}


	gameState = initGameState;
}

/*
killGameApplication - This method changes the game loop
control such that it will not iterate again. In addition,
it posts a Windows message to close the application. The
result is that the application will exit after the current
game loop completes.
*/
void Game::killGameApplication()
{
	// END THE GAME LOOP
	changeGameState(EXIT_GAME_STATE);

	// AND KILL THE WINDOW
	os->killApp();
}

/*
reloadAllDevices - Windows applications must cooperate
with other running applications, so when someone hits
ALT-TAB and switches from a full-screen game, it might
lose ownership of the graphics card. This method can
be called when a full-screen application retains ownership
of all necessary resources such that all necessary
data (like textures, sound, music, etc.) can be reloaded.
*/
void Game::reloadAllDevices()
{
	graphics->reloadGraphics();

	// WE MIGHT ADD MORE LATER
}

/*
runGameLoop - This is the game loop management method.
It runs continuously while the game is active. Once per
frame it instructs the major game objects to get
user input, record user input, update the GUI state, 
update the sprites' states using AI and input, perform
collision detection and resolution (physics), render
the screen, etc.

This loop is timed such that everything is kept to a 
consistent framerate, thus the game should run 
consistently on all machines.
*/
void Game::runGameLoop()
{
	// FIRST PROFILE?
	bool firstTimeThroughLoop = true;

	// LET'S START THE TIMER FROM SCRATCH
	timer->resetTimer();

	// KEEP RENDERING UNTIL SOMEONE PULLS THE PLUG
	while(isGameActive())
	{
		// MOVE ALONG WINDOWS MESSAGES, THIS ALLOWS
		// US TO GET USER INPUT
		os->processOSMessages();

		// GET USER INPUT AND UPDATE GAME, GUI, OR PLAYER
		// STATE OR WHATEVER IS NECESSARY
		input->processInput(this);

		// USE THE INPUT TO UPDATE THE GAME
		this->processGameData();

		// AND RENDER THE GAME
		graphics->renderGame(this);
	}

	// SHUTDOWN AND RELEASE ALL RESOURCES
	shutdownEverything();
}

/*
processGameLogic - The code inside this method progresses the
game state forward. It contains the game logic and should only
be executed when the game is in-progress.
*/
void Game::processGameLogic()
{
	// NOW CORRECT FOR COLLISION DETECTION
	physics->update(this);

	// UPDATE THE WORLD, INCLUDING SPRITE AND
	// PARTICLE POSITIONS, FACTORING IN INPUT
	// AND AI
	world->update(this);

	// NOW PERFORM ALL AI
	ai->update(this);
}

/*
processGameData - This method directs game logic to be
executed or not, depending on the game state, it addition
it triggers the building or render lists and game loop
timing each frame.
*/
void Game::processGameData()
{
	// WE ONLY PERFORM GAME LOGIC IF THE GAME
	// IS IN PROGRESS
	if (gameState == GAME_PLAYING_GAME_STATE)
	{
		this->processGameLogic();

		// UPDATE TEXT
		TextGenerator *tg = text->getTextGenerator();
		tg->updateText(this);
	}
	else if (gameState == EXIT_GAME_STATE)
	{
		this->killGameApplication();
	}

	

	// BUILD THE RENDER LISTS
	graphics->fillRenderLists(this);

	// KEEP THE FRAME RATE CONSISTENT
	timer->timeGameLoop();
}

/*
shutdownEverything - This method should be called after
the game application has chosen to close. It will res
ult
in the releasing of all game resources.
*/
void Game::shutdownEverything()
{
	graphics->shutdownGraphics();
	input->shutdownInput();

	// WE MAY SHUTDOWN OTHER THINGS HERE LIKE SOUND & MUSIC
	// RESOURCES IN THE FUTURE
}

/*
writeOutput - This method writes text to the debugging log file.
*/
void Game::writeOutput(wchar_t *output)	
{	
	writer->writeText(output);
}

float Game::randFloat(float low, float high)
{
	float temp;
	srand(time(NULL));

	/* swap low & high around if the user makes no sense */
	if (low > high)
	{
		temp = low;
		low = high;
		high = temp;
	}

	/* calculate the random number & return it */
	temp = ( rand() / (static_cast<float>(RAND_MAX) + 1.0) ) * ( high - low ) + low;

	return temp;
}