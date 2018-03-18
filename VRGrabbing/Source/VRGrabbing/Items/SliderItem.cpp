#include "Smash.h"
#include "Runtime/Engine/Classes/Kismet/KismetMathLibrary.h"
#include "SliderItem.h"

ASliderItem::ASliderItem()
{
	mySliderLength = 100.0;
	myShouldSnapToClosestSegment = true;
	mySegmentSearchThreshold = 0.1;
	mySliderRootComponent = NULL;
	myAnimateToSegmentTimelineCurve = NULL;
}

ASliderItem::~ASliderItem()
{
	if (myAnimateToSegmentTimeline != NULL && myAnimateToSegmentTimeline->IsValidLowLevel() == true)
	{
		myAnimateToSegmentTimeline->ConditionalBeginDestroy();
		myAnimateToSegmentTimeline = NULL;
	}
}

void ASliderItem::AnimateToSegmentTimelineUpdateCallback(float val)
{
	FVector interpolatedLocation;

	if (mySliderRootComponent != NULL)
	{
		interpolatedLocation = FMath::Lerp(mySliderRootComponent->RelativeLocation, myUpdatedSliderLocation, val);
		mySliderRootComponent->SetRelativeLocation(interpolatedLocation);
	}
}

void ASliderItem::AnimateToSegmentTimelineFinishedCallback()
{

}

void ASliderItem::BeginPlay()
{
	FOnTimelineFloat onTimelineUpdateCallback;
	FOnTimelineEventStatic onTimelineFinishedCallback;
	TArray<UActorComponent*> components;

	Super::BeginPlay();

	//Get all the scene components in the Button Item blueprint.
	components = this->GetComponentsByClass(USceneComponent::StaticClass());
	for (int currComponentIndex = 0; currComponentIndex < components.Num(); currComponentIndex++)
	{
		if (components[currComponentIndex]->GetName().Equals("SliderRoot", ESearchCase::IgnoreCase) == true)
		{
			mySliderRootComponent = Cast<USceneComponent>(components[currComponentIndex]);
			break;
		}
	}

	if (mySliderRootComponent != NULL)
	{
		myInitialSliderLocation = mySliderRootComponent->RelativeLocation;

		//Find the minimum and maximum positions in mySegments.
		USegment::FindMinMax(mySegments, myMinPosition, myMaxPosition);
	}

	if (myAnimateToSegmentTimelineCurve != NULL)
	{
		myAnimateToSegmentTimeline = NewObject<UTimelineComponent>(this, FName("AnimateToSegmentTimelineAnimation"));
		myAnimateToSegmentTimeline->CreationMethod = EComponentCreationMethod::UserConstructionScript; //Indicate it comes from a blueprint so it gets cleared when we rerun construction scripts
		this->BlueprintCreatedComponents.Add(myAnimateToSegmentTimeline); //Add to array so it gets saved
		myAnimateToSegmentTimeline->SetNetAddressable();	//This component has a stable name that can be referenced for replication

		myAnimateToSegmentTimeline->SetPropertySetObject(this); //Set which object the time-line should drive properties on
		myAnimateToSegmentTimeline->SetDirectionPropertyName(FName("AnimateToSegmentTimelineFinishedCallback"));

		myAnimateToSegmentTimeline->SetLooping(false);
		myAnimateToSegmentTimeline->SetTimelineLength(5.0f);
		myAnimateToSegmentTimeline->SetTimelineLengthMode(ETimelineLengthMode::TL_LastKeyFrame);

		myAnimateToSegmentTimeline->SetPlaybackPosition(0.0f, false);

		//Add the float curve to the time-line and connect it to your time-lines's interpolation function.
		onTimelineUpdateCallback.BindUFunction(this, FName{ TEXT("AnimateToSegmentTimelineUpdateCallback") });
		onTimelineFinishedCallback.BindUFunction(this, FName{ TEXT("AnimateToSegmentTimelineFinishedCallback") });
		myAnimateToSegmentTimeline->AddInterpFloat(myAnimateToSegmentTimelineCurve, onTimelineUpdateCallback);
		myAnimateToSegmentTimeline->SetTimelineFinishedFunc(onTimelineFinishedCallback);

		myAnimateToSegmentTimeline->RegisterComponent();
	}
}

