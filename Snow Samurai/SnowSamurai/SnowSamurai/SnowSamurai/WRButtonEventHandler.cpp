/*
	WRButtonEventHandler.cpp

	See WRButtonEventHandler.h for a class description.
*/

#include "../stdafx.h"
#include "../GameEngine/game/Game.h"
#include "../WRButtonEventHandler.h"

void WRButtonEventHandler::handleButtonEvents(Game *game, 
	wchar_t *command)
{
	// SOMEONE PRESSED THE Exit BUTTON, SO CHANGE THE
	// Game State TO END THE APPLICATION
	if (wcscmp(command, L"Exit") == 0)
	{
		int result = MessageBox(NULL,L"Are you sure you want to exit?", L"Are you sure?", MB_YESNO);

		if(result == IDYES)
		{
			if(game->isGameInProgress())
			{
				game->setAlreadyLoaded(false);
				game->setGameInProgress(false);
				game->changeGameState(SPLASH_SCREEN_GAME_STATE);
			}
			else
			{
				game->changeGameState(EXIT_GAME_STATE);
			}
		}

	}
	else if (wcscmp(command, L"Start") == 0)
	{
		game->changeGameState(GAME_PLAYING_GAME_STATE);
	}
	else if (wcscmp(command, L"New Game") == 0)
	{
		if(game->isGameInProgress())
		{
			int result = MessageBox(NULL,L"Are you sure you want to start a new game?", L"New Game?", MB_YESNO);

			if(result == IDYES)
			{
				game->setAlreadyLoaded(false);
				game->changeGameState(GAME_PLAYING_GAME_STATE);
			}
		}
		else
		{
			game->setAlreadyLoaded(false);
			game->changeGameState(GAME_PLAYING_GAME_STATE);
		}
	}
	else if (wcscmp(command, L"Controls") == 0)
	{
		game->changeGameState(CONTROLS_MENU_GAME_STATE);
	}
	else if (wcscmp(command, L"Help") == 0)
	{
		game->changeGameState(HELP_MENU_GAME_STATE);
	}
	else if (wcscmp(command, L"About") == 0)
	{
		game->changeGameState(ABOUT_MENU_GAME_STATE);
	}
	else if (wcscmp(command, L"Quit") == 0)
	{
		if(game->isGameInProgress())
		{
			game->changeGameState(PAUSED_GAME_STATE);
		}
		else
		{
			game->setAlreadyLoaded(false);
			game->changeGameState(SPLASH_SCREEN_GAME_STATE);
		}
	}

}