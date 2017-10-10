/*
	GameInput.cpp

	See GameInput.h for a class description.
*/

#include "../stdafx.h"
#include "../GameEngine/gui/Button.h"
#include "../GameEngine/gui/Cursor.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/gui/GameGUI.h"
#include "../GameEngine/input/GameInput.h"
#include "../GameEngine/os/GameOS.h"
#include "../GameEngine/input/KeyEventHandler.h"
#include "../GameEngine/gui/ScreenGUI.h"
#include "../GameEngine/TechnologyPlugins/WindowsPlugin/WindowsGameOS.h"

/*
	GameInput - Default constructor, it will initialize the input state variables.
*/
GameInput::GameInput()
{
	initInputState();
}

/*
	~GameInput - Destructor, it will destroy the mousePoint pointer.
*/
GameInput::~GameInput()
{
}

/*
	initInputState - This method empties all of the data about
	key presses. This would be called at the start of the application.
*/
void GameInput::initInputState()
{
	// RESET ALL KEYS
	for (int i = 0; i < 256; i++)
	{
		inputState[i].isFirstPress	= false;
		inputState[i].isPressed		= false;
		inputState[i].wasHeldDown	= false;
	}
}
