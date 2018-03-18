// Copyright 1998-2016 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "Smash.h"
#include "BaseEnums.generated.h"

UENUM(BlueprintType)
enum class EVibrationType : uint8
{
	VE_TOUCH		UMETA(DisplayName = "Touch Vibration"),
	VE_PICKUP		UMETA(DisplayName = "Pickup Vibration"),
	VE_HIT			UMETA(DisplayName = "Direct Hit Vibration"),
	VE_GLANCEHIT	UMETA(DisplayName = "Glancing Hit Vibration"),
	VE_DROP			UMETA(DisplayName = "Drop Vibration"),
	VE_CRUSH		UMETA(DisplayName = "Crush Vibration"),
	VE_SMALLSHOT	UMETA(DisplayName = "Small Gunshot Vibration"),
	VE_BIGSHOT		UMETA(DisplayName = "Large Gunshot Vibration")
};

UENUM(BlueprintType)
enum class EWeaponType : uint8
{
	VE_ONE_HANDED 	UMETA(DisplayName = "One Handed"),
	VE_TWO_HANDED 	UMETA(DisplayName = "Two Handed"),
	VE_THROWING		UMETA(DisplayName = "Throwing")
};