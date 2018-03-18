#include "Smash.h"
#include "Runtime/Engine/Classes/Kismet/KismetMathLibrary.h"
#include "DrawerItem.h"

ADrawerItem::ADrawerItem()
{
	myDrawerInitialVelocity = 0.0f;
	myTimeSinceDrawerLetGo = 0.0f;
	myInitialPhysicsAngle = 0.0f;
	myDrawerComponent = NULL;
}

ADrawerItem::~ADrawerItem()
{

}

void ADrawerItem::BeginPlay()
{
	Super::BeginPlay();

	//Get all the actor components in the Drawer Item blueprint.
	TArray<UActorComponent*> components = this->GetComponentsByClass(UActorComponent::StaticClass());
	for (int currComponentIndex = 0; currComponentIndex < components.Num(); currComponentIndex++)
	{
		if (components[currComponentIndex]->GetName().Equals("DrawerRoot", ESearchCase::IgnoreCase) == true)
		{
			myDrawerComponent = Cast<USceneComponent>(components[currComponentIndex]);
			break;
		}
	}
}

void ADrawerItem::OnPlayerTouched(ABaseHand* playerHand)
{
	FVector ownerLocalSpacePos;

	Super::OnPlayerTouched(playerHand);

	if (playerHand != NULL && myDrawerComponent != NULL)
	{
		myInitialOverlapPosition = playerHand->GetControllerLocation();
		ownerLocalSpacePos = UKismetMathLibrary::InverseTransformLocation(this->GetActorTransform(), myInitialOverlapPosition);
	
		//Calculate the offset of the initial grab and the drawer relative location.
		myCurrentDrawerPosition = myDrawerComponent->RelativeLocation.X;
		myDrawerInitialGrabOffset = ownerLocalSpacePos.X - myCurrentDrawerPosition;

		//Stop the physics of the drawer when we grab it.
		this->StopDrawerPhysics();

		//Play vibration feedback.
		Super::VibrateTouchingHands(EVibrationType::VE_TOUCH);
	}
}

void ADrawerItem::Tick(float deltaTime)
{
	float drawerOffsetDiff;
	FVector ownerLocalSpacePos;

	Super::Tick(deltaTime);

	if (myDrawerComponent != NULL)
	{
		if (myPrimaryHand != NULL)
		{
			//Get the hand location relative to the drawer.
			ownerLocalSpacePos = UKismetMathLibrary::InverseTransformLocation(this->GetActorTransform(), myPrimaryHand->GetControllerLocation());

			//Record the current and previous locations.
			myCurrentDrawerPosition = myDrawerComponent->RelativeLocation.X;
			myPreviousLocation = myCurrentDrawerPosition;

			//Calculate the difference between the hand and the initial location.
			drawerOffsetDiff = ownerLocalSpacePos.X - myDrawerInitialGrabOffset;

			//Record the clamped position and direction of the new drawer location.
			myCurrentDrawerPosition = FMath::Clamp(drawerOffsetDiff, 0.0f, myDrawerLength);
			myCurrentDrawerPositionNormalized = myCurrentDrawerPosition / myDrawerLength;

			//Move the drawer according to the calculation.
			myDrawerComponent->SetRelativeLocation(FVector(myCurrentDrawerPosition, 0.0f, 0.0f));

			//Play vibration feedback if the drawer is at the ends.
			if (myCurrentDrawerPosition >= myDrawerLength || myCurrentDrawerPosition == 0.0f)
			{
				Super::VibrateTouchingHands(EVibrationType::VE_HIT);
			}
		}

		if (myHasPhysicsStarted == true)
		{
			//Keep track of the last time the door was touched.
			myTimeSinceDrawerLetGo += deltaTime;

			if (this->ShouldStopPhysicsSimulation() == true)
			{
				this->StopDrawerPhysics();
			}
			else
			{
				//Set the relative X location of the drawer based on the physics calculation.
				myDrawerComponent->SetRelativeLocation(FVector(this->CalculateRelativeDrawerLocation(), 0.0f, 0.0f));

			}
		}
	}

}

bool ADrawerItem::ShouldStopPhysicsSimulation()
{
	int v;
	int u = myDrawerInitialVelocity;
	int at = (myDrawerFriction * 980) * myTimeSinceDrawerLetGo * FMath::Sign(myDrawerInitialVelocity);
	float currDoorLocation;

	//Physics formula used: v = u - at
	v = u - at;

	if (myDrawerInitialVelocity <= 0)
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

	if (myDrawerComponent != NULL)
	{
		currDoorLocation = myDrawerComponent->GetRelativeTransform().GetLocation().X;
		if (FMath::IsNearlyEqual(currDoorLocation, 0.0f, 0.01f) == true)
		{
			return true;
		}
		if (FMath::IsNearlyEqual(currDoorLocation, myDrawerLength, 0.01f) == true)
		{
			return true;
		}
	}

	return false;
}

void ADrawerItem::StartDrawerPhysics(float initialVelocity)
{
	if (myDrawerComponent != NULL)
	{
		myDrawerInitialVelocity = initialVelocity;
		myInitialPhysicsAngle = myDrawerComponent->GetRelativeTransform().GetRotation().Rotator().Yaw;
		myTimeSinceDrawerLetGo = 0.0f;
		myHasPhysicsStarted = true;
	}
}

float ADrawerItem::CalculateRelativeDrawerLocation()
{
	float drawerMoveAmount;

	//Physics Formula: vt-ut^2
	drawerMoveAmount = myDrawerInitialVelocity * myTimeSinceDrawerLetGo;
	drawerMoveAmount = drawerMoveAmount - (0.5 * (myDrawerFriction * 980) * (myTimeSinceDrawerLetGo * myTimeSinceDrawerLetGo) * FMath::Sign(myDrawerInitialVelocity));

	//Add on the current position of the drawer to accumulate.
	drawerMoveAmount = myCurrentDrawerPosition + drawerMoveAmount;

	//Ensure drawerMoveAmount is within correct bounds.
	return FMath::Clamp(drawerMoveAmount, 0.0f, myDrawerLength);
}

void ADrawerItem::StopDrawerPhysics()
{
	myHasPhysicsStarted = false;
}

void ADrawerItem::OnPlayerDrop(ABaseHand* playerHand)
{
	float drawerDistance;

	Super::OnPlayerDrop(playerHand);

	if (myDrawerComponent != NULL)
	{
		//Calculate the velocity of letting go of the drawer.
		drawerDistance = myDrawerComponent->RelativeLocation.X - myPreviousLocation;

		//Velocity = distance / time, therefore we can do the following.
		this->StartDrawerPhysics(drawerDistance / FApp::GetDeltaTime());
	}
}