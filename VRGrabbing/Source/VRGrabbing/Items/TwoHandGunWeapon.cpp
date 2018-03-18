#include "Smash.h"
#include "Runtime/Engine/Classes/Kismet/KismetMathLibrary.h"
#include "TwoHandGunWeapon.h"

///Constructor
ATwoHandGunWeapon::ATwoHandGunWeapon() : Super()
{
	myBulletSpeed = 5000.0f;
	myBulletLifetime = 10.0f;
	myFirePointComponent = NULL;
	myWeaponType = EWeaponType::VE_ONE_HANDED;
	myDamageModifier = 90.0;
	myRangeModifier = 30.0;
	myIsFullAutomatic = true;
}

///Functions
void ATwoHandGunWeapon::BeginPlay()
{
	FString currName;
	int numComponentsFound = 0;
	TArray<UActorComponent*> components;

	Super::BeginPlay();

	//Get all the scene components in the Marker Item blueprint.
	components = this->GetComponentsByClass(UActorComponent::StaticClass());
	for (int currComponentIndex = 0; numComponentsFound < 4 && currComponentIndex < components.Num(); currComponentIndex++)
	{
		currName = components[currComponentIndex]->GetName();
		if (currName.Equals("FirePoint", ESearchCase::IgnoreCase) == true)
		{
			myFirePointComponent = Cast<USceneComponent>(components[currComponentIndex]);
			numComponentsFound++;
		}
		else if (currName.Equals("Grip", ESearchCase::IgnoreCase) == true)
		{
			myGripComponent = Cast<USceneComponent>(components[currComponentIndex]);
			numComponentsFound++;
		}
		else if (currName.Equals("Handle", ESearchCase::IgnoreCase) == true)
		{
			myHandleComponent = Cast<USceneComponent>(components[currComponentIndex]);
			numComponentsFound++;
		}
		else if (currName.Equals("GunRoot", ESearchCase::IgnoreCase) == true)
		{
			myGunRootComponent = Cast<USceneComponent>(components[currComponentIndex]);
			numComponentsFound++;
		}
	}
}

void ATwoHandGunWeapon::Tick(float deltaTime)
{
	FVector handleLocalPos;
	FVector gripLocalPos;
	FRotator gunLookAtRotation;

	Super::Tick(deltaTime);

	//IF the grip is being touched as well, account for the rotation.
	if (mySecondaryHand != NULL && myGripComponent != NULL && myFirePointComponent != NULL && myHandleComponent != NULL)
	{
		handleLocalPos = UKismetMathLibrary::InverseTransformLocation(this->GetActorTransform(), myPrimaryHand->GetControllerLocation());
		gripLocalPos = UKismetMathLibrary::InverseTransformLocation(this->GetActorTransform(), mySecondaryHand->GetControllerLocation());

		//UE_LOG(LogTemp, Error, TEXT("handleLocalPos:%s"), *(handleLocalPos.ToString()));
		//UE_LOG(LogTemp, Error, TEXT("gripLocalPos:%s"), *(gripLocalPos.ToString()));

		gunLookAtRotation = UKismetMathLibrary::FindLookAtRotation(handleLocalPos, gripLocalPos);

		//Combines two rotators together. 
		FRotator temp = FRotator(0.0f, -90.0f, 0.0f);
		gunLookAtRotation = UKismetMathLibrary::ComposeRotators(temp, gunLookAtRotation);

		//UE_LOG(LogTemp, Error, TEXT("gunLookAtRotation:%s"), *(gunLookAtRotation.ToString()));
		myGunRootComponent->SetRelativeRotation(gunLookAtRotation);
	}
}

void ATwoHandGunWeapon::OnPlayerReceive(ABaseHand* owningHand)
{
	UPrimitiveComponent* handlePrimitiveComponent = NULL;

	//Attachment is different for the two handed weapon since rotation is based on the root.
	//Unfortunately the root is a scene component so it needs to be attached to a moveable part.
	if (owningHand != NULL)
	{
		handlePrimitiveComponent = Cast<UPrimitiveComponent>(myHandleComponent);
		if (handlePrimitiveComponent != NULL)
		{
			handlePrimitiveComponent->SetSimulatePhysics(false);

			handlePrimitiveComponent->AttachToComponent(myGunRootComponent, FAttachmentTransformRules(EAttachmentRule::SnapToTarget, EAttachmentRule::SnapToTarget, EAttachmentRule::KeepWorld, true));
		}

		this->AttachToComponent(owningHand->GetVRHandMesh(), FAttachmentTransformRules(EAttachmentRule::SnapToTarget, EAttachmentRule::SnapToTarget, EAttachmentRule::KeepWorld, true));
	}
}

void ATwoHandGunWeapon::OnPlayerUse()
{
	AStaticMeshActor* spawnedProjectile;
	FTransform firePointTransform;
	FVector bulletImpulse;

	Super::OnPlayerUse();

	//Shoot the gun.
	if (myFirePointComponent != NULL)
	{
		firePointTransform = UKismetMathLibrary::MakeTransform(myFirePointComponent->GetComponentLocation(), myFirePointComponent->GetComponentRotation(), FVector(0.05f, 0.05f, 0.05f));
		bulletImpulse = myFirePointComponent->GetForwardVector().GetSafeNormal() * myBulletSpeed;

		spawnedProjectile = this->GetWorld()->SpawnActor<AStaticMeshActor>(myProjectile, firePointTransform);
		if (spawnedProjectile != NULL)
		{
			spawnedProjectile->GetStaticMeshComponent()->AddImpulse(bulletImpulse);
			spawnedProjectile->SetLifeSpan(myBulletLifetime);

			//Play vibration when shooting gun.
			Super::VibrateTouchingHands(EVibrationType::VE_SMALLSHOT);
		}
	}
}
