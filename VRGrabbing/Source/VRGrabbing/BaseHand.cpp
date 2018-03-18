#include "Smash.h"
#include "Ace.h"
#include "BaseHand.h"
#include "BaseCharacter.h"
#include "Items/BaseItem.h"
#include "Items/BaseWeapon.h"
#include "Animation/AnimBlueprint.h"
#include "GameFramework/InputSettings.h"
#include "Kismet/HeadMountedDisplayFunctionLibrary.h"
#include "MotionControllerComponent.h"

ABaseHand::ABaseHand() : Super()
{
	myIsUsingMotionControllers = true;
	myIsCurrentlyGrabbing = false;
	myIsCurrentlyCrushing = false;
	myReachDistance = 50.0f;
	myDistanceFromController = 10.0f;
	myMinDistanceFromController = 10.0f;
	myMaxDistanceFromController = 250.0f;
	myControllerLocation = FVector::ZeroVector;
	myControllerRotation = FRotator::ZeroRotator;
	myIsItemEquipped = false;

	myGrabbedItem = NULL;
	myGrabbedPhysicsHandle = NULL;

	PrimaryActorTick.bCanEverTick = true;

	myFirstPersonHandMesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("FirstPersonHandMesh"));
	myFirstPersonHandMesh->SetOnlyOwnerSee(false);
	myFirstPersonHandMesh->bCastDynamicShadow = false;
	myFirstPersonHandMesh->CastShadow = false;
	RootComponent = myFirstPersonHandMesh;
	
	myMotionController = CreateDefaultSubobject<UMotionControllerComponent>(TEXT("MotionController"));
	
	//myMotionController->Hand = myControllerHand; DEPRICATED, they changed how this is handled in 4.19
	if (myControllerHand == EControllerHand::Left)
	{
		myMotionController->MotionSource = "LEFT";
	}
	else
	{
		myMotionController->MotionSource = "RIGHT";
	}
	
	myMotionController->SetupAttachment(RootComponent);

	myVRHandMesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("VRHandMesh"));
	myVRHandMesh->SetOnlyOwnerSee(false);			
	myVRHandMesh->bCastDynamicShadow = false;
	myVRHandMesh->CastShadow = false;
	myVRHandMesh->SetupAttachment(myMotionController);

	myCollisionChannels[0] = ECollisionChannel::ECC_Camera;
	myCollisionChannels[1] = ECollisionChannel::ECC_Destructible;
	myCollisionChannels[2] = ECollisionChannel::ECC_MAX;
	myCollisionChannels[3] = ECollisionChannel::ECC_Pawn;
	myCollisionChannels[4] = ECollisionChannel::ECC_PhysicsBody;
	myCollisionChannels[5] = ECollisionChannel::ECC_Vehicle;
	myCollisionChannels[6] = ECollisionChannel::ECC_Visibility;
	myCollisionChannels[7] = ECollisionChannel::ECC_WorldDynamic;
	myCollisionChannels[8] = ECollisionChannel::ECC_WorldStatic;
	myCollisionChannels[9] = ECollisionChannel::ECC_EngineTraceChannel1;
	myCollisionChannels[10] = ECollisionChannel::ECC_EngineTraceChannel2;
	myCollisionChannels[11] = ECollisionChannel::ECC_EngineTraceChannel3;
	myCollisionChannels[12] = ECollisionChannel::ECC_EngineTraceChannel4;
	myCollisionChannels[13] = ECollisionChannel::ECC_EngineTraceChannel5;
	myCollisionChannels[14] = ECollisionChannel::ECC_EngineTraceChannel6;
	myCollisionChannels[15] = ECollisionChannel::ECC_GameTraceChannel1;
	myCollisionChannels[16] = ECollisionChannel::ECC_GameTraceChannel2;
	myCollisionChannels[17] = ECollisionChannel::ECC_GameTraceChannel3;
	myCollisionChannels[18] = ECollisionChannel::ECC_GameTraceChannel4;
	myCollisionChannels[19] = ECollisionChannel::ECC_GameTraceChannel5;
	myCollisionChannels[20] = ECollisionChannel::ECC_GameTraceChannel6;
	myCollisionChannels[21] = ECollisionChannel::ECC_GameTraceChannel7;
	myCollisionChannels[22] = ECollisionChannel::ECC_GameTraceChannel8;
	myCollisionChannels[23] = ECollisionChannel::ECC_GameTraceChannel9;
	myCollisionChannels[24] = ECollisionChannel::ECC_GameTraceChannel10;
	myCollisionChannels[25] = ECollisionChannel::ECC_GameTraceChannel11;
	myCollisionChannels[26] = ECollisionChannel::ECC_GameTraceChannel12;
	myCollisionChannels[27] = ECollisionChannel::ECC_GameTraceChannel13;
	myCollisionChannels[28] = ECollisionChannel::ECC_GameTraceChannel14;
	myCollisionChannels[29] = ECollisionChannel::ECC_GameTraceChannel15;
	myCollisionChannels[30] = ECollisionChannel::ECC_GameTraceChannel16;
	myCollisionChannels[31] = ECollisionChannel::ECC_GameTraceChannel17;
	myCollisionChannels[32] = ECollisionChannel::ECC_GameTraceChannel18;
}

