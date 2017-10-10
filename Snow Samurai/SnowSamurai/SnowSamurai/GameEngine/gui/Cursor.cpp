/*
	Cursor.cpp

	See Cursor.h for a class description.
*/

#include "../stdafx.h"
#include "../GameEngine/gui/Cursor.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/graphics/GameGraphics.h"
#include "../GameEngine/input/GameInput.h"
#include "../GameEngine/os/GameOS.h"
#include "../GameEngine/graphics/RenderList.h"
#include "../GameEngine/graphics/TextureManager.h"
#include <vector>

/*
	Cursor - Default constructor, this method constructs the imageIDs vector
	and sets all numeric variables to zero. This method does not setup a usable
	cursor. To do so, either call mutator methods or use the init method, which
	initializes all necessary parameters at once.
*/
Cursor::Cursor()
{
	imageIDs = new vector<int>();
	x = 0;
	y = 0;
	z = 0;
	alpha = 0;
	width = 0;
	height = 0;
	activeCursorID = 0;
}

/*
	~Cursor - Destructor, it cleans up our vector pointer.
*/
Cursor::~Cursor()	
{
	delete imageIDs;
}

/*
	addRenderItemToRenderList - Called once per frame, this method makes a RenderItem
	representing the current cursor and adds it to the render list. The result being
	that the cursor will be rendered according to its current state. The cursor should
	be rendered last, after the game world and the rest of the GUI.
*/
void Cursor::addRenderItemToRenderList(RenderList *renderList)
{
	renderList->addRenderItem(activeCursorID,
		x,
		y,
		z,
		alpha,
		width,
		height);
}

/*
	initCursor - This method can be used to initialize all important 
	cursor variables at once.
*/
void Cursor::initCursor(vector<int> *initImageIDs,
				int initActiveCursorID,
				int initX,
				int initY,
				int initZ,
				int initAlpha,
				int initWidth,
				int initHeight)
{
	imageIDs = initImageIDs;
	activeCursorID = initActiveCursorID;
	x = initX;
	y = initY;
	z = initZ;
	alpha = initAlpha;
	width = initWidth;
	height = initHeight;
}