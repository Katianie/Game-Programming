#include "Smash.h"
#include "Util/Segment.h"
 
USegment::USegment() : Super()
{
	myValue = 0;
	myName = FName();
}

USegment::USegment(float value, FName name) : Super()
{
	myValue = value;
	myName = name;
}

USegment::~USegment()
{

}

void USegment::FindMinMax(TArray<TSubclassOf<USegment>> segments, float& outMin, float& outMax)
{
	TSubclassOf<USegment> currMin = NULL;
	TSubclassOf<USegment> currMax = NULL;

	if (segments.Num() >= 1)
	{
		currMin = segments[0];
		currMax = segments[0];

		for (int currSegmentIndex = 0; currSegmentIndex < segments.Num(); currSegmentIndex++)
		{
			if (segments[currSegmentIndex].GetDefaultObject()->GetValue() > currMax.GetDefaultObject()->GetValue())
			{
				currMax = segments[currSegmentIndex];
			}

			if (segments[currSegmentIndex].GetDefaultObject()->GetValue() < currMin.GetDefaultObject()->GetValue())
			{
				currMin = segments[currSegmentIndex];
			}
		}
	}

	outMin = currMin.GetDefaultObject()->GetValue();
	outMax = currMax.GetDefaultObject()->GetValue();
}

TSubclassOf<USegment> USegment::FindClosestSegment(TArray<TSubclassOf<USegment>> segments, float currentSliderLocation)
{
	TSubclassOf<USegment> retVal = NULL;
	float closestSegmentDelta;

	if (segments.Num() >= 1)
	{
		retVal = segments[0];
		closestSegmentDelta = FMath::Abs(retVal.GetDefaultObject()->GetValue() - currentSliderLocation);
		for (int currSegmentIndex = 0; currSegmentIndex < segments.Num(); currSegmentIndex++)
		{
			if (FMath::Abs(segments[currSegmentIndex].GetDefaultObject()->GetValue() - currentSliderLocation) < closestSegmentDelta)
			{
				retVal = segments[currSegmentIndex];
				closestSegmentDelta = FMath::Abs(retVal.GetDefaultObject()->GetValue() - currentSliderLocation);
			}
		}
	}

	return retVal;
}

TSubclassOf<USegment> USegment::FindSegmentWithValue(TArray<TSubclassOf<USegment>> segments, float currentSliderLocation, float segmentSearchThreshold, bool& outSucess)
{
	TSubclassOf<USegment> retVal = NULL;

	for (int currSegmentIndex = 0; currSegmentIndex < segments.Num(); currSegmentIndex++)
	{
		if (segments[currSegmentIndex].GetDefaultObject()->GetValue() >= (currentSliderLocation - segmentSearchThreshold))
		{
			if (segments[currSegmentIndex].GetDefaultObject()->GetValue() <= (currentSliderLocation + segmentSearchThreshold))
			{
				retVal = segments[currSegmentIndex];
				break;
			}
		}
	}

	return retVal;
}

float USegment::GetValue()
{
	return myValue;
}

FName USegment::GetName()
{
	return myName;
}

void USegment::SetValue(float value)
{
	myValue = value;
}

void USegment::SetName(FName name)
{
	myName = name;
}