/**
* Ticks.
*
* @param deltaTime The delta time.
*/
void ABaseHand::Tick(float deltaTime)
{
	Super::Tick(deltaTime);

	//Get the VR controller location and rotation if we are using them.
	if (myVRHandMesh != NULL)
	{
		myControllerLocation = myVRHandMesh->GetComponentLocation();
		myControllerRotation = myVRHandMesh->GetComponentRotation();
		myControllerTransform = myVRHandMesh->GetComponentTransform();
	}

	//Update grabbed object location & rotation (if any)
	if (myGrabbedPhysicsHandle != NULL)
	{
		myNewGrabbedLocation = myControllerLocation + (myControllerRotation.Vector());

		myGrabbedPhysicsHandle->SetTargetLocation(myNewGrabbedLocation);
	}
}

/** Functions. */
void ABaseHand::BeginPlay()
{
	//Call the base class  
	Super::BeginPlay();

	//Show or hide the two versions of the gun based on whether or not we're using motion controllers.
	if (myFirstPersonHandMesh != NULL && myVRHandMesh != NULL)
	{
		if (myIsUsingMotionControllers == true)
		{
			myFirstPersonHandMesh->SetHiddenInGame(true, true);
			myVRHandMesh->SetHiddenInGame(false, true);
		}
		else
		{
			myVRHandMesh->SetHiddenInGame(true, true);
			myFirstPersonHandMesh->SetHiddenInGame(false, true);
		}
	}
}

bool ABaseHand::Grab(AActor* actorThatIsGrabbing, bool showDebugLine)
{
	double grabRadius = 7.0;

	FVector lineTraceEnd;
	FVector lineTraceStart;
	AActor* actorHit = NULL;
	FHitResult traceHitResult;

	//Only allow grab if the hand isn't already holding something and it is grabbable.
	if (actorThatIsGrabbing != NULL && myIsItemEquipped == false && myIsCurrentlyGrabbing == false)
	{
		//We are now grabbing.
		myIsCurrentlyGrabbing = true;

		//Update the current location and rotation variables.
		myControllerLocation = myVRHandMesh->GetComponentLocation();
		myControllerRotation = myVRHandMesh->GetComponentRotation();
		myControllerTransform = myVRHandMesh->GetComponentTransform();

		//Set Line Trace (Ray-Cast) endpoints
		lineTraceStart = myControllerLocation;
		lineTraceEnd = lineTraceStart + (myControllerRotation.Vector() * myReachDistance);

		myLastHitDistance = lineTraceEnd - lineTraceStart;

		//Ray trace
		actorHit = ABaseHand::CastCircleOfRays(grabRadius, lineTraceStart, lineTraceEnd, traceHitResult, showDebugLine);

		//Check if there's a valid object to grab
		if (actorHit != NULL)
		{
			//Retain a reference to the owning character and the grabbed item.
			myOwningCharacter = Cast<ABaseCharacter>(actorThatIsGrabbing);
			myGrabbedItem = Cast<ABaseItem>(actorHit);

			//Ensure cast was successful. 
			if (myOwningCharacter != NULL && myGrabbedItem != NULL)
			{
				if (myGrabbedItem->GetIsGrabbable() == true)
				{
					if (myControllerHand == EControllerHand::Left)
					{
						myGrabbedItem->SetLeftOwningHand(this);
					}
					else
					{
						myGrabbedItem->SetRightOwningHand(this);
					}

					//Do the actual attachment of the item to the hand.
					if (Cast<ABaseWeapon>(myGrabbedItem) != NULL)
					{
						this->GrabWeapon(myGrabbedItem);
					}
					else
					{
						this->GrabItem(myGrabbedItem, traceHitResult);
					}
				}
				else if (myGrabbedItem->GetIsTouchable() == true)
				{
					this->TouchItem(myGrabbedItem, traceHitResult);
				}
			}
		}
	}

	return false;
}

