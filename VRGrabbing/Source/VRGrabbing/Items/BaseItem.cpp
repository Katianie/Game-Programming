#include "Smash.h"
#include "Ace.h"
#include "DestructibleMesh.h"
#include "PhysicsEngine/PhysicsAsset.h"
#include "BaseItem.h"
 
///Constructor
ABaseItem::ABaseItem() : Super()
{
	//Loveless
	myIsActive = true;
	myIsEquipable = false;
	myIsHarvestable = false;
	myIsTouchable = true;
	myIsGrabbable = true;
	myIsBreakable = true;
	myIsFractured = false;
	myIsFullAutomatic = false;
	myHasBeenTouchedOnce = false;
	myMass = 1.0f;
	myItemName = FText::FromString("Default item name.");
	myGrabbableRootComponentName = FText();
	myGrabbableSecondaryComponentName = FText();
	myItemDescription = FText::FromString("Default item description.");
	myCollidedActor = NULL;
	myInventoryTexture = NULL;
	myPickupItemSound = NULL;
	myDropItemSound = NULL;
	myDestructibleComponent = NULL;
	myGrabbedPhysicsHandle = NULL;
	myOnEquipAnimationSequence = NULL;
	myLeftHand = NULL;
	myRightHand = NULL;
	myPrimaryHand = NULL;
	mySecondaryHand = NULL;
	myOnGrabItemTransform = FTransform(FRotator::ZeroRotator, FVector::ZeroVector);

	//Enable Tick for BaseItem(s).
	PrimaryActorTick.bCanEverTick = true;
}

///Destructor
ABaseItem::~ABaseItem()
{
	if (myGrabbedPhysicsHandle != NULL && myGrabbedPhysicsHandle->IsValidLowLevel() == true)
	{
		myGrabbedPhysicsHandle->ConditionalBeginDestroy();
		myGrabbedPhysicsHandle = NULL;
	}
}

///Functions
//Called when the item is spawned.
void ABaseItem::BeginPlay()
{
	FScriptDelegate fractureDelegateFunction;

	Super::BeginPlay();

	//Find the destructible component from the blueprint. 
	myDestructibleComponent = this->FindComponentByClass<UDestructibleComponent>();

	if (myDestructibleComponent != NULL)
	{
		//Register a function listener so we know when the destructible has broke/shattered/fractured.
		fractureDelegateFunction.BindUFunction(this, "NotifyFracture");
		myDestructibleComponent->OnComponentFracture.Add(fractureDelegateFunction);
	}

	if (myIsGrabbable == true)
	{
		//Create and attach a PhysicsHandle to this item so it can be grabbed.
		myGrabbedPhysicsHandle = NewObject<UPhysicsHandleComponent>(this);
		this->AddInstanceComponent(myGrabbedPhysicsHandle);
		myGrabbedPhysicsHandle->OnComponentCreated();
		myGrabbedPhysicsHandle->RegisterComponent();

		myGrabbedPhysicsHandle->AngularStiffness = MAXINT32;
		myGrabbedPhysicsHandle->LinearStiffness = MAXINT32;
		myGrabbedPhysicsHandle->LinearDamping = 0;
		myGrabbedPhysicsHandle->AngularDamping = 0;
	}
}

void ABaseItem::Tick(float deltaTime)
{
	Super::Tick(deltaTime);

	//Allows the destructible component to detect collision again once it has been grabbed.
	if (this->GetIsBeingTouched() == false && myDestructibleComponent != NULL)
	{
		myDestructibleComponent->SetEnableGravity(true);
		myDestructibleComponent->SetSimulatePhysics(true);
	}
}

//Returns a formated string containing the stats for an item.
FText ABaseItem::RetrieveStats()
{
	//Size of the format string, +2 for new line character, +1 for ending null terminator.
	int bufferSize = myItemName.ToString().Len() + 2 + myItemDescription.ToString().Len() + 1;
	char* buffer = NULL;
	FText retVal = FText();

	if (bufferSize < MAX_STRING_BUFFER_SIZE)
	{
		buffer = (char*)calloc(bufferSize, sizeof(char));

		//Don't use + to concatenate strings because it will create a temp string for every + and cause a memory leak.
		sprintf_s(buffer, bufferSize, "%S%s%S", myItemName.ToString().GetCharArray().GetData(), "\n", myItemDescription.ToString().GetCharArray().GetData());

		//Convert it to FText since UE4 works best with it.
		retVal = FText::FromString(buffer);
	}

	return retVal;
}

