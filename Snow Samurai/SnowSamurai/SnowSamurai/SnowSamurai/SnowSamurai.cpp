/*
	WorldRenderingGame.cpp

	This is a test game application, a game that
	demonstrates use of	the game engine simply
	for rendering images and text as well as for reacting to
	key presses and button cicks. Students will extend
	this for their projects, gradually adding other components,
	like additional gui controls, tiling, sprites, collision 
	detection, etc.
 */

#include "../stdafx.h"

// GAME OBJECT INCLUDES
#include "../GameEngine/gui/Cursor.h"
#include "../GameEngine/gui/ScreenGUI.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/graphics/GameGraphics.h"
#include "../GameEngine/gui/GameGUI.h"
#include "../GameEngine/input/GameInput.h"
#include "../GameEngine/os/GameOS.h"
#include "../GameEngine/text/GameText.h"

// EMPTY GAME INCLUDES
#include "WRButtonEventHandler.h"
#include "WRKeyEventHandler.h"
#include "WRTextGenerator.h"

// WINDOWS PLATFORM INCLUDES
#include "../GameEngine/TechnologyPlugins/WindowsPlugin/WindowsGameOS.h"
#include "../GameEngine/TechnologyPlugins/WindowsPlugin/WindowsGameInput.h"
#include "../GameEngine/TechnologyPlugins/WindowsPlugin/WindowsGameTimer.h"

// DIRECTX INCLUDES
#include "../GameEngine/TechnologyPlugins/DirectXPlugin/DirectXGraphics.h"
#include "../GameEngine/TechnologyPlugins/DirectXPlugin/DirectXTextureManager.h"

// METHODS OTHER THAN WinMain DEFINED BELOW
wchar_t*	constructEmptyWCHAR_TArray(LPCWSTR initChars);
void		initWRgui(Game *game);

/*
	WinMain - This is the application's starting point. In this
	method we will construct a Game object, then construct all the
	custom data for our game, and then initialize the Game with
	our custom data. We'll then start the game loop.
*/
int WINAPI WinMain(HINSTANCE hInstance,
                   HINSTANCE hPrevInstance,
                   LPSTR lpCmdLine,
                   int nCmdShow)
{
	// USE WINDOWED MODE (ONE LESS HEADACHE)
	bool fullscreen = false;

	// CREATE A GAME
	Game *worldRenderingGame = new Game();

	// SPECIFY THE DIRECTORY WHERE ALL GAME DESIGN FILES
	// ARE TO BE LOADED FROM
	wchar_t *gameDataPathName = constructEmptyWCHAR_TArray(L"design/");

	// WE'RE USING THE WINDOWS PLATFORM, SO MAKE A CUSTOM
	// GameOS OBJECT (WindowsGameOS), FOR SOME WINDOWS
	// PLATFORM STUFF, INCLUDING A Window OF COURSE
	wchar_t *gameTitle = constructEmptyWCHAR_TArray(L"Snow Samurai");
	WindowsGameOS *os = new WindowsGameOS(	hInstance, 
											nCmdShow,
											fullscreen,
											gameTitle,
											worldRenderingGame);

	// RENDERING WILL BE DONE USING DirectX
	DirectXGraphics *graphics = new DirectXGraphics(worldRenderingGame);
	graphics->init();
	graphics->initGraphics(os->getWindowHandle(), fullscreen);
	graphics->initTextFont(20);

	// WE'LL USE WINDOWS PLATFORM METHODS FOR GETTING INPUT
	WindowsGameInput *input = new WindowsGameInput();

	// AND THE TIMER
	WindowsGameTimer *timer = new WindowsGameTimer();

	// NOW INITIALIZE THE Game WITH ALL THE
	// PLATFORM AND GAME SPECIFIC DATA WE JUST CREATED
	worldRenderingGame->init(gameDataPathName,
						(GameGraphics*)graphics,
						(GameOS*)os,
						(GameInput*)input,
						(GameTimer*)timer);

	// LOAD OUR CUSTOM TEXT GENERATOR, WHICH DRAWS
	// TEXT ON THE SCREEN EACH FRAME
	WRTextGenerator *textGenerator = new WRTextGenerator();
	textGenerator->initText(worldRenderingGame);
	worldRenderingGame->getText()->setTextGenerator((TextGenerator*)textGenerator);

	// LOAD THE GUI STUFF, NOTE THAT THIS SHOULD REALLY
	// BE DONE FROM A FILE, NOT HARD CODED
	initWRgui(worldRenderingGame);

	// SPECIFY WHO WILL HANDLE BUTTON EVENTS
	WRButtonEventHandler *eventHandler = new WRButtonEventHandler();
	GameGUI *gui = worldRenderingGame->getGUI();
	gui->registerButtonEventHandler((ButtonEventHandler*)eventHandler);

	// SPECIFY WHO WILL HANDLE KEY EVENTS
	WRKeyEventHandler *keyHandler = new WRKeyEventHandler();
	input->registerKeyHandler((KeyEventHandler*)keyHandler);

	GameWorld *world = worldRenderingGame->getWorld();
	wchar_t *levelFile = constructEmptyWCHAR_TArray(L"design/Level1_Setup.csv");
	world->addLevelFileName(levelFile);

	levelFile = constructEmptyWCHAR_TArray(L"design/Level1_Background.csv");
	world->addLevelFileName(levelFile);

	levelFile = constructEmptyWCHAR_TArray(L"design/Level1_Floor.csv");
	world->addLevelFileName(levelFile);

	// START THE GAME LOOP
	worldRenderingGame->runGameLoop();

	return 0;
}