bool ABaseHand::GrabItem(ABaseItem* grabbedItem, FHitResult hitResult)
{
	//Get the orientation of the item and hand to use when it is grabbed.
	//Prevent grabbing broken items since this picks up all chunks.
	if (grabbedItem != NULL && grabbedItem->GetIsGrabbable() == true && grabbedItem->GetIsFractured() == false)
	{
		//Attempt to Grab Object
		myGrabbedItem = grabbedItem;

		//Only reset the component state if it is being picked up with no hands currently touching it.
		myGrabbedPrimitiveComponent = Cast<UPrimitiveComponent>(myGrabbedItem->GetGrabbableRootComponent());
		if (myGrabbedPrimitiveComponent != NULL)
		{
			if (myGrabbedPrimitiveComponent->IsPhysicsStateCreated() == true)
			{
				myGrabbedPrimitiveComponent->RecreatePhysicsState();
			}

			myGrabbedPrimitiveComponent->AttachToComponent(myVRHandMesh, FAttachmentTransformRules(EAttachmentRule::KeepWorld, false));
 
			myGrabbedPrimitiveComponent->SetSimulatePhysics(false);
			myGrabbedPrimitiveComponent->SetEnableGravity(false);
			myGrabbedPrimitiveComponent->RecreatePhysicsState();
		}

		//Let the item know it has been grabbed.
		myGrabbedItem->OnPlayerReceive(this);

		return true;
	}

	return false;
}

bool ABaseHand::TouchItem(ABaseItem* grabbedItem, FHitResult hitResult)
{
	if (grabbedItem != NULL && grabbedItem->GetIsTouchable() == true)
	{
		//Let the item know it has been touched.
		grabbedItem->OnPlayerTouched(this);

		return true;
	}

	return false;
}

bool ABaseHand::GrabWeapon(ABaseItem* grabbedItem)
{
	TArray<UObject*> subItems;
	UPrimitiveComponent* grabbedComponent = NULL;
	FTransform itemOnGrabTransform = FTransform();
	FTransform handOnGrabTransform = FTransform();
	UPhysicsHandleComponent* physicsHandle = NULL;

	//Get the orientation of the item and hand to use when it is grabbed.
	//Prevent grabbing broken items since this picks up all chunks.
	if (grabbedItem != NULL && grabbedItem->GetIsGrabbable() == true && grabbedItem->GetIsFractured() == false)
	{
		myGrabbedItem = grabbedItem;
		itemOnGrabTransform = grabbedItem->GetOnGrabItemTransform();

		//Attempt to Grab Object
		grabbedComponent = Cast<UPrimitiveComponent>(grabbedItem->GetGrabbableRootComponent());
		if (grabbedComponent != NULL)
		{
			physicsHandle = myGrabbedItem->FindComponentByClass<UPhysicsHandleComponent>();
			if (physicsHandle != NULL)
			{
				physicsHandle->GrabComponentAtLocationWithRotation(grabbedComponent,
																	FName("PhysicsGrabHandle"),
																	myGrabbedItem->GetActorLocation() + itemOnGrabTransform.GetLocation(),
																	itemOnGrabTransform.GetRotation().Rotator());
			}

			//If object is successfully grabbed, move object with Controller
			if (myGrabbedItem->GetPrimaryHand() != NULL)
			{
				myGrabbedPrimitiveComponent = grabbedComponent;
				myGrabbedPhysicsHandle = physicsHandle;

				if (this == myGrabbedItem->GetPrimaryHand())
				{
					//Let the item know it has been grabbed.
					myGrabbedItem->OnPlayerReceive(this);

					if (myGrabbedItem->GetIsAutomaticallyEquipped() == true)
					{
						ABaseHand::EquipItem();
					}
				}
				else // this is secondary hand.
				{
					//Item has already been recived.
				}

				//Item was sucessfully received.
				return true;
			}
		}
	}

	return false;
}

