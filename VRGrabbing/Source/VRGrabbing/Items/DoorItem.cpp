#include "Smash.h"
#include "Runtime/Engine/Classes/Kismet/KismetMathLibrary.h"
#include "DoorItem.h"

ADoorItem::ADoorItem() : Super()
{
	myIsResettingRotation = false;
	myHasPhysicsStarted = false;
	myDoorInitialGrabOffset = 0.0f;
	myDoorInitialVelocity = 0.0f;
	myTimeSinceDoorLetGo = 0.0f;
	myInitialPhysicsAngle = 0.0f;
	myInitialDoorRotation = FRotator::ZeroRotator;
	myInterpDoorRotation = FRotator::ZeroRotator;
	myPreviousRotation = FRotator::ZeroRotator;
	myDoorFriction = 0.01f;
	myMinAngle = 10.0f;
	myMaxAngle = 170.0f;
	myInitialOverlapPosition = FVector::ZeroVector;
	myDoorRotateComponent = NULL;
	myResetRotationTimelineCurve = NULL;

	//Enable Tick for BaseItem(s).
	PrimaryActorTick.bCanEverTick = true;
}

ADoorItem::~ADoorItem()
{
	if (myResetRotationTimeline != NULL && myResetRotationTimeline->IsValidLowLevel() == true)
	{
		myResetRotationTimeline->ConditionalBeginDestroy();
		myResetRotationTimeline = NULL;
	}
}

void ADoorItem::BeginPlay()
{
	FOnTimelineFloat onTimelineUpdateCallback;
	FOnTimelineEventStatic onTimelineFinishedCallback;
	TArray<UActorComponent*> components;

	Super::BeginPlay();

	//Get all the scene components in the Button Item blueprint.
	components = this->GetComponentsByClass(USceneComponent::StaticClass());
	for (int currComponentIndex = 0; currComponentIndex < components.Num(); currComponentIndex++)
	{
		if (components[currComponentIndex]->GetName().Equals("DoorRotate", ESearchCase::IgnoreCase) == true)
		{
			myDoorRotateComponent = Cast<USceneComponent>(components[currComponentIndex]);
			break;
		}
	}

	if (myDoorRotateComponent != NULL)
	{
		myInitialDoorRotation = myDoorRotateComponent->GetRelativeTransform().GetRotation().Rotator();
		myPreviousRotation = myInitialDoorRotation;
	}

	if (myResetRotationTimelineCurve != NULL)
	{
		myResetRotationTimeline = NewObject<UTimelineComponent>(this, FName("ResetRotationTimelineAnimation"));
		myResetRotationTimeline->CreationMethod = EComponentCreationMethod::UserConstructionScript; // Indicate it comes from a blueprint so it gets cleared when we rerun construction scripts
		this->BlueprintCreatedComponents.Add(myResetRotationTimeline); // Add to array so it gets saved
		myResetRotationTimeline->SetNetAddressable();	// This component has a stable name that can be referenced for replication

		myResetRotationTimeline->SetPropertySetObject(this); // Set which object the time-line should drive properties on
		myResetRotationTimeline->SetDirectionPropertyName(FName("ResetRotationTimelineFinishedCallback"));

		myResetRotationTimeline->SetLooping(false);
		myResetRotationTimeline->SetTimelineLength(5.0f);
		myResetRotationTimeline->SetTimelineLengthMode(ETimelineLengthMode::TL_LastKeyFrame);

		myResetRotationTimeline->SetPlaybackPosition(0.0f, false);

		//Add the float curve to the time--line and connect it to your time-lines's interpolation function
		onTimelineUpdateCallback.BindUFunction(this, FName{ TEXT("ResetRotationTimelineUpdateCallback") });
		onTimelineFinishedCallback.BindUFunction(this, FName{ TEXT("ResetRotationTimelineFinishedCallback") });
		myResetRotationTimeline->AddInterpFloat(myResetRotationTimelineCurve, onTimelineUpdateCallback);
		myResetRotationTimeline->SetTimelineFinishedFunc(onTimelineFinishedCallback);

		myResetRotationTimeline->RegisterComponent();
	}

}

