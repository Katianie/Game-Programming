// Copyright 1998-2016 Epic Games, Inc. All Rights Reserved.

#include "Smash.h"
#include "SmashGameMode.h"
#include "SmashHUD.h"
#include "BaseCharacter.h"

ASmashGameMode::ASmashGameMode() : Super()
{
	// set default pawn class to our Blueprinted character
	static ConstructorHelpers::FClassFinder<APawn> PlayerPawnClassFinder(TEXT("/Game/FullBodyMannequin/FirstPerson/Blueprints/FirstPersonCharacter"));
	DefaultPawnClass = PlayerPawnClassFinder.Class;

	// use our custom HUD class
	HUDClass = ASmashHUD::StaticClass();
}
