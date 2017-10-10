/*
	AnimatedSprite.cpp

	See AnimatedSprite.h for a class description.
*/

#include "../stdafx.h"
#include "../GameEngine/world/AnimatedSprite.h"
#include "../GameEngine/world/AnimatedSpriteType.h"
#include "../GameEngine/physics/PhysicalProperties.h"
#include "../GameEngine/physics/BoundingVolume.h"
#include "../GameEngine/world/GameWorld.h"

/*
AnimatedSprite - Default constructor, just sets everything to 0.
*/
AnimatedSprite::AnimatedSprite()  
{
	spriteType = 0;
	currentState = 0;
	currentFrame = 0;
	frameIndex = 0;
	animationCounter = 0;
	myIsInAir = false;
	myJustAttacked = false;
	boundingBox = new BoundingVolume();
}

/*
Destructor - Nothing to clean up. We don't want to clean up the
sprite type because it is a shared variable.
*/
AnimatedSprite::~AnimatedSprite() 
{

}

BoundingVolume*	AnimatedSprite::getBoundingBox()
{ 
	return boundingBox;   
}

/*
changeFrame - This method allows for the changing of an image in an 
animation sequence for a given animation state.
*/
void AnimatedSprite::changeFrame()
{
	//I do all this so when u attack it only attacks once instead of infinatly
	//Then I need to calculate what the next state is
	if(currentState != PAUSED_GAME_STATE)
	{
		if (currentState == ATTACKING_RIGHT)
		{
			if (frameIndex >= 2)
			{
				if(this->getPhysicalProperties()->getVelocityX() > 0)
				{
					currentState = RUNNING_RIGHT;
				}
				else
				{
					currentState = STANDING_FACING_RIGHT;
				}
			}
		}
		else if (currentState == ATTACKING_LEFT)
		{
			if (frameIndex >= 2)
			{
				if(this->getPhysicalProperties()->getVelocityX() < 0)
				{
					currentState = RUNNING_LEFT;
				}
				else
				{
					currentState == STANDING_FACING_LEFT;
				}
			}

		}
	
		currentFrame = spriteType->getAnimationFrameID(currentState, frameIndex);
		frameIndex++;
		frameIndex = frameIndex % spriteType->getSequenceSize(currentState);
	}

}

void AnimatedSprite::setCurrentState(int newState) 
{
	currentState = newState;
	frameIndex = 0;
}

/*
updateSprite - To be called every frame of animation. This
method advances the animation counter appropriately per
the animation speed. It also updates the sprite location
per the current velocity.
*/
void AnimatedSprite::updateSprite(Game *game)
{
	animationCounter++;
	animationCounter = animationCounter%spriteType->getAnimationSpeed();

	// WE ONLY CHANGE THE FRAME OF ANIMATION ONCE EVERY animationSpeed FRAMES
	if (animationCounter == 0)
	{
		changeFrame();
	}

	pp->incX(pp->getVelocityX());
	pp->incY(pp->getVelocityY());
}