void ADoorItem::OnPlayerTouched(ABaseHand* playerHand)
{
	FVector ownerLocalSpacePos;
	FVector doorLocalSpacePos;
	FVector doorOffsetDiff;
	float doorOffsetAngle;

	Super::OnPlayerTouched(playerHand);

	if (playerHand != NULL && myDoorRotateComponent != NULL)
	{
		myInitialOverlapPosition = playerHand->GetControllerLocation();
		ownerLocalSpacePos = UKismetMathLibrary::InverseTransformLocation(this->GetActorTransform(), myInitialOverlapPosition);

		//Calculate the angle between the hand and the door.
		doorOffsetDiff = ownerLocalSpacePos - myDoorRotateComponent->GetRelativeTransform().GetLocation();
		doorOffsetAngle = FMath::RadiansToDegrees(FMath::Atan2(doorOffsetDiff.Y, doorOffsetDiff.X));
		myDoorInitialRotation = myDoorRotateComponent->GetRelativeTransform().GetRotation().Rotator().Yaw;
		myDoorInitialGrabOffset = doorOffsetAngle - myDoorInitialRotation;
		
		//Stop the physics of the door when we grab it.
		this->StopDoorPhysics();
	}
}

void ADoorItem::Tick(float deltaTime)
{
	FVector ownerLocalSpacePos;
	FVector doorOffsetDiff;
	float doorOffsetAngle;
	float doorMoveAmount;

	Super::Tick(deltaTime);

	//Only tick if not resetting rotation.
	if (myIsResettingRotation == false)
	{
		if (myResetRotationTimeline != NULL)
		{
			myResetRotationTimeline->TickComponent(deltaTime, ELevelTick::LEVELTICK_TimeOnly, NULL);
		}

		if (myDoorRotateComponent != NULL)
		{
			if (myPrimaryHand != NULL)
			{
				//Get the owners hand location in local space.
				ownerLocalSpacePos = UKismetMathLibrary::InverseTransformLocation(this->GetActorTransform(), myPrimaryHand->GetControllerLocation());

				//Calculate the angle between the hand and the door.
				doorOffsetDiff = ownerLocalSpacePos - myDoorRotateComponent->GetRelativeTransform().GetLocation();
				doorOffsetAngle = FMath::RadiansToDegrees(FMath::Atan2(doorOffsetDiff.Y, doorOffsetDiff.X));

				//Record the previous rotation.
				myPreviousRotation = myDoorRotateComponent->GetRelativeTransform().GetRotation().Rotator();

				//Move the door between the initial grab location and the current hand location.
				doorMoveAmount = doorOffsetAngle - myDoorInitialGrabOffset;

				FMath::ClampAngle(doorMoveAmount, myMinAngle, myMaxAngle);

				//Move the door according to the angle of the hand location and the start hand location.
				myDoorRotateComponent->SetRelativeRotation(FRotator(0.0f, doorMoveAmount, 0.0f));

				//Play vibration feedback.
				Super::VibrateTouchingHands(EVibrationType::VE_TOUCH);
			}

			if (myHasPhysicsStarted == true)
			{
				//Keep track of the last time the door was touched.
				myTimeSinceDoorLetGo += deltaTime;

				//Determine if we need to keep moving the door or we can stop it.
				if (this->ShouldStopPhysicsSimulation() == true)
				{
					this->StopDoorPhysics();
				}
				else
				{
					myDoorRotateComponent->SetRelativeRotation(FRotator(0.0f, this->CalculateRelativeDoorAngle(), 0.0f));
				}
			}
		}
	}
}

