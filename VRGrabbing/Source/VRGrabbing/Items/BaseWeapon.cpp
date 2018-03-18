#include "Smash.h"
#include "Ace.h"
#include "BaseWeapon.h"

///Constructor
ABaseWeapon::ABaseWeapon() : Super()
{
	myWeaponType = EWeaponType::VE_ONE_HANDED;
	myDamageModifier = 0;
	myIsEquipable = true;
	myIsAutomaticallyEquipped = true;
	myIsFullAutomatic = false;

	//Set the local rotation/locations for when the item is grabbed/equipped.
	myOnGrabItemTransform = FTransform(FRotator::ZeroRotator, FVector::ZeroVector);
}

///Functions
FText ABaseWeapon::RetrieveStats()
{
	int sizeOfBuffer = 0;
	char* strWeaponType = NULL;
	char* buffer = NULL;
	FText retVal = FText();

	if (myWeaponType == EWeaponType::VE_ONE_HANDED)
	{
		strWeaponType = Ace::AllocateAndCopyString("One Handed");
	}
	else if (myWeaponType == EWeaponType::VE_TWO_HANDED)
	{
		strWeaponType = Ace::AllocateAndCopyString("Two Handed");
	}
	else if (myWeaponType == EWeaponType::VE_THROWING)
	{
		strWeaponType = Ace::AllocateAndCopyString("Throwing");
	}
	else
	{
		strWeaponType = Ace::AllocateAndCopyString("Unknown");
	}

	//Size of the format string + length of item name + length of the weapon type name + length of each number + length of item description + 1 for null terminator.
	sizeOfBuffer = 100 + myItemName.ToString().Len() + strlen(strWeaponType) + Ace::CalculateNumLength(myDamageModifier) + myItemDescription.ToString().Len() + 1;
	if (sizeOfBuffer < MAX_STRING_BUFFER_SIZE)
	{
		buffer = (char*)calloc(sizeOfBuffer, sizeof(char));

		//Don't use + to concatenate strings because it will create a temp string for every + and cause a memory leak.
		sprintf_s(buffer, sizeOfBuffer * sizeof(char), "%S \n\nWeapon Type:%s \nWeapon Damage:%d \n\n%S", myItemName.ToString().GetCharArray().GetData(), strWeaponType, myDamageModifier, myItemDescription.ToString().GetCharArray().GetData());

		//Convert it to FText since UE4 works best with it.
		retVal = FText::FromString(buffer);
	}

	_aligned_free(strWeaponType);

	return retVal;
}

///Handlers
void ABaseWeapon::BeginPlay()
{
	Super::BeginPlay();
}

void ABaseWeapon::OnPlayerReceive(ABaseHand* owningHand)
{
	UPrimitiveComponent* grabbedPrimitiveComponent = NULL;

	if (owningHand != NULL)
	{
		grabbedPrimitiveComponent = owningHand->GetGrabbedPrimitiveComponent();
		if (grabbedPrimitiveComponent != NULL)
		{
			if (owningHand == myLeftHand)
			{
				grabbedPrimitiveComponent->RelativeRotation = FRotator(grabbedPrimitiveComponent->RelativeRotation.Pitch, grabbedPrimitiveComponent->RelativeRotation.Yaw + 180, grabbedPrimitiveComponent->RelativeRotation.Roll);
			}

			//Handle attachment of weapon to hand.
			grabbedPrimitiveComponent->AttachToComponent(owningHand->GetVRHandMesh(), FAttachmentTransformRules(EAttachmentRule::KeepRelative, EAttachmentRule::KeepRelative, EAttachmentRule::KeepWorld, true), FName::FName("Palm"));

			grabbedPrimitiveComponent->SetSimulatePhysics(false);

			grabbedPrimitiveComponent->SetRelativeLocation(myOnGrabItemTransform.GetLocation());
			grabbedPrimitiveComponent->SetRelativeRotation(myOnGrabItemTransform.GetRotation());
		}
	}

	Super::VibrateTouchingHands(EVibrationType::VE_PICKUP);
}

void ABaseWeapon::NotifyHit(UPrimitiveComponent* collisionComp, AActor* otherActor, UPrimitiveComponent* otherCollisionComp, bool bSelfMoved, FVector hitLocation, FVector hitNormal, FVector normalImpulse, const FHitResult& hitResult)
{
	//Call the parent's NotifyHit() function.
	Super::NotifyHit(collisionComp, otherActor, otherCollisionComp, bSelfMoved, hitLocation, hitNormal, normalImpulse, hitResult);

	////Vibrate the controller if the collision was from the player.
	//if (myOwningHand != NULL)
	//{
	//	//TODO: DEPENDING ON FOCE OF A HIT, CHOOSE BETWEEN ARRAY OF VIBRATIONS TO INDICATE HOW HARD THEY HIT.
	//	myOwningHand->VibrateController(EVibrationType::VE_HIT);
	//}
}

///Getters
EWeaponType ABaseWeapon::GetWeaponType()
{
	return myWeaponType;
}

///Setters
void ABaseWeapon::SetWeaponType(EWeaponType weaponType)
{
	myWeaponType = weaponType;
}



