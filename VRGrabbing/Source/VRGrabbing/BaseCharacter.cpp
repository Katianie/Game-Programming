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

DEFINE_LOG_CATEGORY_STATIC(LogFPChar, Warning, All);

/** Constructor. */
ABaseCharacter::ABaseCharacter()
{
	//Enable actor ticking.
	PrimaryActorTick.bCanEverTick = true; 

	// Set size for collision capsule
	GetCapsuleComponent()->InitCapsuleSize(5.0f, 96.0f);

	// set our turn rates for input
	myBaseTurnRate = 45.0f;	
	myBaseLookUpRate = 45.0f;

	myStrength = 800;
	
	// Create a CameraComponent	
	myFirstPersonCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("FirstPersonCamera"));
	myFirstPersonCamera->SetupAttachment(GetCapsuleComponent());
	myFirstPersonCamera->RelativeLocation = FVector(-39.56f, 1.75f, 64.0f); // Position the camera
	myFirstPersonCamera->bUsePawnControlRotation = false;

	// Create a mesh component that will be used when being viewed from a '1st person' view (when controlling this pawn)
	myFirstPersonMesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("FirstPersonMesh"));
	myFirstPersonMesh->SetOnlyOwnerSee(true);
	myFirstPersonMesh->SetupAttachment(myFirstPersonCamera);
	myFirstPersonMesh->bCastDynamicShadow = false;
	myFirstPersonMesh->CastShadow = true;
	//myFirstPersonMesh->RelativeRotation = FRotator(1.9f, -19.19f, 5.2f);
	//myFirstPersonMesh->RelativeLocation = FVector(-0.5f, -4.4f, -155.7f);

	myLeftHand = NULL;
	myRightHand = NULL;
}

/**
 * Virtual Override Functions.
 *
 * @param [in,out] playerInputComponent If non-null, the player input component.
 */
void ABaseCharacter::SetupPlayerInputComponent(UInputComponent* playerInputComponent)
{
	//Setup game play key bindings
	check(playerInputComponent);

	//Player Jump.
	playerInputComponent->BindAction("Jump", IE_Pressed, this, &ACharacter::Jump);
	playerInputComponent->BindAction("Jump", IE_Released, this, &ACharacter::StopJumping);

	//Grab and Release: Left Hand
	playerInputComponent->BindAction("GrabLeftHand", IE_Pressed, this, &ABaseCharacter::OnLeftTriggerPress);
	playerInputComponent->BindAction("GrabLeftHand", IE_Released, this, &ABaseCharacter::OnLeftTriggerRelease);

	//Grab and Release: Right Hand
	playerInputComponent->BindAction("GrabRightHand", IE_Pressed, this, &ABaseCharacter::OnRightTriggerPress);
	playerInputComponent->BindAction("GrabRightHand", IE_Released, this, &ABaseCharacter::OnRightTriggerRelease);

	//Use Item: Left Hand
	playerInputComponent->BindAction("UseItemLeftHand", IE_Pressed, this, &ABaseCharacter::OnLeftTriggerPress);

	//Use Item: Right Hand
	playerInputComponent->BindAction("UseItemRightHand", IE_Pressed, this, &ABaseCharacter::OnRightTriggerPress);

	//Equip Item: Left Hand
	playerInputComponent->BindAction("EquipItemLeftHand", IE_Pressed, this, &ABaseCharacter::OnEquipLeftHandItem);
	
	//Equip Item: Right Hand
	playerInputComponent->BindAction("EquipItemRightHand", IE_Pressed, this, &ABaseCharacter::OnEquipRightHandItem);

	//Crush Item: Left Hand
	playerInputComponent->BindAction("CrushItemLeftHand", IE_Pressed, this, &ABaseCharacter::OnLeftGripPress);
	playerInputComponent->BindAction("CrushItemLeftHand", IE_Released, this, &ABaseCharacter::OnLeftGripRelease);

	//Crush Item: Right Hand
	playerInputComponent->BindAction("CrushItemRightHand", IE_Pressed, this, &ABaseCharacter::OnRightGripPress);
	playerInputComponent->BindAction("CrushItemRightHand", IE_Released, this, &ABaseCharacter::OnRightGripRelease);

	//Reset VR View
	playerInputComponent->BindAction("ResetVR", IE_Pressed, this, &ABaseCharacter::OnResetVR);

	//Player Movement Forward/Backward
	playerInputComponent->BindAxis("MoveForward", this, &ABaseCharacter::MoveForward);
	playerInputComponent->BindAxis("MoveStafing", this, &ABaseCharacter::MoveStafing);
}