bool ABaseHand::Crush(int playerStrength)
{
	//TODO: FIX CRUSH ANIMATION.
	if (myGrabbedItem != NULL)
	{	
		//Only crush the item if you have enough strength and is breakable!
		if (myGrabbedItem->GetIsBreakable() == true && playerStrength >= myGrabbedItem->GetDestructibleStrength())
		{
			this->SetIsCurrentlyCrushing(true);
			myGrabbedItem->OnPlayerCrush(this, playerStrength);

			return true;
		}
	}
	
	return false;
}

void ABaseHand::UseItem()
{
	if (myGrabbedItem != NULL)
	{
		myGrabbedItem->OnPlayerUse();
	}
}

void ABaseHand::Drop()
{
	//Do not allow going directly from equip to drop.
	if (myIsItemEquipped == false)
	{
		myIsCurrentlyGrabbing = false;
		myIsCurrentlyCrushing = false;

		if (myGrabbedItem != NULL && this->GetControllerHand() == EControllerHand::Left && myGrabbedItem->GetRightHand() != NULL)
		{
			myGrabbedItem->SetLeftOwningHand(NULL);
			myGrabbedItem = NULL;
		}
		else if (myGrabbedItem != NULL && this->GetControllerHand() == EControllerHand::Right && myGrabbedItem->GetLeftHand() != NULL)
		{
			myGrabbedItem->SetRightOwningHand(NULL);
			myGrabbedItem = NULL;
		}
		else
		{
			if (myGrabbedItem != NULL)
			{
				myGrabbedItem->OnPlayerDrop(this);
				myGrabbedItem = NULL;
			}

			if (myGrabbedPrimitiveComponent != NULL)
			{
				myGrabbedPrimitiveComponent->DetachFromComponent(FDetachmentTransformRules::KeepWorldTransform);
				myGrabbedPrimitiveComponent->SetSimulatePhysics(true);
				myGrabbedPrimitiveComponent = NULL;
			}

			if (myGrabbedPhysicsHandle != NULL)
			{
				//Player has latched on to something, release it
				myGrabbedPhysicsHandle->ReleaseComponent();
				myGrabbedPhysicsHandle = NULL;
			}
		}
	}
}

bool ABaseHand::EquipItem()
{
	if (myGrabbedItem != NULL && myIsItemEquipped == false)
	{
		//Ensure an item is an Equipable.
		if (myGrabbedItem->GetIsEquipable() == true)
		{
			myGrabbedItem->OnPlayerEquip();
			myIsItemEquipped = true;
			return true;
		}
	}

	return false;
}

bool ABaseHand::UnEquipItem()
{
	//Ensure an item is equipped in the current hand.
	if (myGrabbedItem != NULL && myIsItemEquipped == true)
	{
		myIsItemEquipped = false;

		return true;
	}

	return false;
}

AActor* ABaseHand::CastCircleOfRays(double radius, FVector& lineTraceStart, FVector& lineTraceEnd, bool showDebugLines)
{
	AActor* actorHit = NULL;
	double iInRad;
	double xOffset;
	double yOffset;

	//Cast the first ray since its the center point.
	actorHit = ABaseHand::CastRay(lineTraceStart, lineTraceEnd, showDebugLines);

	//Check if the first cast actually got the actor.
	//Dynamically create a circle of rays to cast out from the hand.
	for (int i = 1; i <= 360 && actorHit == NULL; i++)
	{
		iInRad = Ace::DegreesToRadians(i);
		xOffset = radius * Ace::Cos(iInRad);
		yOffset = radius * Ace::Sin(iInRad);

		lineTraceStart = FVector(myControllerLocation.X + xOffset, myControllerLocation.Y + yOffset, myControllerLocation.Z);
		lineTraceEnd = lineTraceStart + (myControllerRotation.Vector() * myReachDistance);

		//Line trace
		actorHit = ABaseHand::CastRay(lineTraceStart, lineTraceEnd, showDebugLines);
	}

	return actorHit;
}

