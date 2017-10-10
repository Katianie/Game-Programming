/*	
	AnimatedSprite.h

	This class represents a sprite that can can
	be used to animate a game character or object.
*/

#pragma once
#include "../resource.h"
#include "../stdafx.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/world/AnimatedSpriteType.h"
#include "../GameEngine/physics/BoundingVolume.h"
#include "../GameEngine/physics/CollidableObject.h"
#include "../GameEngine/physics/PhysicalProperties.h"
#include "../GameEngine/world/Viewport.h"
#include <vector>

// WE COULD KEEP TRACK OF ANIMATION STATES
// USING CONSTANTS IF WE LIKE
const int STANDING_FACING_RIGHT = 0;
const int RUNNING_RIGHT = 1;
const int STANDING_FACING_LEFT = 2;
const int RUNNING_LEFT = 3;
const int JUMPING_RIGHT = 4;
const int JUMPING_LEFT = 5;
const int ATTACKING_RIGHT = 6;
const int ATTACKING_LEFT = 7;


class AnimatedSprite : public CollidableObject
{
private:
	// SPRITE TYPE FOR THIS SPRITE. THE SPRITE TYPE IS THE ID
	// OF AN AnimatedSpriteType OBJECT AS STORED IN THE SpriteManager
	AnimatedSpriteType *spriteType;

	// TRANSPARENCY/OPACITY
	int alpha;

	// ANIMATION SEQUENCE CURRENTLY IN USE
	int currentState;

	// FRAME OF ANIMATION CURRENTLY BEING USED FOR currentState
	int currentFrame;

	// THE INDEX OF THE CURRENT FRAME IN THE ANIMATION SEQUENCE
	int frameIndex;

	// USED TO ITERATE THROUGH THE CURRENT ANIMATION SEQUENCE
	int animationCounter;

	bool myIsInAir;

	bool myJustAttacked;

	bool myIsFacingRight;

	BoundingVolume *boundingBox;

	int myScore;

public:
	// INLINED ACCESSOR METHODS
	int					getAlpha()			{ return alpha;			  }
	int					getCurrentFrame()	{ return currentFrame;	  }
	int					getCurrentState()	{ return currentState;	  }
	int					getFrameIndex()		{ return frameIndex;	  }
	int	                getScore()			{ return myScore;         }
	bool				getIsInAir()		{ return myIsInAir;		  }
	bool				getIsFacingRight()	{ return myIsFacingRight; }
	bool				getJustAttacked()   { return myJustAttacked;  }
	AnimatedSpriteType*	getSpriteType()		{ return spriteType;	  }
	BoundingVolume*		getBoundingBox();
		
	// INLINED MUTATOR METHODS
	void setAlpha(int initAlpha)
	{	alpha = initAlpha;				}

	void setSpriteType(AnimatedSpriteType *initSpriteType)
	{	spriteType = initSpriteType;	}

	void setIsInAir(bool isInAir)
	{	myIsInAir = isInAir;			}

	void setBoundingBox(BoundingVolume *box)
	{	boundingBox = box;				}

	void setIsFacingRight(bool facingRight)
	{	myIsFacingRight = facingRight;	}

	void setScore(int score)
	{   myScore = score;				}

	void setJustAttacked(bool justAttacked)
	{   myJustAttacked = justAttacked;  }


	// METHODS DEFINED IN AnimatedSprite.cpp
	AnimatedSprite();
	~AnimatedSprite();
	void changeFrame();
	void setCurrentState(int newState);
	void updateSprite(Game *game);
};