//Searches all components of this item and returns the first occurence of the component 
//specified by componentName. If componentName is empty, the root component will be returned.
//If the specified component is not found, NULL will be returned.
USceneComponent* ABaseItem::FindSubComponentByName(FText componentName)
{
	USceneComponent* retVal = NULL;
	TArray<UObject*> subComponents;

	if (componentName.IsEmptyOrWhitespace() == true)
	{
		retVal = Super::GetRootComponent();
	}
	else
	{
		this->GetDefaultSubobjects(subComponents);
		for (int currSubComponentIndex = 0; currSubComponentIndex < subComponents.Num() && retVal == NULL; currSubComponentIndex++)
		{
			//Find the correpsonding subcomponent based on myGrabableRootComponentName.
			if (subComponents[currSubComponentIndex]->GetName().Equals(componentName.ToString(), ESearchCase::IgnoreCase) == true)
			{
				retVal = Cast<USceneComponent>(subComponents[currSubComponentIndex]);
			}
		}
	}

	return retVal;
}

//This handles the collision of an grabbed item and another item.
//This is because the grabbed item's physics is disabled by default
//and thus cannot detect collision while being held.
void ABaseItem::NotifyActorBeginOverlap(AActor* otherActor)
{
	//We don't care about the hand collision here.
	if (otherActor->GetName().Contains("hand") == false)
	{
		//Make sure this collision is not overlapping itself.
		if (this->GetName().Equals(otherActor->GetName()) == false)
		{
			UPrimitiveComponent* rootPrimitiveComponent = Cast<UPrimitiveComponent>(this->GetRootComponent());
			UPrimitiveComponent* otherPrimitiveComponent = Cast<UPrimitiveComponent>(otherActor->GetRootComponent());

			this->NotifyHit(rootPrimitiveComponent, otherActor, otherPrimitiveComponent, false, this->GetActorLocation(), this->GetActorLocation().GetSafeNormal(), FVector::ZeroVector, FHitResult());
		}
	}

	//Call parent function
	Super::NotifyActorBeginOverlap(otherActor);
}

