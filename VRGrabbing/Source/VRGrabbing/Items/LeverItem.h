// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseItem.h"
#include "LeverItem.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FLeverItemPulledSignatrue);

UCLASS(config = Game)
class ALeverItem : public ABaseItem
{
	GENERATED_BODY()

protected:

	FLeverItemPulledSignatrue LeverItem_OnPulled;

	USceneComponent* myLeverBallComponent;
	USceneComponent* myLeverRotateComponent;
	UBoxComponent* myFinalPullBoxComponent;
	FRotator myInitialLeverRotation;
	FRotator myInterpLeverRotation;
	bool myIsResettingRotation;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	float myMinAngle;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	float myMaxAngle;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	UCurveFloat* myResetRotationTimelineCurve;

	UPROPERTY()
	UTimelineComponent* myResetRotationTimeline;

public:
	///Constructor
	ALeverItem();

	///Destructor
	virtual ~ALeverItem();

	///Callbacks
	UFUNCTION()
	void ResetRotationTimelineUpdateCallback(float val);
	UFUNCTION()
	void ResetRotationTimelineFinishedCallback();
	UFUNCTION()
	virtual void OnComponentBeginOverlap(UPrimitiveComponent* overlappedComp, AActor* otherActor, UPrimitiveComponent* otherComp, int32 otherBodyIndex, bool isFromSweep, const FHitResult& sweepResult);

	///Functions
	virtual void BeginPlay() override;
	virtual void OnPlayerTouched(ABaseHand* playerHand) override;
	virtual void Tick(float deltaTime) override;
	virtual void ResetRotation();
	virtual void OnPlayerDrop(ABaseHand* playerHand) override;
};