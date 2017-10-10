#include "GamePhysics.h"

GamePhysics::GamePhysics(Game *game)
{
	myPlayer = game->getWorld()->getSpriteManager()->getPlayer();
	myPhysicalProperty = myPlayer->getPhysicalProperties();
	myBots = game->getAI()->getBots();
	myViewport = game->getWorld()->getViewport();


	myGameOverImage = new OverlayImage();
	myButtonEventHandler = new WRButtonEventHandler();
}

void GamePhysics::update(Game *game)
{
	srand(time(NULL));

	if(myPhysicalProperty->getY() < GROUND_HEIGHT)
	{
		//anything effected by grav here
		myPhysicalProperty->incVelocityY(3.0f);
		myPlayer->setIsInAir(true);
		myPlayer->getBoundingBox()->setY(myPhysicalProperty->getY());
	}
	
	if(myPhysicalProperty->getY() > GROUND_HEIGHT)
	{
		if(myPlayer->getCurrentState() == JUMPING_LEFT)
		{
			if(myPhysicalProperty->getVelocityX() < 0)
			{
				myPlayer->setCurrentState(RUNNING_LEFT);
			}
			else
			{
				myPlayer->setCurrentState(STANDING_FACING_LEFT);
			}
		}
		else
		{
			if(myPhysicalProperty->getVelocityX() > 0)
			{
				myPlayer->setCurrentState(RUNNING_RIGHT);
			}
			else
			{
				myPlayer->setCurrentState(STANDING_FACING_RIGHT);
			}
		}

		myPhysicalProperty->setY(GROUND_HEIGHT);
		myPhysicalProperty->setVelocityY(0.0f);
		myPlayer->setIsInAir(false);


		myPlayer->getBoundingBox()->setY(myPhysicalProperty->getY());
	}

	//now the fun part

	//collision for player on the left and right ends of the level
	if(myPlayer->getBoundingBox()->getX() <= 0)
	{
		myPlayer->getPhysicalProperties()->setX(1);
	}
	else if(myPlayer->getBoundingBox()->getX() >= game->getWorld()->getWorldWidth())
	{
		myPlayer->getPhysicalProperties()->setX(game->getWorld()->getWorldWidth() - myPlayer->getBoundingBox()->getWidth());
	}

	//check all the bots for possible collision
	for(int i = 0; i < myBots->size(); i++)
	{
		myCurrBot = myBots->at(i);
		myCurrBotX = myCurrBot->getBoundingBox()->getX();
		myCurrBotY = myCurrBot->getBoundingBox()->getY();
		myCurrBotWidth = myCurrBot->getBoundingBox()->getWidth();
		myCurrBotHeight = myCurrBot->getBoundingBox()->getHeight();
		myCurrBotVelocity = myCurrBot->getPhysicalProperties()->getVelocityX();


		if(myViewport->areWorldCoordinatesInViewport(myCurrBotX, myCurrBotY, myCurrBotWidth, myCurrBotHeight))
		{
			//check to see if playerY is between bot y and bot y + height
			if( myPlayer->getBoundingBox()->getY() >= myCurrBotY && myPlayer->getBoundingBox()->getY() <= (myCurrBotY + myCurrBotHeight) )
			{
				//need to test all edges of rect?
				if(myPlayer->getIsFacingRight())
				{
					myCurrBotTime = ( myCurrBotX - myPlayer->getBoundingBox()->getX() ) 
						/ ( myPlayer->getPhysicalProperties()->getVelocityX() - myCurrBotVelocity );
				}
				else
				{
					myCurrBotTime = ( myCurrBotX - myPlayer->getBoundingBox()->getX() ) 
						/ ( myPlayer->getPhysicalProperties()->getVelocityX() - myCurrBotVelocity );
				}

				if(myCurrBotTime >= 0.0 && myCurrBotTime < 13.0)
				{
					if(myPlayer->getCurrentState() == ATTACKING_RIGHT || myPlayer->getCurrentState() == JUMPING_RIGHT)
					{
						//kill snowball(aka move it to use it later)
						myCurrBot->getPhysicalProperties()->setX(game->getWorld()->getWorldWidth() + (rand() % 1 + 1000));

						//points
						myPlayer->setScore(myPlayer->getScore() + 100);

					}
					else if(myPlayer->getCurrentState() == ATTACKING_LEFT || myPlayer->getCurrentState() == JUMPING_LEFT)
					{
						//kill snowball(aka move it to use it later)
						myCurrBot->getPhysicalProperties()->setX(game->getWorld()->getWorldWidth() + (rand() % 1 + 1000));

						//points
						myPlayer->setScore(myPlayer->getScore() + 100);
					}
					else
					{
						//gameover
						myGui = game->getGUI()->getScreenGUI(GAME_OVER_GAME_STATE);
						myGUITextureManager = game->getGraphics()->getGUITextureManager();
						myNewGameButton = new Button();
						myExitButton = new Button();

						myNewGameButtonID = myGUITextureManager->getStringTable()->getIndexOfStringFromStringTable(L"textures/gui/buttons/new_game.bmp");
						myNewGameOverButtonID = myGUITextureManager->getStringTable()->getIndexOfStringFromStringTable(L"textures/gui/buttons/new_game_mo.bmp");
						myNewGameCommand = constructEmptyWCHAR_TArray(L"New Game");

						myExitButtonID =  myGUITextureManager->getStringTable()->getIndexOfStringFromStringTable(L"textures/gui/buttons/exit_game.bmp");
						myExitOverButtonID =  myGUITextureManager->getStringTable()->getIndexOfStringFromStringTable(L"textures/gui/buttons/exit_game_mo.bmp");
						myExitCommand = constructEmptyWCHAR_TArray(L"Exit");

						myGameOverID = myGUITextureManager->getStringTable()->getIndexOfStringFromStringTable(L"textures/gui/overlays/Game_Over.bmp"); 
						myGameOverImage->x = 0;
						myGameOverImage->y = 0;
						myGameOverImage->z = 0;
						myGameOverImage->alpha = 200;
						myGameOverImage->width = 1024;
						myGameOverImage->height = 768;
						myGameOverImage->imageID = myGameOverID;

						myGui->addOverlayImage(myGameOverImage);

						myExitButton->initButton(myExitButtonID, 
							myExitOverButtonID,
							400,
							600,
							0,
							255,
							200,
							100,
							false,
							myExitCommand);

						myNewGameButton->initButton(myNewGameButtonID, 
							myNewGameOverButtonID,
							400,
							500,
							0,
							255,
							200,
							100,
							false,
							myNewGameCommand);

						myGui->addButton(myExitButton);
						myGui->addButton(myNewGameButton);

						myButtonEventHandler = new WRButtonEventHandler();
						myGui->registerButtonEventHandler((ButtonEventHandler*)myButtonEventHandler);

						game->changeGameState(GAME_OVER_GAME_STATE);

						

					}

				}
			}

			
		}
	}
}