void ABaseItem::NotifyHit(UPrimitiveComponent* collisionComp, AActor* otherActor, UPrimitiveComponent* otherCollisionComp, bool bSelfMoved, FVector hitLocation, FVector hitNormal, FVector normalImpulse, const FHitResult& hitResult)
{
	FVector thisActorOrigin = FVector::ZeroVector;
	FVector thisActorBounds = FVector::ZeroVector;
	FVector otherActorOrigin = FVector::ZeroVector;
	FVector otherActorBounds = FVector::ZeroVector;
	FVector thisVelocityVector = FVector::ZeroVector;
	FVector otherVelocityVector = FVector::ZeroVector;
	ABaseItem* otherItem = NULL;
	float thisArea = 0.0f;
	float otherArea = 0.0f;
	float hitStrength = 800.0f;
	float thisVelocity = 0.0f;
	float otherVelocity = 0.0f;
	float thisForceOfImpact = 0.0f;
	float otherForceOfImpact = 0.0f;
	
	//Call the parent's NotifyHit() function.
	Super::NotifyHit(collisionComp, otherActor, otherCollisionComp, bSelfMoved, hitLocation, hitNormal, normalImpulse, hitResult);

	if (this != otherActor)
	{
		myCollidedActor = otherActor;
	}

	if (myDestructibleComponent != NULL && myDestructibleComponent->IsSimulatingPhysics() == false)
	{
		myDestructibleComponent->SetSimulatePhysics(true);
		if (this->GetIsBeingTouched() == true)
		{
			myDestructibleComponent->SetEnableGravity(false);
		}
		else
		{
			myDestructibleComponent->SetEnableGravity(true);
		}
	}

	if (myCollidedActor != NULL)
	{
		//We only need the speed component of the velocity (size of vector) to calculate the force.
		thisVelocityVector = this->GetVelocity();
		thisVelocity = thisVelocityVector.Size();
		if (thisVelocity == 0.0f && myDestructibleComponent != NULL)
		{
			//Item is a destructible so its velocity is there instead.
			thisVelocityVector = myDestructibleComponent->GetPhysicsLinearVelocity();
			thisVelocity = thisVelocityVector.Size();
		}
		
		//Force = mass * acceleration (for us technically Momentum = mass * velocity). 
		thisForceOfImpact = (myMass * thisVelocity);
		
		//Calculate the area so we know how big to make the damage radius.
		this->GetActorBounds(false, thisActorOrigin, thisActorBounds);
		thisArea = thisActorBounds.X * thisActorBounds.Y * thisActorBounds.Z;

		//Check if collision was with two BaseItems or just one.
		otherItem = Cast<ABaseItem>(myCollidedActor);
		if (otherItem != NULL && myDestructibleComponent != NULL && otherItem->GetDestructibleComponent() != NULL)
		{
			if (otherItem->GetDestructibleComponent()->IsSimulatingPhysics() == false)
			{
				otherItem->GetDestructibleComponent()->SetSimulatePhysics(true);

				if (this->GetIsBeingTouched() == true)
				{
					otherItem->GetDestructibleComponent()->SetEnableGravity(false);
				}
				else
				{
					otherItem->GetDestructibleComponent()->SetEnableGravity(true);
				}
			}

			//We only need the speed component of the velocity (size of vector) to calculate the force.
			otherVelocityVector = otherItem->GetVelocity();
			otherVelocity = otherVelocityVector.Size();
			if (otherVelocity == 0.0f && otherItem->GetDestructibleComponent() != NULL)
			{
				//Item is a destructible so its velocity is there instead.
				otherVelocityVector = otherItem->GetDestructibleComponent()->GetPhysicsLinearVelocity();
				otherVelocity = otherVelocityVector.Size();
			}

			//Calculate the momentum of the other item.
			otherForceOfImpact = (otherItem->GetMass() * otherVelocity);

			//Calculate the area of the other item.
			otherItem->GetActorBounds(false, otherActorOrigin, otherActorBounds);
			otherArea = otherActorBounds.X * otherActorBounds.Y * otherActorBounds.Z;

			if (myIsBreakable == true && otherForceOfImpact >= this->GetDestructibleStrength())
			{
				//UE_LOG(LogTemp, Error, TEXT("(this)%s->forceOfImpact:%f"), *this->GetName(), thisForceOfImpact);
				//UE_LOG(LogTemp, Error, TEXT("%s->otherForceOfImpact:%f"), *this->GetName(), otherForceOfImpact);
				//UE_LOG(LogTemp, Error, TEXT("(this)%s->DestructibleStrength:%f"), *this->GetName(), this->GetDestructibleStrength());
				//myDestructibleComponent->ApplyRadiusDamage(otherItem->GetDestructibleStrength(), hitLocation, thisArea, otherForceOfImpact, true);
			}

			if (otherItem->GetIsBreakable() == true && thisForceOfImpact >= otherItem->GetDestructibleStrength())
			{
				//UE_LOG(LogTemp, Error, TEXT("%s->forceOfImpact:%f"), *otherActor->GetName(), otherForceOfImpact);
				//UE_LOG(LogTemp, Error, TEXT("%s->thisForceOfImpact:%f"), *otherActor->GetName(), thisForceOfImpact);
				//UE_LOG(LogTemp, Error, TEXT("%s->DestructibleStrength:%f"), *otherActor->GetName(), otherItem->GetDestructibleStrength());
				//otherItem->GetDestructibleComponent()->ApplyRadiusDamage(this->GetDestructibleStrength(), hitLocation, otherArea, thisForceOfImpact, true);
			}
		}
		else
		{
			////Apply damage to the specified area. In some cases, this is the only way to break the destructible.
			//if (myDestructibleComponent != NULL && myIsBreakable == true)
			//{
			//	myDestructibleComponent->ApplyRadiusDamage(hitStrength, hitLocation, thisArea, thisForceOfImpact, true);
			//}
		}
	}

	//UE_LOG(LogTemp, Error, TEXT("(this)%s->forceOfImpact:%f		%s->forceOfImpact:%f"), *this->GetName(), thisForceOfImpact, *otherActor->GetName(), otherForceOfImpact);
}

void ABaseItem::NotifyFracture(const FVector& hitPosition, const FVector& hitDirection)
{
	myIsFractured = true;

	//Make it so this item can no longer be picked up. 
	//This is because it would pick up all the chunks at once.
	myIsGrabbable = false;

	if (this->GetIsBeingTouched() == true)
	{
		if (myRightHand != NULL)
		{
			myRightHand->Drop();
		}
		if (myLeftHand != NULL)
		{
			myLeftHand->Drop();
		}
	}
}

void ABaseItem::OnPlayerEquip()
{
	//TODO: Play small vibration when equipped.
}

void ABaseItem::OnPlayerUse()
{
	//TODO: Play small vibration when used.
}

