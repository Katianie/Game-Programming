#pragma once

/*	
	Author: _______________

	GameAI.h

	This class will be used to manage all game bots, and the
	Artificial Intelligence associated with them. Each frame
	we will update the AI state, which should update all bots
	accordingly.

	Students will implement this as part of their projects.
*/

// PREPROCESSOR INCLUDE STATEMENTS
#include "../resource.h"
#include "../stdafx.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/ai/Bot.h"

using namespace std;

class GameAI
{
private:
	vector<Bot*> *myBots;
public:
	// WE'LL DEFINE THESE METHODS LATER
	GameAI(Game *game);

	~GameAI()
	{
	}

	vector<Bot*>* getBots()
	{
		return myBots;
	}

	void update(Game *game);
};