bool ADoorItem::ShouldStopPhysicsSimulation()
{
	int v;
	int u = myDoorInitialVelocity;
	int at = (myDoorFriction * 980) * myTimeSinceDoorLetGo * FMath::Sign(myDoorInitialVelocity);
	float currDoorAngle;

	//Physics formula used: v = u - at
	v = u - at;

	if (myDoorInitialVelocity <= 0)
	{
		if (v > 0.0f)
		{
			return true;
		}
	}
	else
	{
		if (v <= 0.0f)
		{
			return true;
		}
	}

	if (myDoorRotateComponent != NULL)
	{
		currDoorAngle = myDoorRotateComponent->GetRelativeTransform().GetRotation().Rotator().Yaw;
		if (currDoorAngle >= myMaxAngle)
		{
			return true;
		}
		if (currDoorAngle <= myMinAngle)
		{
			return true;
		}
		if (FMath::IsNearlyEqual(currDoorAngle, myMinAngle, 1.0f) == true)
		{
			return true;
		}
		if (FMath::IsNearlyEqual(currDoorAngle, myMaxAngle, 1.0f) == true)
		{
			return true;
		}
	}

	return false;
}

void ADoorItem::StartDoorPhysics(float initialVelocity)
{
	if (myDoorRotateComponent != NULL)
	{
		myDoorInitialVelocity = initialVelocity;
		myInitialPhysicsAngle = myDoorRotateComponent->GetRelativeTransform().GetRotation().Rotator().Yaw;
		myTimeSinceDoorLetGo = 0.0f;
		myHasPhysicsStarted = true;
	}
}

void ADoorItem::ResetRotationTimelineUpdateCallback(float val)
{
	float currRotation;

	//This function is called for every tick in the time-line.
	if (myDoorRotateComponent != NULL)
	{
		currRotation = FMath::Lerp(myInterpDoorRotation.Yaw, myInitialDoorRotation.Yaw, val);
		myDoorRotateComponent->SetRelativeRotation(FRotator(myInterpDoorRotation.Pitch, currRotation, myInterpDoorRotation.Roll));
	}
}

void ADoorItem::ResetRotationTimelineFinishedCallback()
{
	//This function is called when the time-line reaches the finished stage.
	myIsResettingRotation = false;
}

float ADoorItem::CalculateRelativeDoorAngle()
{
	float retVal = 0.0f;
	float d;
	float vt;
	float atSquared;

	//Physics formula used: d = vt - at ^ 2
	vt = myDoorInitialVelocity * myTimeSinceDoorLetGo;
	atSquared = 0.5 * (myDoorFriction * 980) * (myTimeSinceDoorLetGo * myTimeSinceDoorLetGo) * FMath::Sign(myDoorInitialVelocity);
	d = vt - atSquared;

	//Add the initial angle to the current one.
	retVal = myInitialPhysicsAngle + d;

	//Clamp between the minimum and maximum angle.
	return FMath::ClampAngle(retVal, myMinAngle, myMaxAngle);
}

void ADoorItem::CloseDoor()
{
	if (myDoorRotateComponent != NULL)
	{
		myIsResettingRotation = true;
		myInterpDoorRotation = myDoorRotateComponent->GetRelativeTransform().GetRotation().Rotator();

		//Reset rotation time-line
		if (myResetRotationTimeline != NULL)
		{
			myResetRotationTimeline->PlayFromStart();
		}
	}
}

void ADoorItem::StopDoorPhysics()
{
	myHasPhysicsStarted = false;
}

void ADoorItem::OnPlayerDrop(ABaseHand* playerHand)
{
	float doorDistance;
	FRotator doorRelativeRotation;

	Super::OnPlayerDrop(playerHand);

	if (myDoorRotateComponent != NULL)
	{
		//Calculate the velocity of letting go of the door.
		doorRelativeRotation = myDoorRotateComponent->GetRelativeTransform().GetRotation().Rotator();
		doorDistance =  doorRelativeRotation.Yaw - myPreviousRotation.Yaw;

		//Velocity = distance / time, therefore we can do the following.
		this->StartDoorPhysics(doorDistance / FApp::GetDeltaTime());
	}
}