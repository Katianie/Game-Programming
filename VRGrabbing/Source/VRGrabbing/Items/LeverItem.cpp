#include "Smash.h"
#include "Runtime/Engine/Classes/Kismet/KismetMathLibrary.h"
#include "LeverItem.h"

ALeverItem::ALeverItem()
{
	myIsResettingRotation = false;
	myLeverBallComponent = NULL;
	myLeverRotateComponent = NULL;
	myFinalPullBoxComponent = NULL;
	myResetRotationTimeline = NULL;
}

ALeverItem::~ALeverItem()
{
	if (myResetRotationTimeline != NULL && myResetRotationTimeline->IsValidLowLevel() == true)
	{
		myResetRotationTimeline->ConditionalBeginDestroy();
		myResetRotationTimeline = NULL;
	}
}

void ALeverItem::BeginPlay()
{
	int numComponents = 0;
	FOnTimelineFloat onTimelineUpdateCallback;
	FOnTimelineEventStatic onTimelineFinishedCallback;
	TArray<UActorComponent*> components;

	Super::BeginPlay();

	//Get all the scene components in the Button Item blueprint.
	components = this->GetComponentsByClass(USceneComponent::StaticClass());
	for (int currComponentIndex = 0; numComponents < 3 && currComponentIndex < components.Num(); currComponentIndex++)
	{
		if (components[currComponentIndex]->GetName().Equals("LeverRotate", ESearchCase::IgnoreCase) == true)
		{
			myLeverRotateComponent = Cast<USceneComponent>(components[currComponentIndex]);
			numComponents++;
		}
		else if (components[currComponentIndex]->GetName().Equals("LeverBall", ESearchCase::IgnoreCase) == true)
		{
			myLeverBallComponent = Cast<USceneComponent>(components[currComponentIndex]);
			numComponents++;
		}
		else if (components[currComponentIndex]->GetName().Equals("FinalPullBox", ESearchCase::IgnoreCase) == true)
		{
			myFinalPullBoxComponent = Cast<UBoxComponent>(components[currComponentIndex]);
			numComponents++;
		}
	}

	if (myLeverRotateComponent != NULL)
	{
		myInitialLeverRotation = myLeverRotateComponent->RelativeRotation;
	}

	if (myResetRotationTimelineCurve != NULL)
	{
		myResetRotationTimeline = NewObject<UTimelineComponent>(this, FName("ResetRotationTimelineAnimation"));
		myResetRotationTimeline->CreationMethod = EComponentCreationMethod::UserConstructionScript; // Indicate it comes from a blueprint so it gets cleared when we rerun construction scripts
		this->BlueprintCreatedComponents.Add(myResetRotationTimeline); //Add to array so it gets saved
		myResetRotationTimeline->SetNetAddressable();	//This component has a stable name that can be referenced for replication

		myResetRotationTimeline->SetPropertySetObject(this); //Set which object the time-line should drive properties on
		myResetRotationTimeline->SetDirectionPropertyName(FName("ResetRotationTimelineFinishedCallback"));

		myResetRotationTimeline->SetLooping(false);
		myResetRotationTimeline->SetTimelineLength(5.0f);
		myResetRotationTimeline->SetTimelineLengthMode(ETimelineLengthMode::TL_LastKeyFrame);

		myResetRotationTimeline->SetPlaybackPosition(0.0f, false);

		//Add the float curve to the time-line and connect it to your time-lines's interpolation function
		onTimelineUpdateCallback.BindUFunction(this, FName{ TEXT("ResetRotationTimelineUpdateCallback") });
		onTimelineFinishedCallback.BindUFunction(this, FName{ TEXT("ResetRotationTimelineFinishedCallback") });
		myResetRotationTimeline->AddInterpFloat(myResetRotationTimelineCurve, onTimelineUpdateCallback);
		myResetRotationTimeline->SetTimelineFinishedFunc(onTimelineFinishedCallback);

		myResetRotationTimeline->RegisterComponent();
	}

	if (myFinalPullBoxComponent != NULL)
	{
		myFinalPullBoxComponent->OnComponentBeginOverlap.AddDynamic(this, &ALeverItem::OnComponentBeginOverlap);
	}
}

void ALeverItem::OnPlayerTouched(ABaseHand* playerHand)
{
	Super::OnPlayerTouched(playerHand);
}

void ALeverItem::Tick(float deltaTime)
{
	FVector ownerLocalSpacePos;
	FVector currLeverDistance;
	float rotateAngle = 0.0f;

	Super::Tick(deltaTime);

	//Only tick if not resetting rotation.
	if (myIsResettingRotation == false)
	{
		if (myPrimaryHand != NULL && myLeverRotateComponent != NULL)
		{
			ownerLocalSpacePos = UKismetMathLibrary::InverseTransformLocation(this->GetActorTransform(), myPrimaryHand->GetControllerLocation());

			currLeverDistance = ownerLocalSpacePos - myLeverRotateComponent->RelativeLocation;

			//Calculate the angle between the hand and the current location of the lever.
			rotateAngle = FMath::RadiansToDegrees(FMath::Atan2(currLeverDistance.Z, currLeverDistance.X));
			FMath::Clamp(rotateAngle, myMinAngle, myMaxAngle);

			myLeverRotateComponent->SetRelativeRotation(FRotator(rotateAngle, 0.0f, 0.0f));
		}
	}
}

void ALeverItem::ResetRotation()
{
	if (myLeverRotateComponent != NULL)
	{
		myIsResettingRotation = true;
		myInterpLeverRotation = myLeverRotateComponent->RelativeRotation;

		//Reset rotation time-line
		if (myResetRotationTimeline != NULL)
		{
			myResetRotationTimeline->PlayFromStart();
		}
	}
}

void ALeverItem::ResetRotationTimelineUpdateCallback(float val)
{
	//This function is called for every tick in the time-line.
	if (myLeverRotateComponent != NULL)
	{
		myLeverRotateComponent->SetRelativeRotation(FMath::Lerp(myInterpLeverRotation, myInitialLeverRotation, val));
	}
}

void ALeverItem::ResetRotationTimelineFinishedCallback()
{
	//This function is called when the time-line reaches the finished stage.
	myIsResettingRotation = false;
}

void ALeverItem::OnComponentBeginOverlap(UPrimitiveComponent* overlappedComp, AActor* otherActor, UPrimitiveComponent* otherComp, int32 otherBodyIndex, bool isFromSweep, const FHitResult& sweepResult)
{
	if (myLeverBallComponent != NULL && otherComp != NULL)
	{
		if (otherComp == Cast<UPrimitiveComponent>(myLeverBallComponent))
		{
			ALeverItem::ResetRotation();

			//Lever Item Pulled delegate
			if (LeverItem_OnPulled.IsBound() == true)
			{
				LeverItem_OnPulled.Broadcast();
			}

			//Play vibration feedback.
			Super::VibrateTouchingHands(EVibrationType::VE_HIT);
		}
	}
}

void ALeverItem::OnPlayerDrop(ABaseHand* playerHand)
{
	Super::OnPlayerDrop(playerHand);

	ALeverItem::ResetRotation();
}