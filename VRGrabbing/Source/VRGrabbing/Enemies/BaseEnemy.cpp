// Copyright 1998-2016 Epic Games, Inc. All Rights Reserved.
#include "Smash.h"
#include "BaseEnemy.h"
#include "BaseProjectile.h"
#include "Animation/AnimInstance.h"
#include "GameFramework/InputSettings.h"
#include "Kismet/HeadMountedDisplayFunctionLibrary.h"
#include "MotionControllerComponent.h"

ABaseEnemy::ABaseEnemy()
{
	//Enable actor ticking.
	PrimaryActorTick.bCanEverTick = true;

	// Set size for collision capsule
	GetCapsuleComponent()->InitCapsuleSize(40.f, 80.0f);

	// set our turn rates for input
	myEnemyTurnRate = 45.0f;
	myEnemyLookUpRate = 45.0f;

	// Create a mesh component that will be used when being viewed from a '1st person' view (when controlling this pawn)
	myEnemyArms = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("EnemyArms"));
	myEnemyArms->SetOnlyOwnerSee(false);
	myEnemyArms->SetupAttachment(RootComponent);
	myEnemyArms->bCastDynamicShadow = false;
	myEnemyArms->CastShadow = true;
	myEnemyArms->RelativeRotation = FRotator(1.9f, -19.19f, 5.2f);
	myEnemyArms->RelativeLocation = FVector(-0.5f, -4.4f, -155.7f);

	//Create a gun mesh component
	myEnemyGun = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("EnemyGun"));
	myEnemyGun->SetOnlyOwnerSee(false);			// only the owning player will see this mesh
	myEnemyGun->bCastDynamicShadow = false;
	myEnemyGun->CastShadow = false;
	myEnemyGun->SetupAttachment(myEnemyArms, TEXT("GripPoint"));

	myEnemyGunMuzzleLocation = CreateDefaultSubobject<USceneComponent>(TEXT("EnemyMuzzleLocation"));
	myEnemyGunMuzzleLocation->SetupAttachment(myEnemyGun);
	myEnemyGunMuzzleLocation->SetRelativeLocation(FVector(0.2f, 48.4f, -10.6f));

	//Default offset from the character location for projectiles to spawn
	myEnemyGunOffset = FVector(100.0f, 0.0f, 10.0f);
}

void ABaseEnemy::BeginPlay()
{
	// Call the base class  
	Super::BeginPlay();

	//Attach gun mesh component to Skeleton, doing it here because the skeleton is not yet created in the constructor
	myEnemyGun->AttachToComponent(myEnemyArms, FAttachmentTransformRules(EAttachmentRule::SnapToTarget, true), TEXT("GripPoint"));
}

int tempcount = 1;
void ABaseEnemy::Tick(float deltaTime)
{
	Super::Tick(deltaTime);

	//if (tempcount % 10 == 0)
	//{
	//	ABaseEnemy::OnFire();
	//	tempcount = 1;
	//}

	//tempcount++;
}

void ABaseEnemy::OnFire()
{
	// try and fire a projectile
	if (myEnemyProjectile != NULL)
	{
		UWorld* const World = GetWorld();
		if (World != NULL)
		{
			const FRotator SpawnRotation = GetControlRotation();
			// MuzzleOffset is in camera space, so transform it to world space before offsetting from the character location to find the final muzzle position
			const FVector SpawnLocation = ((myEnemyGunMuzzleLocation != nullptr) ? myEnemyGunMuzzleLocation->GetComponentLocation() : GetActorLocation()) + SpawnRotation.RotateVector(myEnemyGunOffset);

			//Set Spawn Collision Handling Override
			FActorSpawnParameters ActorSpawnParams;
			ActorSpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AdjustIfPossibleButDontSpawnIfColliding;

			// spawn the projectile at the muzzle
			World->SpawnActor<ABaseProjectile>(myEnemyProjectile, SpawnLocation, SpawnRotation, ActorSpawnParams);

		}
	}

	// try and play the sound if specified
	if (myEnemyFireSound != NULL)
	{
		UGameplayStatics::PlaySoundAtLocation(this, myEnemyFireSound, GetActorLocation());
	}

	// try and play a firing animation if specified
	if (myEnemyFireAnimation != NULL)
	{
		// Get the animation object for the arms mesh
		UAnimInstance* AnimInstance = myEnemyArms->GetAnimInstance();
		if (AnimInstance != NULL)
		{
			AnimInstance->Montage_Play(myEnemyFireAnimation, 1.f);
		}
	}
}

void ABaseEnemy::MoveForward(float Value)
{
	if (Value != 0.0f)
	{
		// add movement in that direction
		AddMovementInput(GetActorForwardVector(), Value);
	}
}

void ABaseEnemy::MoveRight(float Value)
{
	if (Value != 0.0f)
	{
		// add movement in that direction
		AddMovementInput(GetActorRightVector(), Value);
	}
}

void ABaseEnemy::TurnAtRate(float Rate)
{
	// calculate delta for this frame from the rate information
	AddControllerYawInput(Rate * myEnemyTurnRate * GetWorld()->GetDeltaSeconds());
}

void ABaseEnemy::LookUpAtRate(float Rate)
{
	// calculate delta for this frame from the rate information
	AddControllerPitchInput(Rate * myEnemyLookUpRate * GetWorld()->GetDeltaSeconds());
}