//Also returns the hit result from the line trace.
AActor* ABaseHand::CastCircleOfRays(double radius, FVector& lineTraceStart, FVector& lineTraceEnd, FHitResult& outHitResult, bool showDebugLines)
{
	AActor* actorHit = NULL;
	double iInRad;
	double xOffset;
	double yOffset;

	//Cast the first ray since its the center point.
	actorHit = ABaseHand::CastRay(lineTraceStart, lineTraceEnd, showDebugLines);

	//Check if the first cast actually got the actor.
	//Dynamically create a circle of rays to cast out from the hand.
	for (int i = 1; i <= 360 && actorHit == NULL; i++)
	{
		iInRad = Ace::DegreesToRadians(i);
		xOffset = radius * Ace::Cos(iInRad);
		yOffset = radius * Ace::Sin(iInRad);

		lineTraceStart = FVector(myControllerLocation.X + xOffset, myControllerLocation.Y + yOffset, myControllerLocation.Z);
		lineTraceEnd = lineTraceStart + (myControllerRotation.Vector() * myReachDistance);

		//Line trace
		actorHit = ABaseHand::CastRay(lineTraceStart, lineTraceEnd, outHitResult, showDebugLines);
	}

	return actorHit;
}

/**
* Ray-cast and get any object hit by the line trace.
*
* @param [in,out] lineTraceStart The line trace start.
* @param [in,out] lineTraceEnd   The line trace end.
* @param [in,out] showDebugLine  If true, shows a graphical representation of the ray cast.
*
* @return null if it fails, else a pointer to an AActor.
*/
AActor* ABaseHand::CastRay(FVector& lineTraceStart, FVector& lineTraceEnd, bool showDebugLine)
{
	FHitResult hitResult;
	AActor* actorHit = NULL;
	FCollisionQueryParams traceParameters(FName(TEXT("BaseHandRayCast")), true, GetOwner());

	//Show Debug line (helpful for a visual indicator during testing)
	if (showDebugLine == true)
	{
		DrawDebugLine(GetWorld(), lineTraceStart, lineTraceEnd, FColor(255, 0, 0), false, -1, 0, 1.0f);
	}

	//Do line trace / ray-cast
	return ABaseHand::LineTrace(lineTraceStart, lineTraceEnd, hitResult);
}

/**
* Ray-cast and get any object hit by the line trace.
*
* @param [in,out] lineTraceStart The line trace start.
* @param [in,out] lineTraceEnd   The line trace end.
* @param [out]	  outHitResult	 If not null, the hit result from the line trace.
* @param [in,out] showDebugLine  If true, shows a graphical representation of the ray cast.
*
* @return null if it fails, else a pointer to an AActor.
*/
AActor* ABaseHand::CastRay(FVector& lineTraceStart, FVector& lineTraceEnd, FHitResult& outHitResult, bool showDebugLine)
{
	AActor* actorHit = NULL;
	FCollisionQueryParams traceParameters(FName(TEXT("BaseHandRayCast")), false, GetOwner());

	//Show Debug line (helpful for a visual indicator during testing)
	if (showDebugLine == true)
	{
		DrawDebugLine(GetWorld(), lineTraceStart, lineTraceEnd, FColor(255, 0, 0), false, -1, 0, 1.0f);
	}

	//Do line trace / ray-cast
	return ABaseHand::LineTrace(lineTraceStart, lineTraceEnd, outHitResult);
}

