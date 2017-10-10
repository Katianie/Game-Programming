#include "GameAI.h"
#include <vector>

#define NUM_BOTS 50

GameAI::GameAI(Game *game)
{
	float rNum = 0.0f;
	myBots = new vector<Bot*>();

	for(int i = 0; i < NUM_BOTS; i++)
	{
		if(i % 2 == 0)
		{
			myBots->push_back(new Bot(game, 7));
		}
		else
		{
			myBots->push_back(new Bot(game, 8));
		}
	}

	for(int i = 0; i < NUM_BOTS; i++)
	{
		rNum = rand() % 7 + 2;
		myBots->at(i)->getPhysicalProperties()->setVelocityX(-rNum);
	}
}


void GameAI::update(Game *game)
{
	for(int i = 0; i < NUM_BOTS; i++)
	{
		myBots->at(i)->update(game);
	}
}