void ABaseItem::OnPlayerTouched(ABaseHand* playerHand)
{
	if (playerHand != NULL)
	{
		myHasBeenTouchedOnce = true;

		//Retain a reference to the first hand that received the item.
		if (myPrimaryHand == NULL)
		{
			myPrimaryHand = playerHand;
		}

		//Retain a reference to the hand that received the item.
		if (playerHand->GetControllerHand() == EControllerHand::Left)
		{
			myLeftHand = playerHand;
		}
		else
		{
			myRightHand = playerHand;
		}

		//Use the touch vibration on the controller.
		ABaseItem::VibrateTouchingHands(EVibrationType::VE_TOUCH);
	}
}

void ABaseItem::OnPlayerReceive(ABaseHand* owningHand)
{
	if (owningHand != NULL)
	{
		//Retain a reference to the first hand that received the item.
		if (myPrimaryHand == NULL)
		{
			myPrimaryHand = owningHand;
		}

		//Retain a reference to the hand that received the item.
		if (owningHand->GetControllerHand() == EControllerHand::Left)
		{
			myLeftHand = owningHand;
		}
		else
		{
			myRightHand = owningHand;
		}

		//Use the pickup vibration on the controller.
		owningHand->VibrateController(EVibrationType::VE_PICKUP);

		//Something general for all items on pickup, like playing a sound.
		if (myPickupItemSound != NULL)
		{
			//Loud as shit.
			UGameplayStatics::PlaySoundAtLocation(this, myPickupItemSound, this->GetActorLocation());
		}

		//Use the touch vibration on the controller.
		ABaseItem::VibrateTouchingHands(EVibrationType::VE_PICKUP);
	}
}

void ABaseItem::OnPlayerDrop(ABaseHand* owningHand)
{
	if (owningHand != NULL)
	{
		if (myPrimaryHand == owningHand)
		{
			if (owningHand->GetControllerHand() == EControllerHand::Left && myRightHand != NULL)
			{
				myPrimaryHand = myRightHand;
				mySecondaryHand = NULL;
			}
			else if (owningHand->GetControllerHand() == EControllerHand::Right && myLeftHand != NULL)
			{
				myPrimaryHand = myLeftHand;
				mySecondaryHand = NULL;
			}
			else
			{
				myPrimaryHand = NULL;
			}
		}

		if (owningHand->GetControllerHand() == EControllerHand::Left)
		{
			myLeftHand = NULL;
		}
		else
		{
			myRightHand = NULL;
		}

		//Something general for all items on drop, like playing a sound.
		if (myDropItemSound != NULL)
		{
			//Loud as shit.
			UGameplayStatics::PlaySoundAtLocation(this, myDropItemSound, this->GetActorLocation());
		}

		ABaseItem::VibrateTouchingHands(EVibrationType::VE_DROP);
	}

}

void ABaseItem::OnPlayerCrush(ABaseHand* owningHand, int playerStrength)
{
	if (myDestructibleComponent != NULL && myIsBreakable == true)
	{
		myDestructibleComponent->ApplyDamage(playerStrength,
											 this->GetActorLocation(), 
											 FVector(0.0f, 0.0f, -1.0f),
											 10);

		ABaseItem::VibrateTouchingHands(EVibrationType::VE_CRUSH);
	}
}

void ABaseItem::VibrateTouchingHands(EVibrationType vibrationType)
{
	if (myPrimaryHand != NULL)
	{
		myPrimaryHand->VibrateController(vibrationType);
	}
	if (mySecondaryHand != NULL)
	{
		mySecondaryHand->VibrateController(vibrationType);
	}
}

///Getters
bool ABaseItem::GetIsActive()
{
	return myIsActive;
}

bool ABaseItem::GetIsHarvestable()
{
	return myIsHarvestable;
}

bool ABaseItem::GetIsBreakable()
{
	return myIsBreakable;
}

bool ABaseItem::GetIsTouchable()
{
	return myIsTouchable;
}

bool ABaseItem::GetIsGrabbable()
{
	return myIsGrabbable;
}

bool ABaseItem::GetIsEquipable()
{
	return myIsEquipable;
}

bool ABaseItem::GetIsFractured()
{
	return myIsFractured;
}

bool ABaseItem::GetIsFullAutomatic()
{
	return myIsFullAutomatic;
}

bool ABaseItem::GetIsBeingTouched()
{
	return myLeftHand != NULL || myRightHand != NULL;
}

bool ABaseItem::GetIsAutomaticallyEquipped()
{
	return myIsAutomaticallyEquipped;
}

