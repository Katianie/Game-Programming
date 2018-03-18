#include "Smash.h"
#include "Runtime/Engine/Classes/Kismet/KismetMathLibrary.h"
#include "OneHandGunWeapon.h"

///Constructor
AOneHandGunWeapon::AOneHandGunWeapon() : Super()
{
	myBulletSpeed = 5000.0f;
	myBulletLifetime = 10.0f;
	myFirePointComponent = NULL;
	myWeaponType = EWeaponType::VE_ONE_HANDED;
	myDamageModifier = 90.0;
	myRangeModifier = 30.0;
}

///Functions
void AOneHandGunWeapon::BeginPlay()
{
	Super::BeginPlay();

	//Get all the actor components in the Drawer Item blueprint.
	TArray<UActorComponent*> components = this->GetComponentsByClass(UActorComponent::StaticClass());
	for (int currComponentIndex = 0; currComponentIndex < components.Num(); currComponentIndex++)
	{
		if (components[currComponentIndex]->GetName().Equals("FirePoint", ESearchCase::IgnoreCase) == true)
		{
			myFirePointComponent = Cast<USceneComponent>(components[currComponentIndex]);
			break;
		}
	}
}

void AOneHandGunWeapon::OnPlayerUse()
{
	AStaticMeshActor* spawnedProjectile;
	FTransform firePointTransform;
	FVector bulletImpulse;

	Super::OnPlayerUse();

	//Shoot the gun.
	if (myFirePointComponent != NULL)
	{
		firePointTransform = UKismetMathLibrary::MakeTransform(myFirePointComponent->GetComponentLocation(), myFirePointComponent->GetComponentRotation(), FVector(0.07f, 0.07f, 0.07f));
		bulletImpulse = myFirePointComponent->GetForwardVector().GetSafeNormal() * myBulletSpeed;

		spawnedProjectile = this->GetWorld()->SpawnActor<AStaticMeshActor>(myProjectile, firePointTransform);
		if(spawnedProjectile != NULL)
		{
			spawnedProjectile->GetStaticMeshComponent()->AddImpulse(bulletImpulse);
			spawnedProjectile->SetLifeSpan(myBulletLifetime);

			//Play vibration when shooting gun.
			Super::VibrateTouchingHands(EVibrationType::VE_BIGSHOT);
		}
		
	}
}