/**
 * Ticks.
 *
 * @param deltaTime The delta time.
 */
void ABaseCharacter::Tick(float deltaTime)
{
	Super::Tick(deltaTime);

	//TODO: TWEEK SENSITIVITY AND TIMING OF AUTOMATIC FIRE (ITS WAY TO FAST AND SENSITIVE).
	//Handle if the user is holding an input such as the trigger.
	//if (myIsLeftTriggerPressed == true)
	//{
	//	ABaseCharacter::OnLeftTriggerHeld();
	//}
	//if (myIsRightTriggerPressed == true)
	//{
	//	ABaseCharacter::OnRightTriggerHeld();
	//}

}

/** Functions. */
void ABaseCharacter::BeginPlay()
{
	FActorSpawnParameters spawnParams;

	//Call the base class  
	Super::BeginPlay();

	//Create and spawn the two hands; specifics are specified in the blueprints.
	if (myLeftHandBlueprint != NULL)
	{
		myLeftHand = GetWorld()->SpawnActor<ABaseHand>(myLeftHandBlueprint);
		if (myLeftHand != NULL)
		{
			myLeftHand->SetActorLocation(this->GetActorLocation());
			myLeftHand->AttachToComponent(RootComponent, FAttachmentTransformRules(EAttachmentRule::SnapToTarget, false), TEXT("LeftHandToCharacter"));
		}
	}
	if (myRightHandBlueprint != NULL)
	{
		myRightHand = GetWorld()->SpawnActor<ABaseHand>(myRightHandBlueprint);
		if (myRightHand != NULL)
		{
			myRightHand->SetActorLocation(this->GetActorLocation());
			myRightHand->AttachToComponent(RootComponent, FAttachmentTransformRules(EAttachmentRule::SnapToTarget, false), TEXT("RightHandToCharacter"));
		}
	}
}

//Called when the user presses the left trigger.
void ABaseCharacter::OnLeftTriggerPress()
{
	if (myLeftHand != NULL)
	{
		//Only allow grabbing if the hand isn't already holding something.
		if (myLeftHand->GetIsItemEquipped() == false)
		{
			this->OnGrabLeftHand();
		}
		else
		{
			//User pressed the trigger button with an equipped item.
			this->OnUseLeftHandItem();
		}

		myIsLeftTriggerPressed = true;
	}
}

void ABaseCharacter::OnLeftTriggerHeld()
{
	ABaseItem* heldItem = NULL;

	if (myLeftHand != NULL)
	{
		heldItem = myLeftHand->GetGrabbedItem();
		if (heldItem != NULL && myLeftHand == heldItem->GetPrimaryHand())
		{
			//Handle holding only if it makes sense for the item.
			if (heldItem->GetIsFullAutomatic() == true)
			{
				//Keep calling use until button is released.
				this->OnUseLeftHandItem();
			}
		}
	}
}

//Called when the user presses the right trigger.
void ABaseCharacter::OnRightTriggerPress()
{
	if (myRightHand != NULL)
	{
		//Only allow grabbing if the hand isn't already holding something.
		if (myRightHand->GetIsItemEquipped() == false)
		{
			//Attempt to grab with the right hand.
			this->OnGrabRightHand();
		}
		else
		{
			//User pressed the trigger button with an equipped item.
			this->OnUseRightHandItem();
		}

		myIsRightTriggerPressed = true;
	}
}

