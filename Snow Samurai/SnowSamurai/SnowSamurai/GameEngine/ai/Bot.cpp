#include "../GameEngine/ai/Bot.h"

Bot::Bot(Game *game, int imageId)
{
	SpriteManager *spriteManager = game->getWorld()->getSpriteManager();
	physicalProperties = new PhysicalProperties();
	AnimatedSpriteType *animatedSpriteType;
	wchar_t *animStateName = constructEmptyWCHAR_TArray(L"MOVING");
	vector<int> *playSequence = new vector<int>();
	int randX = 0; //used to space out the moving snoballs when they are created

	animatedSprite = new  AnimatedSprite();
	animatedSpriteType = new AnimatedSpriteType();

	boundingBox = animatedSprite->getBoundingBox();

	srand(time(NULL));
	randX = rand() % 1 + 1000;

	animatedSpriteType->addImageID(imageId);

	animatedSpriteType->setAnimationSpeed(60);

	playSequence->push_back(imageId);

	animatedSpriteType->addAnimationState(animStateName, playSequence);

	if(imageId == 7)
	{
		animatedSpriteType->setTextureSize(64, 64);
		boundingBox->setWidth(64);
		boundingBox->setHeight(64);

		physicalProperties->setY(GROUND_HEIGHT - 64);
		physicalProperties->setX(game->getWorld()->getWorldWidth() + randX);
	}
	else
	{
		animatedSpriteType->setTextureSize(128, 128);
		boundingBox->setWidth(128);
		boundingBox->setHeight(128);

		physicalProperties->setY(GROUND_HEIGHT - 200);
		physicalProperties->setX(game->getWorld()->getWorldWidth() + randX);
	}

	boundingBox->setY(physicalProperties->getY());
	boundingBox->setX(physicalProperties->getX());

	spriteManager->addSpriteType(animatedSpriteType);

	animatedSprite->setSpriteType(animatedSpriteType);

	animatedSprite->setPhysicalProperties(physicalProperties);

	physicalProperties->setAccelerationX(0);
	physicalProperties->setAccelerationY(0);

	animatedSprite->setAlpha(255);
	animatedSprite->setCurrentState(0);

	spriteManager->addSprite(animatedSprite);
}

void Bot::update(Game *game)
{
	if(physicalProperties->getX() < 0)
	{
		physicalProperties->setX(game->getWorld()->getWorldWidth() + (rand() % 1 + 100));
	}
	else
	{
		physicalProperties->incX(physicalProperties->getVelocityX());
		physicalProperties->incX(physicalProperties->getVelocityX());
	}

	boundingBox->setX(physicalProperties->getX());
	boundingBox->setY(physicalProperties->getY());
}