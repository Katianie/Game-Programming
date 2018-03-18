// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseItem.h"
#include "DrawerItem.generated.h"

UCLASS(config = Game)
class ADrawerItem : public ABaseItem
{
	GENERATED_BODY()

protected:
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	float myDrawerFriction;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	float myDrawerLength;

	bool myHasPhysicsStarted;
	float myCurrentDrawerPosition;
	float myPreviousLocation;
	float myDrawerInitialGrabOffset;
	float myDrawerInitialVelocity;
	float myInitialPhysicsAngle;
	float myTimeSinceDrawerLetGo;
	float myCurrentDrawerPositionNormalized;
	FVector myInitialOverlapPosition;
	USceneComponent* myDrawerComponent;


public:
	///Constructor
	ADrawerItem();

	///Destructor
	virtual ~ADrawerItem();

	///Functions;
	virtual void BeginPlay() override;
	virtual void Tick(float deltaTime) override;
	virtual void OnPlayerTouched(ABaseHand* playerHand) override;
	virtual bool ShouldStopPhysicsSimulation();
	virtual void StartDrawerPhysics(float initialVelocity);
	virtual float CalculateRelativeDrawerLocation();
	virtual void StopDrawerPhysics();
	virtual void OnPlayerDrop(ABaseHand* playerHand) override;

};