AActor* ABaseHand::LineTrace(FVector& lineTraceStart, FVector& lineTraceEnd, FHitResult& outHitResult)
{
	FHitResult hit;
	AActor* actorHit = NULL;
	int numCollisionChannels = 33;
	FCollisionQueryParams traceParameters(FName(TEXT("BaseHandRayCast")), false, GetOwner());

	//Check all possible channels for collision.
	for (int i = 0; i < numCollisionChannels; i++)
	{
		ECollisionChannel currChannel = myCollisionChannels[i];

		if (&outHitResult == NULL)
		{
			ABaseHand::GetWorld()->LineTraceSingleByObjectType(hit, lineTraceStart, lineTraceEnd, currChannel, traceParameters);
			actorHit = hit.GetActor();
		}
		else
		{
			ABaseHand::GetWorld()->LineTraceSingleByObjectType(outHitResult, lineTraceStart, lineTraceEnd, currChannel, traceParameters);
			actorHit = outHitResult.GetActor();
		}

		if (actorHit != NULL)
		{
			return actorHit;
		}
	}

	return NULL;	
}

void ABaseHand::VibrateController(EVibrationType vibrationType)
{
	APlayerController* playerController = NULL;

	if (myOwningCharacter != NULL)
	{
		//Handle Vibration when picking up something.
		playerController = Cast<APlayerController>(myOwningCharacter->Controller);
		if (playerController != NULL)
		{
			if (vibrationType == EVibrationType::VE_TOUCH)
			{
				playerController->ClientPlayForceFeedback(myPickUpForceFeedback, false, "BaseHandVibrateController_Touch");
			}	
			else if (vibrationType == EVibrationType::VE_PICKUP)
			{
				playerController->ClientPlayForceFeedback(myPickUpForceFeedback, false, "BaseHandVibrateController_Pickup");
			}
			else if (vibrationType == EVibrationType::VE_HIT)
			{
				playerController->ClientPlayForceFeedback(myHitForceFeedback, false, "BaseHandVibrateController_Hit");
			}
			else if (vibrationType == EVibrationType::VE_GLANCEHIT)
			{
				playerController->ClientPlayForceFeedback(myPickUpForceFeedback, false, "BaseHandVibrateController_GlanceHit");
			}
			else if (vibrationType == EVibrationType::VE_DROP)
			{
				playerController->ClientPlayForceFeedback(myHitForceFeedback, false, "BaseHandVibrateController_Drop");
			}
			else if (vibrationType == EVibrationType::VE_CRUSH)
			{
				playerController->ClientPlayForceFeedback(myPickUpForceFeedback, false, "BaseHandVibrateController_Crush");
			}
			else if (vibrationType == EVibrationType::VE_SMALLSHOT)
			{
				playerController->ClientPlayForceFeedback(myHitForceFeedback, false, "BaseHandVibrateController_SmallShot");
			}
			else if (vibrationType == EVibrationType::VE_BIGSHOT)
			{
				playerController->ClientPlayForceFeedback(myHitForceFeedback, false, "BaseHandVibrateController_BigShot");
			}
			else
			{
				playerController->ClientPlayForceFeedback(myPickUpForceFeedback, false, "BaseHandVibrateController_Touch");
			}
		}
	}
}

///Getters
USkeletalMeshComponent* ABaseHand::GetFirstPersonHandMesh()
{
	return myFirstPersonHandMesh;
}

USkeletalMeshComponent* ABaseHand::GetVRHandMesh()
{
	return myVRHandMesh;
}

UMotionControllerComponent* ABaseHand::GetMotionController()
{
	return myMotionController;
}

EControllerHand ABaseHand::GetControllerHand()
{
	return myControllerHand;
}

bool ABaseHand::GetIsItemEquipped()
{
	return myIsItemEquipped;
}

bool ABaseHand::GetIsCurrentlyGrabbing()
{
	return myIsCurrentlyGrabbing;
}

bool ABaseHand::GetIsCurrentlyCrushing()
{
	return myIsCurrentlyCrushing;
}

float ABaseHand::GetDistanceFromController()
{
	return myDistanceFromController;
}

float ABaseHand::GetReachDistance()
{
	return myReachDistance;
}

float ABaseHand::GetMinDistanceFromController()
{
	return myMinDistanceFromController;
}

float ABaseHand::GetMaxDistanceFromController()
{
	return myMaxDistanceFromController;
}

