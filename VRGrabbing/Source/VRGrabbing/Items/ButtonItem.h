// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseItem.h"
#include "Runtime/Engine/Classes/Kismet/KismetMathLibrary.h"
#include "ButtonItem.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FButtonItemPressedSignatrue);

UCLASS(config = Game)
class AButtonItem : public ABaseItem
{
	GENERATED_BODY()

protected:
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	float myMaxButtonPress;

	FButtonItemPressedSignatrue ButtonItem_OnPressed;

	bool myHasBeenTouchedOnce;
	FVector myInitialOverlapPosition;
	FVector myButtonInitialPosition;
	UStaticMeshComponent* myButtonComponent;

public:

	///Constructor
	AButtonItem();

	///Destructor
	virtual ~AButtonItem();

	///Functions
	virtual void BeginPlay() override;
	virtual void Tick(float deltaTime) override;
	virtual void OnPlayerTouched(ABaseHand* playerHand) override;
	virtual void OnPlayerDrop(ABaseHand* playerHand) override;
};