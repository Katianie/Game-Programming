// Copyright 1998-2016 Epic Games, Inc. All Rights Reserved.

#include "Smash.h"
#include "BaseProjectile.h"
#include "GameFramework/ProjectileMovementComponent.h"

/** Default constructor. */
ABaseProjectile::ABaseProjectile()
{
	// Use a sphere as a simple collision representation
	myCollisionComponent = CreateDefaultSubobject<USphereComponent>(TEXT("SphereComp"));
	myCollisionComponent->InitSphereRadius(2.0f);
	myCollisionComponent->BodyInstance.SetCollisionProfileName("Projectile");
	myCollisionComponent->OnComponentHit.AddDynamic(this, &ABaseProjectile::OnHit);		// set up a notification for when this component hits something blocking

	// Players can't walk on it
	myCollisionComponent->SetWalkableSlopeOverride(FWalkableSlopeOverride(WalkableSlope_Unwalkable, 0.0f));
	myCollisionComponent->CanCharacterStepUpOn = ECB_No;

	// Set as root component
	RootComponent = myCollisionComponent;

	// Use a ProjectileMovementComponent to govern this projectile's movement
	myProjectileMovementComponent = CreateDefaultSubobject<UProjectileMovementComponent>(TEXT("ProjectileComp"));
	myProjectileMovementComponent->UpdatedComponent = myCollisionComponent;
	myProjectileMovementComponent->InitialSpeed = 1000.0f;
	myProjectileMovementComponent->MaxSpeed = 3000.0f;
	myProjectileMovementComponent->bRotationFollowsVelocity = true;
	myProjectileMovementComponent->bShouldBounce = false;

	// Die after 3 seconds by default
	InitialLifeSpan = 3.0f;
}

/**
 * Executes the hit action.
 *
 * @param [in,out] HitComp    If non-null, the hit component.
 * @param [in,out] OtherActor If non-null, the other actor.
 * @param [in,out] OtherComp  If non-null, the other component.
 * @param NormalImpulse		  The normal impulse.
 * @param Hit				  The hit.
 */
void ABaseProjectile::OnHit(UPrimitiveComponent* HitComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, FVector NormalImpulse, const FHitResult& Hit)
{
	// Only add impulse and destroy projectile if we hit a physics
	if ((OtherActor != NULL) && (OtherActor != this) && (OtherComp != NULL) && OtherComp->IsSimulatingPhysics())
	{
		OtherComp->AddImpulseAtLocation(GetVelocity() * 100.0f, GetActorLocation());

		Destroy();
	}
}

///Getters
USphereComponent* ABaseProjectile::GetCollisionComponent()
{ 
	return myCollisionComponent;
}

UProjectileMovementComponent* ABaseProjectile::GetProjectileMovementComponent()
{ 
	return myProjectileMovementComponent;
}