float ABaseItem::GetDestructibleStrength()
{
	if (myDestructibleComponent != NULL)
	{
		UDestructibleMesh* destructMesh = myDestructibleComponent->GetDestructibleMesh();
		if (destructMesh != NULL)
		{
			return destructMesh->DefaultDestructibleParameters.DamageParameters.DamageThreshold;
		}
	}

	//By default, make it as strong as possible (virtually indestructible).
	return INT_MAX;
}

float ABaseItem::GetMass()
{
	return myMass;
}

FText ABaseItem::GetItemName()
{
	return myItemName;
}

FText ABaseItem::GetItemDescription()
{
	return myItemDescription;
}

AActor* ABaseItem::GetCollidedActor()
{
	return myCollidedActor;
}

UTexture2D* ABaseItem::GetInventoryTexture()
{
	return myInventoryTexture;
}

ABaseHand* ABaseItem::GetLeftHand()
{
	return myLeftHand;
}

ABaseHand* ABaseItem::GetRightHand()
{
	return myRightHand;
}

ABaseHand* ABaseItem::GetPrimaryHand()
{
	return myPrimaryHand;
}

ABaseHand* ABaseItem::GetSecondaryHand()
{
	return mySecondaryHand;
}

UAnimSequence* ABaseItem::GetOnEquipAnimationSequence()
{
	return myOnEquipAnimationSequence;
}

UPhysicsHandleComponent* ABaseItem::GetGrabbedPhysicsHandle()
{
	return myGrabbedPhysicsHandle;
}

UDestructibleComponent* ABaseItem::GetDestructibleComponent()
{
	return myDestructibleComponent;
}

FTransform ABaseItem::GetOnGrabItemTransform()
{
	return myOnGrabItemTransform;
}

USceneComponent* ABaseItem::GetGrabbableRootComponent()
{
	return ABaseItem::FindSubComponentByName(myGrabbableRootComponentName);
}

USceneComponent* ABaseItem::GetGrabbableSecondaryComponent()
{
	return ABaseItem::FindSubComponentByName(myGrabbableSecondaryComponentName);
}

///Setters
void ABaseItem::SetIsActive(bool isActive)
{
	myIsActive = isActive;
}

void ABaseItem::SetIsHarvestable(bool isHarvestable)
{
	myIsHarvestable = isHarvestable;
}

void ABaseItem::SetIsBreakable(bool isBreakable)
{
	myIsBreakable = isBreakable;
}

void ABaseItem::SetIsEquipable(bool isEquipable)
{
	myIsEquipable = isEquipable;
}

void ABaseItem::SetIsGrabbable(bool isGrabbable)
{
	myIsGrabbable = isGrabbable;
}

void ABaseItem::SetItemName(FText itemName)
{
	myItemName = itemName;
}

void ABaseItem::SetItemName(FString itemName)
{
	myItemName = FText::FromString(itemName);
}

void ABaseItem::SetItemDescription(FText itemDescription)
{
	myItemDescription = itemDescription;
}

void ABaseItem::SetItemDescription(FString itemDesctiption)
{
	myItemDescription = FText::FromString(itemDesctiption);
}

void ABaseItem::SetInventoryTexture(UTexture2D* inventoryTexture)
{
	myInventoryTexture = inventoryTexture;
}

void ABaseItem::SetLeftOwningHand(ABaseHand* owningLeftHand)
{
	myLeftHand = owningLeftHand;

	//Set the primary hand as the first hand to grab.
	if (myRightHand == NULL)
	{
		myPrimaryHand = myLeftHand;
	}
	else
	{
		mySecondaryHand = myLeftHand;
	}
}

void ABaseItem::SetRightOwningHand(ABaseHand* owningRightHand)
{
	myRightHand = owningRightHand;

	//Set the primary hand as the first hand to grab.
	if (myLeftHand == NULL)
	{
		myPrimaryHand = myRightHand;
	}
	else
	{
		mySecondaryHand = myRightHand;
	}
}

void ABaseItem::SetOnEquipAnimationSequence(UAnimSequence* onEquipAnimationSequence)
{
	myOnEquipAnimationSequence = onEquipAnimationSequence;
}

void ABaseItem::SetGrabbedPhysicsHandle(UPhysicsHandleComponent* physHandle)
{
	myGrabbedPhysicsHandle = physHandle;
}

void ABaseItem::SetOnGrabItemTransform(FTransform onGrabItemTransform)
{
	myOnGrabItemTransform = onGrabItemTransform;
}