/*
	initWRgui - This method builds a GUI for the Empty Game application.
	Note that we load all the GUI components from this method, including
	the ScreenGUI with Buttons and Overlays and the Cursor.
*/
void initWRgui(Game *game)
{
	GameGraphics *graphics = game->getGraphics();
	GameGUI *gui = game->getGUI();
	DirectXTextureManager *guiTextureManager = (DirectXTextureManager*)graphics->getGUITextureManager();

	// COLOR USED FOR RENDERING TEXT
	graphics->setFontColor(200, 100, 200);

	// COLOR KEY - COLOR TO BE IGNORED WHEN LOADING AN IMAGE
	graphics->setColorKey(96, 128, 224);

	// SETUP THE CURSOR
	vector<int> *imageIDs = new vector<int>();

	// - FIRST LOAD THE GREEN CURSOR IMAGE
	wchar_t *fileName = constructEmptyWCHAR_TArray(L"textures/gui/cursor/sword_cursor.bmp");
	int imageID = guiTextureManager->loadTexture(fileName);
	imageIDs->push_back(imageID);

	// - AND NOW THE RED ONE
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/cursor/red_sword_cursor.bmp");
	imageID = guiTextureManager->loadTexture(fileName);
	imageIDs->push_back(imageID);

	//LOAD IN NEW GAME BUTTON AND RESUME BUTTON BECAUSE
	//THEY ARE NOT LOADED INTO THE GAME YET
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/new_game.bmp");
	imageID = guiTextureManager->loadTexture(fileName);
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/new_game_mo.bmp");
	imageID = guiTextureManager->loadTexture(fileName);
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/resume_game.bmp");
	imageID = guiTextureManager->loadTexture(fileName);
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/resume_game_mo.bmp");
	imageID = guiTextureManager->loadTexture(fileName);

	// - NOW BUILD AND LOAD THE CURSOR
	Cursor *cursor = new Cursor();
	cursor->initCursor(	imageIDs,
						*(imageIDs->begin()),
						0,
						0,
						0,
						255,
						32,
						32);
	gui->setCursor(cursor);

	//Create back button to be used in all menus
	Button *quitButton = new Button();
	wchar_t *quitCommand = constructEmptyWCHAR_TArray(L"Quit");
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/quit.bmp");
	int quitTextureID = guiTextureManager->loadTexture(fileName);
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/quit_mo.bmp");
	int quitmoTextureID = guiTextureManager->loadTexture(fileName);
	

	quitButton = new Button();
	quitButton->initButton(quitTextureID, 
							quitmoTextureID,
							0,
							0,
							0,
							255,
							200,
							100,
							false,
							quitCommand);

	// NOW LET'S LOAD A GUI SCREEN, BE CAREFUL TO ADD
	// THESE IN THE PROPER ORDER IF YOU EVER ADD MORE THAN ONE
	ScreenGUI *screenGUI = new ScreenGUI();
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/overlays/Snow_Samurai_Splash.bmp");
	imageID = guiTextureManager->loadTexture(fileName);
	OverlayImage *imageToAdd = new OverlayImage();
	imageToAdd->x = 0;
	imageToAdd->y = 0;
	imageToAdd->z = 0;
	imageToAdd->alpha = 200;
	imageToAdd->width = 1024;
	imageToAdd->height = 768;
	imageToAdd->imageID = imageID;
	screenGUI->addOverlayImage(imageToAdd);

	gui->addScreenGUI(screenGUI);

	// AND LET'S ADD AN EXIT BUTTON
	Button *buttonToAdd = new Button();

	// - GET THE BUTTON COMMAND AND IMAGE IDs
	wchar_t *buttonCommand = constructEmptyWCHAR_TArray(L"Exit");
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/exit_game.bmp");
	int normalTextureID = guiTextureManager->loadTexture(fileName);
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/exit_game_mo.bmp");
	int mouseOverTextureID = guiTextureManager->loadTexture(fileName);

	// - INIT THE BUTTON
	buttonToAdd->initButton(normalTextureID, 
							mouseOverTextureID,
							412,
							700,
							0,
							255,
							200,
							100,
							false,
							buttonCommand);

	// AND NOW LOAD IT INTO A ScreenGUI
	screenGUI->addButton(buttonToAdd);

	// AND LET'S ADD A START BUTTON
	buttonToAdd = new Button();

	// - GET THE BUTTON COMMAND AND IMAGE IDs
	buttonCommand = constructEmptyWCHAR_TArray(L"Start");
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/start_game.bmp");
	normalTextureID = guiTextureManager->loadTexture(fileName);
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/start_game_mo.bmp");
	mouseOverTextureID = guiTextureManager->loadTexture(fileName);

	// - INIT THE BUTTON
	buttonToAdd->initButton(normalTextureID, 
							mouseOverTextureID,
							412,
							300,
							0,
							255,
							200,
							100,
							false,
							buttonCommand);

	// AND NOW LOAD IT INTO A ScreenGUI
	screenGUI->addButton(buttonToAdd);

	//ADD A CONTROLS BUTTON
	buttonToAdd = new Button();

	// - GET THE BUTTON COMMAND AND IMAGE IDs
	buttonCommand = constructEmptyWCHAR_TArray(L"Controls");
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/controls.bmp");
	normalTextureID = guiTextureManager->loadTexture(fileName);
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/controls_mo.bmp");
	mouseOverTextureID = guiTextureManager->loadTexture(fileName);

	// - INIT THE BUTTON
	buttonToAdd->initButton(normalTextureID, 
							mouseOverTextureID,
							412,
							400,
							0,
							255,
							200,
							100,
							false,
							buttonCommand);

	// AND NOW LOAD IT INTO A ScreenGUI
	screenGUI->addButton(buttonToAdd);

	//ADD A HELP BUTTON
	buttonToAdd = new Button();

	// - GET THE BUTTON COMMAND AND IMAGE IDs
	buttonCommand = constructEmptyWCHAR_TArray(L"Help");
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/help.bmp");
	normalTextureID = guiTextureManager->loadTexture(fileName);
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/help_mo.bmp");
	mouseOverTextureID = guiTextureManager->loadTexture(fileName);

	// - INIT THE BUTTON
	buttonToAdd->initButton(normalTextureID, 
							mouseOverTextureID,
							412,
							500,
							0,
							255,
							200,
							100,
							false,
							buttonCommand);

	// AND NOW LOAD IT INTO A ScreenGUI
	screenGUI->addButton(buttonToAdd);

	//ADD AN ABOUT BUTTON
	buttonToAdd = new Button();

	// - GET THE BUTTON COMMAND AND IMAGE IDs
	buttonCommand = constructEmptyWCHAR_TArray(L"About");
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/about.bmp");
	normalTextureID = guiTextureManager->loadTexture(fileName);
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/buttons/about_mo.bmp");
	mouseOverTextureID = guiTextureManager->loadTexture(fileName);

	// - INIT THE BUTTON
	buttonToAdd->initButton(normalTextureID, 
							mouseOverTextureID,
							412,
							600,
							0,
							255,
							200,
							100,
							false,
							buttonCommand);

	// AND NOW LOAD IT INTO A ScreenGUI
	screenGUI->addButton(buttonToAdd);

	// NOW ADD THE IN-GAME GUI
	screenGUI = new ScreenGUI();
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/overlays/Ingame_Menu.bmp");
	imageID = guiTextureManager->loadTexture(fileName);
	gui->addScreenGUI(screenGUI);

	//Add the screen for the controls
	screenGUI = new ScreenGUI();
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/overlays/Controls.bmp");
	imageID = guiTextureManager->loadTexture(fileName);
	imageToAdd = new OverlayImage();
	imageToAdd->x = 0;
	imageToAdd->y = 0;
	imageToAdd->z = 0;
	imageToAdd->alpha = 200;
	imageToAdd->width = 1024;
	imageToAdd->height = 768;
	imageToAdd->imageID = imageID;
	screenGUI->addOverlayImage(imageToAdd);

	//add the quit button for the controls screen
	screenGUI->addButton(quitButton);

	gui->addScreenGUI(screenGUI);

	//Add the screen for Help
	screenGUI = new ScreenGUI();
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/overlays/Help.bmp");
	imageID = guiTextureManager->loadTexture(fileName);
	imageToAdd = new OverlayImage();
	imageToAdd->x = 0;
	imageToAdd->y = 0;
	imageToAdd->z = 0;
	imageToAdd->alpha = 200;
	imageToAdd->width = 1024;
	imageToAdd->height = 768;
	imageToAdd->imageID = imageID;
	screenGUI->addOverlayImage(imageToAdd);

	//add the quit button for the help screen
	screenGUI->addButton(quitButton);

	//add the screen we just created to the list
	gui->addScreenGUI(screenGUI);

	//Add the screen for About
	screenGUI = new ScreenGUI();
	fileName = constructEmptyWCHAR_TArray(L"textures/gui/overlays/About.bmp");
	imageID = guiTextureManager->loadTexture(fileName);
	imageToAdd = new OverlayImage();
	imageToAdd->x = 0;
	imageToAdd->y = 0;
	imageToAdd->z = 0;
	imageToAdd->alpha = 200;
	imageToAdd->width = 1024;
	imageToAdd->height = 768;
	imageToAdd->imageID = imageID;
	screenGUI->addOverlayImage(imageToAdd);

	//add the quit button for the about screen
	screenGUI->addButton(quitButton);

	//add the screen we just created to the list
	gui->addScreenGUI(screenGUI);

	//add blank screen gui for the pause menu(constucted elsewhere)
	gui->addScreenGUI(new ScreenGUI());

	//blank screen for game over 
	gui->addScreenGUI(new ScreenGUI());

}