void ABaseCharacter::OnRightTriggerHeld()
{
	ABaseItem* heldItem = NULL;

	if (myRightHand != NULL)
	{
		heldItem = myRightHand->GetGrabbedItem();
		if (heldItem != NULL && myRightHand == heldItem->GetPrimaryHand())
		{
			//Handle holding only if it makes sense for the item.
			if (heldItem->GetIsFullAutomatic() == true)
			{
				//Keep calling use until button is released.
				this->OnUseRightHandItem();
			}
		}
	}
}

//Called when the user presses the grip buttons on the left controller.
void ABaseCharacter::OnLeftGripPress()
{
	if (myLeftHand != NULL)
	{
		//Only allow crushing if the item isn't an equipable.
		if (myLeftHand->GetIsItemEquipped() == false)
		{
			//Attempt to grab with the left hand.
			this->OnCrushLeftHandItem();
		}
		else
		{
			//User pressed the grip button with an equipped item.
			this->OnUnEquipLeftHandItem();
		}
	}
}

//Called when the user presses the grip buttons on the right controller.
void ABaseCharacter::OnRightGripPress()
{
	if (myRightHand != NULL)
	{
		//Only allow crushing if the item isn't an equipable.
		if (myRightHand->GetIsItemEquipped() == false)
		{
			this->OnCrushRightHandItem();
		}
		else
		{
			//User pressed the grip button with an equipped item.
			this->OnUnEquipRightHandItem();
		}
	}
}

//Called automatically when the user releases the left trigger.
void ABaseCharacter::OnLeftTriggerRelease()
{
	if (myLeftHand != NULL)
	{
		//Don't drop the item if its equipped, You must unequip to do that.
		if (myLeftHand->GetIsItemEquipped() == false)
		{
			//OK fine drop the fucking thing.
			myLeftHand->Drop();
		}

		myIsLeftTriggerPressed = false;
	}
}

//Called automatically when the user releases the right trigger.
void ABaseCharacter::OnRightTriggerRelease()
{
	if (myRightHand != NULL)
	{
		//Don't drop the item if its equipped, You must unequip to do that.
		if (myRightHand->GetIsItemEquipped() == false)
		{
			//OK fine drop the fucking thing.
			myRightHand->Drop();
		}

		myIsRightTriggerPressed = false;
	}
}

//Called automatically when the user releases the grip buttons on the left controller.
void ABaseCharacter::OnLeftGripRelease()
{
	if (myLeftHand != NULL)
	{
		//Equipable items cannot be crushed.
		if (myLeftHand->GetIsItemEquipped() == false)
		{
			//Handle reverting the hand back to hold state.
			myLeftHand->SetIsCurrentlyCrushing(false);
		}
	}
}

//Called automatically when the user releases the grip buttons on the right controller.
void ABaseCharacter::OnRightGripRelease()
{
	if (myRightHand != NULL)
	{
		//Equipable items cannot be crushed.
		if (myRightHand->GetIsItemEquipped() == false)
		{
			//Handle reverting the hand back to hold state.
			myRightHand->SetIsCurrentlyCrushing(false);
		}
	}
}

//Use the special ability of the item the user is holding in the left hand.
void ABaseCharacter::OnUseLeftHandItem()
{
	if (myLeftHand != NULL)
	{
		myLeftHand->UseItem();
	}
}

//Use the special ability of the item the user is holding in the right hand.
void ABaseCharacter::OnUseRightHandItem()
{
	if (myRightHand != NULL)
	{
		myRightHand->UseItem();
	}
}

void ABaseCharacter::OnEquipLeftHandItem()
{
	if (myLeftHand != NULL)
	{
		myLeftHand->EquipItem();
	}
}

void ABaseCharacter::OnEquipRightHandItem()
{
	if (myRightHand != NULL)
	{
		myRightHand->EquipItem();
	}
}

