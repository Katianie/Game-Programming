
#pragma once

// PREPROCESSOR INCLUDE STATEMENTS
#include "../resource.h"
#include "../stdafx.h"
#include "../GameEngine/world/AnimatedSprite.h"
#include "../GameEngine/game/Game.h"
#include "../GameEngine/graphics/GameGraphics.h"
#include "../GameEngine/graphics/TextureManager.h"
#include "../GameEngine/world/SpriteManager.h"
#include "../GameEngine/physics/PhysicalProperties.h"
#include "../GameEngine/physics/BoundingVolume.h"
#include <time.h>

class Bot
{
private:
	AnimatedSprite *animatedSprite;
	PhysicalProperties *physicalProperties;
	BoundingVolume *boundingBox;
	float density;

public:
	// INLINED METHODS
	AnimatedSprite* getAnimatedSprite() { return animatedSprite; }
	PhysicalProperties* getPhysicalProperties() { return physicalProperties; }
	BoundingVolume* getBoundingBox() { return boundingBox; }
	float getDensity() { return density; }


	void setDensity(float initDensity) { density = initDensity; }
	
	Bot(Game *game, int imageId);
	~Bot();
	void update(Game *game);
};