// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseItem.h"
#include "Util/Segment.h"
#include "SliderItem.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FSliderMoveSignature, float, sliderLocation);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FSliderSegmentSelectedSignature, TSubclassOf<USegment>, selectedSegment);

UCLASS(config = Game)
class ASliderItem : public ABaseItem
{
	GENERATED_BODY()

protected:
	
	FSliderMoveSignature SliderItem_OnMove;
	FSliderSegmentSelectedSignature SliderItem_OnSegmentSelected;

	USceneComponent* mySliderRootComponent;
	TSubclassOf<USegment> myCurrentSegment;
	FVector myInitialSliderLocation;
	FVector myUpdatedSliderLocation;
	float myCurrentSliderLocation;
	float myCurrentSliderDirection;
	float myMinPosition;
	float myMaxPosition;

	//TODO: MAKE GUNS TAKE TSUBCLASS OF PROJECTILE.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Touch)
	TArray<TSubclassOf<USegment>> mySegments;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	float mySliderLength;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	bool myShouldSnapToClosestSegment;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	float mySegmentSearchThreshold;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Touch)
	UCurveFloat* myAnimateToSegmentTimelineCurve;

	UPROPERTY()
	UTimelineComponent* myAnimateToSegmentTimeline;

public:
	///Constructor
	ASliderItem();

	///Destructor
	virtual ~ASliderItem();

	///Callbacks
	UFUNCTION()
	void AnimateToSegmentTimelineUpdateCallback(float val);
	UFUNCTION()
	void AnimateToSegmentTimelineFinishedCallback();

	///Functions
	virtual void BeginPlay() override;
	virtual void AnimateToSegment(TSubclassOf<USegment> segment);
	virtual void OnPlayerTouched(ABaseHand* playerHand) override;
	virtual void Tick(float deltaTime) override;
	virtual void OnPlayerDrop(ABaseHand* playerHand) override;
};