/*	
	Author: _______________

	GamePhysics.h

	This class will be used to manage all game collisions
	and physics. This includes collision detection, and resolution.
	Each frame we will update the collided objects velocities and
	positions accordingly.

	Students will implement this as part of their projects.
*/

#pragma once
#include "../resource.h"
#include "../stdafx.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/world/SpriteManager.h"
#include "../GameEngine/world/AnimatedSprite.h"
#include "../GameEngine/physics/PhysicalProperties.h"
#include "../GameEngine/graphics/GameGraphics.h"
#include "../GameEngine/graphics/TextureManager.h"
#include "../GameEngine/physics/BoundingVolume.h"
#include "../GameEngine/ai/Bot.h"
#include "../GameEngine/ai/GameAI.h"
#include "../GameEngine/world/Viewport.h"
#include "../GameEngine/gui/ScreenGUI.h"
#include "../GameEngine/gui/OverlayImage.h"
#include "WRButtonEventHandler.h"
#include <vector>

using namespace std;

class GamePhysics
{
private:
	AnimatedSprite *myPlayer;
	PhysicalProperties *myPhysicalProperty;
	Viewport *myViewport;
	vector<Bot*> *myBots;
	ScreenGUI *myGui;
	TextureManager *myGUITextureManager;
	OverlayImage *myGameOverImage;
	WRButtonEventHandler *myButtonEventHandler;
	Button *myExitButton;
	Button *myNewGameButton;

	Bot* myCurrBot;
	float myCurrBotX;
	float myCurrBotY;
	float myCurrBotWidth;
	float myCurrBotHeight;
	float myCurrBotVelocity;
	double myCurrBotTime;
	double myCollisionTime;

	int myGameOverID;
	int myExitButtonID;
	int myExitOverButtonID;
	int myNewGameButtonID;
	int myNewGameOverButtonID;
	wchar_t *myExitCommand;
	wchar_t *myNewGameCommand;

public:
	
	GamePhysics(Game *game);

	~GamePhysics()
	{

	}

	void update(Game *game);

};