void ABaseCharacter::OnUnEquipLeftHandItem()
{
	if (myLeftHand != NULL)
	{
		myLeftHand->UnEquipItem();
	}
}

void ABaseCharacter::OnUnEquipRightHandItem()
{
	if (myRightHand != NULL)
	{
		myRightHand->UnEquipItem();
	}
}

/** Executes the grab left hand action. */
void ABaseCharacter::OnGrabLeftHand()
{
	if (myLeftHand != NULL)
	{
		myLeftHand->Grab(this);
	}
}

/** Executes the grab right hand action. */
void ABaseCharacter::OnGrabRightHand()
{
	if (myRightHand != NULL)
	{
		myRightHand->Grab(this);
	}
}

void ABaseCharacter::OnCrushLeftHandItem()
{
	if (myLeftHand != NULL)
	{
		myLeftHand->Crush(myStrength);
	}
}

void ABaseCharacter::OnCrushRightHandItem()
{
	if (myRightHand != NULL)
	{
		myRightHand->Crush(myStrength);
	}
}

/** Executes the reset VR action. */
void ABaseCharacter::OnResetVR()
{
	UHeadMountedDisplayFunctionLibrary::ResetOrientationAndPosition();
}

/**
 * Move forward.
 *
 * @param value The value.
 */
void ABaseCharacter::MoveForward(float value)
{
	if (myFirstPersonCamera != NULL && value != 0.0f)
	{
		this->AddMovementInput(myFirstPersonCamera->GetForwardVector(), value);
	}
}

/**
 * Move strafing.
 *
 * @param value The value.
 */
void ABaseCharacter::MoveStafing(float value)
{
	if (myFirstPersonCamera != NULL && value != 0.0f)
	{
		AddMovementInput(myFirstPersonCamera->GetForwardVector(), value);
	}
}

/**
 * Turns.
 *
 * @param rate The rate.
 */
void ABaseCharacter::Turn(float rate)
{
	AddControllerYawInput(rate * myBaseTurnRate * GetWorld()->GetDeltaSeconds());
}

/**
 * Looks up a given key to find its associated value.
 *
 * @param rate The rate.
 */
void ABaseCharacter::LookUp(float rate)
{
	AddControllerPitchInput(rate * myBaseLookUpRate * GetWorld()->GetDeltaSeconds());
}

void ABaseCharacter::VibrateLeftController(EVibrationType vibrationType)
{
	if (myLeftHand != NULL)
	{
		myLeftHand->VibrateController(vibrationType);
	}
}

void ABaseCharacter::VibrateRightController(EVibrationType vibrationType)
{
	if (myRightHand != NULL)
	{
		myRightHand->VibrateController(vibrationType);
	}
}

///Getters
USkeletalMeshComponent* ABaseCharacter::GetFirstPersonMesh()
{
	return myFirstPersonMesh;
}

ABaseHand* ABaseCharacter::GetLeftHand()
{
	return myLeftHand;
}

ABaseHand* ABaseCharacter::GetRightHand()
{
	return myRightHand;
}

float ABaseCharacter::GetBaseTurnRate()
{
	return myBaseTurnRate;
}

float ABaseCharacter::GetBaseLookUpRate()
{
	return myBaseLookUpRate;
}

float ABaseCharacter::GetStrength()
{
	return myStrength;
}

bool ABaseCharacter::GetAreBothHandsGrabbing()
{
	if (myLeftHand != NULL && myRightHand != NULL)
	{
		if (myLeftHand->GetIsCurrentlyGrabbing() == true && myRightHand->GetIsCurrentlyGrabbing() == true)
		{
			return true;
		}
	}

	return false;
}

///Setters
void ABaseCharacter::SetBaseTurnRate(float baseTurnRate)
{
	myBaseTurnRate = baseTurnRate;
}

void ABaseCharacter::SetBaseLookUpRate(float baseLookUpRate)
{
	myBaseLookUpRate = baseLookUpRate;
}