void ASliderItem::AnimateToSegment(TSubclassOf<USegment> segment)
{
	float temp;

	if (segment != NULL && myAnimateToSegmentTimeline != NULL)
	{
		myAnimateToSegmentTimeline->PlayFromStart();

		//Normalize the segment value to the length of the slider.
		temp = mySliderLength * (segment.GetDefaultObject()->GetValue() / (myMaxPosition - myMinPosition));

		myUpdatedSliderLocation = myInitialSliderLocation - FVector(0.0f, temp, 0.0f);
	}
}

void ASliderItem::OnPlayerTouched(ABaseHand* playerHand)
{
	Super::OnPlayerTouched(playerHand);
}

void ASliderItem::Tick(float deltaTime)
{
	TSubclassOf<USegment> foundSegment = NULL;
	FVector ownerLocalSpacePos;
	FVector updatedLocation;
	float scaledLocationY;
	float locationDelta;
	bool success = false;

	Super::Tick(deltaTime);

	if (myAnimateToSegmentTimeline != NULL)
	{
		myAnimateToSegmentTimeline->TickComponent(deltaTime, ELevelTick::LEVELTICK_TimeOnly, NULL);
	}

	if (myPrimaryHand != NULL && mySliderRootComponent != NULL)
	{
		ownerLocalSpacePos = UKismetMathLibrary::InverseTransformLocation(this->GetActorTransform(), myPrimaryHand->GetControllerLocation());

		updatedLocation = myInitialSliderLocation - ownerLocalSpacePos;

		//Normalize
		updatedLocation = updatedLocation / mySliderLength;

		//Scaled location by position delta (only interested in the Y direction).
		locationDelta = (myMaxPosition - myMinPosition);
		scaledLocationY = updatedLocation.Y * locationDelta;

		//Clamp between min/max
		myCurrentSliderLocation = FMath::ClampAngle(scaledLocationY, myMinPosition, myMaxPosition);

		//Normalize the current slider location to get the direction.
		myCurrentSliderDirection = (myCurrentSliderLocation / FMath::ClampAngle(locationDelta, 0.0f, 1.0f));

		updatedLocation = FVector(0.0f, mySliderLength * myCurrentSliderDirection, 0.0f);
		mySliderRootComponent->SetRelativeLocation(myInitialSliderLocation - updatedLocation);

		//Call Slider Changed Delegate.
		if (SliderItem_OnMove.IsBound() == true)
		{
			SliderItem_OnMove.Broadcast(myCurrentSliderLocation);
		}

		foundSegment = USegment::FindSegmentWithValue(mySegments, myCurrentSliderLocation, mySegmentSearchThreshold, success);
		if (success == true)
		{
			if (foundSegment != NULL && foundSegment->GetName() != myCurrentSegment->GetName())
			{
				myCurrentSegment = foundSegment;

				//Play vibration when dragging over segment.
				Super::VibrateTouchingHands(EVibrationType::VE_TOUCH);
			}

		}
	}
}

void ASliderItem::OnPlayerDrop(ABaseHand* playerHand)
{
	bool success = false;
	TSubclassOf<USegment> foundSegment;

	Super::OnPlayerDrop(playerHand);

	if (myShouldSnapToClosestSegment == true)
	{
		foundSegment = USegment::FindClosestSegment(mySegments, myCurrentSliderLocation);

		ASliderItem::AnimateToSegment(foundSegment);

		success = true;
	}
	else
	{
		foundSegment = USegment::FindSegmentWithValue(mySegments, myCurrentSliderLocation, mySegmentSearchThreshold, success);
	}

	if (success == true)
	{
		//Call Segment Selected Delegate.
		if (SliderItem_OnSegmentSelected.IsBound() == true)
		{
			SliderItem_OnSegmentSelected.Broadcast(foundSegment);
		}
	}
}