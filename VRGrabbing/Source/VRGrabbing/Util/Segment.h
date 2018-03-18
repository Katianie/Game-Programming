// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "Object.h"
#include "Segment.generated.h"

UCLASS(Blueprintable)
class USegment : public UObject
{
	GENERATED_BODY()

protected:
	///Instance Variables
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Data)
	float myValue;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Data)
	FName myName;

public:
	///Constructors
	USegment();
	USegment(float value, FName name);

	///Destructor
	virtual ~USegment();

	///Functions
	static void FindMinMax(TArray<TSubclassOf<USegment>> segments, float& outMin, float& outMax);
	static TSubclassOf<USegment> FindClosestSegment(TArray<TSubclassOf<USegment>>  segments, float currentSliderLocation);
	static TSubclassOf<USegment> FindSegmentWithValue(TArray<TSubclassOf<USegment>>  segments, float currentSliderLocation, float segmentSearchThreshold, bool& outSuccess);

	///Getters
	float GetValue();
	FName GetName();

	///Setters
	void SetValue(float value);
	void SetName(FName name);
};