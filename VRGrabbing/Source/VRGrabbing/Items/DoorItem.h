// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseItem.h"
#include "DoorItem.generated.h"

UCLASS(config = Game)
class ADoorItem : public ABaseItem
{
	GENERATED_BODY()

protected:

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	float myDoorFriction;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	float myMinAngle;
	
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	float myMaxAngle;

	bool myIsResettingRotation;
	bool myHasPhysicsStarted;
	float myDoorInitialGrabOffset;
	float myDoorInitialVelocity;
	float myDoorInitialRotation;
	float myTimeSinceDoorLetGo;
	float myInitialPhysicsAngle;
	FVector myInitialOverlapPosition;
	FRotator myInitialDoorRotation;
	FRotator myPreviousRotation;
	FRotator myInterpDoorRotation;
	USceneComponent* myDoorRotateComponent;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	UCurveFloat* myResetRotationTimelineCurve;

	UPROPERTY()
	UTimelineComponent* myResetRotationTimeline;

public:

	///Constructor
	ADoorItem();

	///Destructor
	virtual ~ADoorItem();

	///Callbacks
	UFUNCTION()
	void ResetRotationTimelineUpdateCallback(float val);
	UFUNCTION()
	void ResetRotationTimelineFinishedCallback();

	///Functions
	float CalculateRelativeDoorAngle();
	virtual void BeginPlay() override;
	virtual void Tick(float deltaTime) override;
	virtual void OnPlayerTouched(ABaseHand* playerHand) override;
	virtual bool ShouldStopPhysicsSimulation();
	virtual void StartDoorPhysics(float initialVelocity);
	virtual void StopDoorPhysics();
	virtual void CloseDoor();
	virtual void OnPlayerDrop(ABaseHand* playerHand) override;
};