bool ABaseHand::GetIsUsingMotionControllers()
{
	return myIsUsingMotionControllers;
}

FVector ABaseHand::GetControllerLocation()
{
	return myControllerLocation;
}

FRotator ABaseHand::GetControllerRotation()
{
	return myControllerRotation;
}

UPrimitiveComponent* ABaseHand::GetGrabbedPrimitiveComponent()
{
	return myGrabbedPrimitiveComponent;
}

FTransform ABaseHand::GetGrabbedPhysicsHandleTransform()
{
	return myGrabbedPhysicsHandleTransform;
}

FTransform ABaseHand::GetControllerTransform()
{
	return myControllerTransform;
}

FVector ABaseHand::GetControllerRelativeLocation()
{
	return myMotionController->RelativeLocation;
}

FRotator ABaseHand::GetControllerRelativeRotation()
{
	return myMotionController->RelativeRotation;
}

FVector ABaseHand::GetNewGrabbedLocation()
{
	return myNewGrabbedLocation;
}

FVector ABaseHand::GetLastHitDiffrence()
{
	return myLastHitDistance;
}

FVector ABaseHand::GetDiffGrabbedLocation()
{
	return myDiffGrabbedLocation;
}

FQuat ABaseHand::GetDiffGrabbedRotation()
{
	return myDiffGrabbedRotation;
}

ABaseItem* ABaseHand::GetGrabbedItem()
{
	return myGrabbedItem;
}

UPhysicsHandleComponent* ABaseHand::GetGrabbedPhysicsHandle()
{
	return myGrabbedPhysicsHandle;
}

ABaseCharacter* ABaseHand::GetOwningCharacter()
{
	return myOwningCharacter;
}

float ABaseHand::GetOwnerStrength()
{
	if (myOwningCharacter != NULL)
	{
		return myOwningCharacter->GetStrength();
	}

	return -1.0f;
}

///Setters
void ABaseHand::SetMinDistanceFromController(float minDistance)
{
	myMinDistanceFromController = minDistance;
}

void ABaseHand::SetMaxDistanceFromController(float maxDistance)
{
	myMaxDistanceFromController = maxDistance;
}

void ABaseHand::SetIsUsingMotionControllers(bool isUsingMotionControllers)
{
	myIsUsingMotionControllers = isUsingMotionControllers;
}

void ABaseHand::SetControllerLocation(FVector controllerLocation)
{
	myControllerLocation = controllerLocation;
}

void ABaseHand::SetControllerRotation(FRotator controllerRotation)
{
	myControllerRotation = controllerRotation;
}

void ABaseHand::SetDistanceFromController(float newDistance)
{
	if (newDistance >= myMinDistanceFromController && newDistance <= myMaxDistanceFromController)
	{
		myDistanceFromController = newDistance;
	}
}

void ABaseHand::SetGrabbedItem(ABaseItem* grabbedItem)
{
	myGrabbedItem = grabbedItem;
}

void ABaseHand::SetGrabbedPrimitiveComponent(UPrimitiveComponent* grabbedPrimitiveComponent)
{
	myGrabbedPrimitiveComponent = grabbedPrimitiveComponent;
}

void ABaseHand::SetIsItemEquipped(bool isItemEquipped)
{
	myIsItemEquipped = isItemEquipped;
}

void ABaseHand::SetIsCurrentlyGrabbing(bool isCurrentlyGrabbing)
{
	myIsCurrentlyGrabbing = isCurrentlyGrabbing;
}

void ABaseHand::SetGrabbedObjectTransform(FTransform grabbedObjectTransform)
{
	myGrabbedObjectTransform = grabbedObjectTransform;
}

void ABaseHand::SetControllerTransform(FTransform controllerTransform)
{
	myControllerTransform = controllerTransform;
}

void ABaseHand::SetIsCurrentlyCrushing(bool isCurrentlyCrushing)
{
	myIsCurrentlyCrushing = isCurrentlyCrushing;
}

void ABaseHand::SetOwningCharacter(ABaseCharacter* owningCharacter)
{
	myOwningCharacter = owningCharacter;
}